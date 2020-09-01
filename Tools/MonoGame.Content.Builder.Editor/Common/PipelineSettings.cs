// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

namespace MonoGame.Tools.Pipeline
{
    public class PipelineSettings
    {
        private const string SettingsPath = "Settings.xml";
        private IsolatedStorageFile _isoStore;
        private bool _isoStoreInit;

        public static PipelineSettings Default { get; private set; }

        public List<string> ProjectHistory;
        public string StartupProject;
        public Framework.Point Size;
        public int HSeparator, VSeparator;
        public bool Maximized, DebugMode, PropertyGroupSort;
        public bool FilterOutput, FilterShowSkipped, FilterShowSuccessful, FilterShowCleaned, AutoScrollBuildOutput;
        public string ErrorMessage;

        static PipelineSettings()
        {
            Default = new PipelineSettings();
        }

        public PipelineSettings()
        {
            ProjectHistory = new List<string>();

            PropertyGroupSort = true;
            FilterOutput = true;
            FilterShowSkipped = true;
            FilterShowSuccessful = true;
            FilterShowCleaned = true;
            AutoScrollBuildOutput = true;

            try
            {
                _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                _isoStoreInit = true;
            }
            catch
            {
            }
        }

        /// <summary>
        /// If the project already exists in history, it will be moved to the end.
        /// </summary>
        public void AddProjectHistory(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string cleanFile = file.Trim();
            ProjectHistory.Remove(cleanFile);
            ProjectHistory.Add(cleanFile);
        }

        public void RemoveProjectHistory(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            string cleanFile = file.Trim();
            ProjectHistory.Remove(cleanFile);
        }

        public void Clear()
        {
            ProjectHistory.Clear();
            StartupProject = null;
            Save();
        }

        public void Save()
        {
            if (!_isoStoreInit)
                return;

            try
            {
                var mode = FileMode.CreateNew;
                if (_isoStore.FileExists(SettingsPath))
                    mode = FileMode.Truncate;

                using var isoStream = new IsolatedStorageFileStream(SettingsPath, mode, _isoStore);
                using var writer = new StreamWriter(isoStream);
                var serializer = new XmlSerializer(typeof(PipelineSettings));
                serializer.Serialize(writer, this);
            }
            catch
            {
            }
        }

        public void Load()
        {
            if (!_isoStoreInit)
                return;

            try
            {
                if (!_isoStore.FileExists(SettingsPath))
                    return;
                
                using var isoStream = new IsolatedStorageFileStream(
                    SettingsPath, FileMode.Open, _isoStore);

                using var reader = new XmlTextReader(isoStream);
                var serializer = new XmlSerializer(typeof(PipelineSettings));
                Default = (PipelineSettings)serializer.Deserialize(reader);

                for (int i = Default.ProjectHistory.Count; i-- > 0;)
                {
                    string entry = Default.ProjectHistory[i];

                    if (!File.Exists(entry))
                        Default.ProjectHistory.RemoveAt(i);

                }
            }
            catch
            {
                Save();
            }
        }
    }
}

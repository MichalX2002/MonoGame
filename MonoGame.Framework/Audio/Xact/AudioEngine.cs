// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MonoGame.Framework.Audio
{
    /// <summary>
    /// Class used to create and manipulate code audio objects.
    /// </summary> 
    public class AudioEngine : IDisposable
    {
        private readonly Dictionary<string, int> _categoryLookup = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _variableLookup = new Dictionary<string, int>();

        private readonly RpcVariable[] _variables;
        private readonly RpcVariable[] _cueVariables;

        private readonly Stopwatch _stopwatch;
        private TimeSpan _lastUpdateTime;

        private readonly ReverbSettings? _reverbSettings;
        private readonly RpcCurve[] _reverbCurves;

        internal readonly RpcCurve[] RpcCurves;
        internal List<Cue> ActiveCues = new List<Cue>();
        internal Dictionary<string, WaveBank> Wavebanks = new Dictionary<string, WaveBank>();

        internal object UpdateLock { get; } = new object();
        internal AudioCategory[] Categories { get; }

        /// <summary>
        /// The current content version.
        /// </summary>
        public const int ContentVersion = 39;

        /// <summary>
        /// This event is triggered when the audio engine is disposed.
        /// </summary>
        public event Event<AudioEngine>? Disposed;

        /// <summary>
        /// Gets whether the audio engine has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <param name="settingsFile">Path to a XACT settings file.</param>
        public AudioEngine(string settingsFile) : this(settingsFile, TimeSpan.Zero, "")
        {
        }

        internal static Stream OpenStream(string filePath)
        {
            var stream = TitleContainer.OpenStream(filePath);
#if ANDROID
            stream = RecyclableMemoryManager.Default.GetBufferedStream(stream, leaveOpen: false);
#endif
            return stream;
        }

        internal RpcVariable[] CreateCueVariables()
        {
            var clone = new RpcVariable[_cueVariables.Length];
            Array.Copy(_cueVariables, clone, _cueVariables.Length);
            return clone;
        }

        /// <param name="settingsFile">Path to a XACT settings file.</param>
        /// <param name="lookAheadTime">
        /// Determines how many milliseconds the engine will look ahead when
        /// determing when to transition to another sound.
        /// </param>
        /// <param name="rendererId">A string that specifies the audio renderer to use.</param>
        /// <remarks>
        /// For the best results, use a <paramref name="lookAheadTime"/> of 250 milliseconds or greater.
        /// </remarks>
        public AudioEngine(string settingsFile, TimeSpan lookAheadTime, string rendererId)
        {
            if (string.IsNullOrEmpty(settingsFile))
                throw new ArgumentNullException(nameof(settingsFile));

            // Read the xact settings file
            // Credits to alisci01 for initial format documentation
            using (var reader = new BinaryReader(OpenStream(settingsFile)))
            {
                uint magic = reader.ReadUInt32();
                if (magic != 0x46534758) //'XGFS'
                    throw new InvalidDataException("XGS format not recognized");

                reader.ReadUInt16(); // toolVersion
                uint formatVersion = reader.ReadUInt16();
                if (formatVersion != 42)
                    Debug.WriteLine("Warning: XGS format " + formatVersion + " not supported!");

                reader.ReadUInt16(); // crc
                reader.ReadUInt32(); // lastModifiedLow
                reader.ReadUInt32(); // lastModifiedHigh
                reader.ReadByte(); //unkn, 0x03. Platform?

                uint numCats = reader.ReadUInt16();
                uint numVars = reader.ReadUInt16();

                reader.ReadUInt16(); //unkn, 0x16
                reader.ReadUInt16(); //unkn, 0x16

                uint numRpc = reader.ReadUInt16();
                uint numDspPresets = reader.ReadUInt16();
                uint numDspParams = reader.ReadUInt16();

                uint catsOffset = reader.ReadUInt32();
                uint varsOffset = reader.ReadUInt32();

                reader.ReadUInt32(); //unknown, leads to a short with value of 1?
                reader.ReadUInt32(); // catNameIndexOffset
                reader.ReadUInt32(); //unknown, two shorts of values 2 and 3?
                reader.ReadUInt32(); // varNameIndexOffset

                uint catNamesOffset = reader.ReadUInt32();
                uint varNamesOffset = reader.ReadUInt32();
                uint rpcOffset = reader.ReadUInt32();
                reader.ReadUInt32(); // dspPresetsOffset
                uint dspParamsOffset = reader.ReadUInt32();

                reader.BaseStream.Seek(catNamesOffset, SeekOrigin.Begin);
                string[] categoryNames = ReadNullTerminatedStrings(numCats, reader);

                Categories = new AudioCategory[numCats];
                reader.BaseStream.Seek(catsOffset, SeekOrigin.Begin);
                for (int i = 0; i < numCats; i++)
                {
                    Categories[i] = new AudioCategory(this, categoryNames[i], reader);
                    _categoryLookup.Add(categoryNames[i], i);
                }

                reader.BaseStream.Seek(varNamesOffset, SeekOrigin.Begin);
                string[] varNames = ReadNullTerminatedStrings(numVars, reader);

                var variables = new List<RpcVariable>();
                var cueVariables = new List<RpcVariable>();
                var globalVariables = new List<RpcVariable>();
                reader.BaseStream.Seek(varsOffset, SeekOrigin.Begin);
                for (var i = 0; i < numVars; i++)
                {
                    var v = new RpcVariable
                    {
                        Name = varNames[i],
                        Flags = reader.ReadByte(),
                        InitValue = reader.ReadSingle(),
                        MinValue = reader.ReadSingle(),
                        MaxValue = reader.ReadSingle()
                    };
                    v.Value = v.InitValue;

                    variables.Add(v);
                    if (!v.IsGlobal)
                        cueVariables.Add(v);
                    else
                    {
                        globalVariables.Add(v);
                        _variableLookup.Add(v.Name, globalVariables.Count - 1);
                    }
                }
                _cueVariables = cueVariables.ToArray();
                _variables = globalVariables.ToArray();

                var reverbCurves = new List<RpcCurve>();
                RpcCurves = new RpcCurve[numRpc];
                if (numRpc > 0)
                {
                    reader.BaseStream.Seek(rpcOffset, SeekOrigin.Begin);
                    for (var i = 0; i < numRpc; i++)
                    {
                        var curve = new RpcCurve
                        {
                            FileOffset = (uint)reader.BaseStream.Position
                        };

                        var variable = variables[reader.ReadUInt16()];
                        if (variable.IsGlobal)
                        {
                            curve.IsGlobal = true;
                            curve.Variable = globalVariables.FindIndex(e => e.Name == variable.Name);
                        }
                        else
                        {
                            curve.IsGlobal = false;
                            curve.Variable = cueVariables.FindIndex(e => e.Name == variable.Name);
                        }

                        var pointCount = (int)reader.ReadByte();
                        curve.Parameter = (RpcParameter)reader.ReadUInt16();

                        curve.Points = new RpcPoint[pointCount];
                        for (var j = 0; j < pointCount; j++)
                        {
                            curve.Points[j].Position = reader.ReadSingle();
                            curve.Points[j].Value = reader.ReadSingle();
                            curve.Points[j].Type = (RpcPointType)reader.ReadByte();
                        }

                        // If the parameter is greater than the max then this is a DSP
                        // parameter which is for reverb.
                        var dspParameter = curve.Parameter - RpcParameter.NumParameters;
                        if (dspParameter >= 0 && variable.IsGlobal)
                            reverbCurves.Add(curve);

                        RpcCurves[i] = curve;
                    }
                }
                _reverbCurves = reverbCurves.ToArray();

                if (numDspPresets > 0)
                {
                    // Note:  It seemed like MS designed this to support multiple
                    // DSP effects, but in practice XACT only has one... Microsoft Reverb.
                    //
                    // So because of this we know exactly how many presets and 
                    // parameters we should have.
                    if (numDspPresets != 1)
                        throw new Exception("Unexpected number of DSP presets!");
                    if (numDspParams != 22)
                        throw new Exception("Unexpected number of DSP parameters!");

                    reader.BaseStream.Seek(dspParamsOffset, SeekOrigin.Begin);
                    _reverbSettings = new ReverbSettings(reader);
                }
            }

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        internal int GetRpcIndex(uint fileOffset)
        {
            for (var i = 0; i < RpcCurves.Length; i++)
            {
                if (RpcCurves[i].FileOffset == fileOffset)
                    return i;
            }
            return -1;
        }

        private static string[] ReadNullTerminatedStrings(uint count, BinaryReader reader)
        {
            var strings = new string[count];
            var builder = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                while (reader.PeekChar() != 0)
                    builder.Append(reader.ReadChar());
                reader.ReadChar(); // skip NULL

                strings[i] = builder.ToString();
                builder.Clear();
            }

            return strings;
        }

        /// <summary>
        /// Performs periodic work required by the audio engine.
        /// </summary>
        /// <remarks>Must be called at least once per frame.</remarks>
        public void Update()
        {
            TimeSpan current = _stopwatch.Elapsed;
            TimeSpan elapsed = current - _lastUpdateTime;
            _lastUpdateTime = current;

            float deltaTime = (float)elapsed.TotalSeconds;

            lock (UpdateLock)
            {
                for (int i = 0; i < ActiveCues.Count;)
                {
                    var cue = ActiveCues[i];
                    cue.Update(deltaTime);

                    if (cue.IsStopped || cue.IsDisposed)
                    {
                        ActiveCues.Remove(cue);
                        continue;
                    }

                    i++;
                }
            }

            // The only global curves we can process seem to be 
            // specifically for the reverb DSP effect.
            if (_reverbSettings != null)
            {
                for (int i = 0; i < _reverbCurves.Length; i++)
                {
                    var curve = _reverbCurves[i];
                    float result = curve.Evaluate(_variables[curve.Variable].Value);
                    int parameter = curve.Parameter - RpcParameter.NumParameters;
                    _reverbSettings[parameter] = result;
                }

                SoundEffect.PlatformSetReverbSettings(_reverbSettings);
            }
        }

        /// <summary>Returns an audio category by name.</summary>
        /// <param name="name">Friendly name of the category to get.</param>
        /// <returns>The AudioCategory with a matching name. Throws an exception if not found.</returns>
        public AudioCategory GetCategory(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!_categoryLookup.TryGetValue(name, out int i))
                throw new InvalidOperationException("This resource could not be created.");

            return Categories[i];
        }

        /// <summary>Gets the value of a global variable.</summary>
        /// <param name="name">Friendly name of the variable.</param>
        /// <returns>float value of the queried variable.</returns>
        /// <remarks>A global variable has global scope. It can be accessed by all code within a project.</remarks>
        public float GetGlobalVariable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!_variableLookup.TryGetValue(name, out int i) || !_variables[i].IsPublic)
                throw new IndexOutOfRangeException("The specified variable index is invalid.");

            lock (UpdateLock)
                return _variables[i].Value;
        }

        internal float GetGlobalVariable(int index)
        {
            lock (UpdateLock)
                return _variables[index].Value;
        }

        /// <summary>Sets the value of a global variable.</summary>
        /// <param name="name">Friendly name of the variable.</param>
        /// <param name="value">Value of the global variable.</param>
        public void SetGlobalVariable(string name, float value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!_variableLookup.TryGetValue(name, out int i) || !_variables[i].IsPublic)
                throw new IndexOutOfRangeException("The specified variable index is invalid.");

            lock (UpdateLock)
                _variables[i].SetValue(value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                // TODO: Should we be forcing any active
                // audio cues to stop here?

                if (disposing)
                    Disposed?.Invoke(this);
            }
        }

        /// <summary>
        /// Disposes the <see cref="AudioEngine"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="AudioEngine"/>
        /// </summary>
        ~AudioEngine()
        {
            Dispose(false);
        }
    }
}


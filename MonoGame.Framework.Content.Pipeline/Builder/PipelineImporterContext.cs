// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.Framework.Content.Pipeline;

namespace MonoGame.Framework.Content.Pipeline.Builder
{
    public class PipelineImporterContext : ContentImporterContext
    {
        private readonly PipelineManager _manager;

        public PipelineImporterContext(PipelineManager manager)
        {
            _manager = manager;
        }

        public override string IntermediateDirectory => _manager.IntermediateDirectory;
        public override string OutputDirectory => _manager.OutputDirectory;
        public override ContentBuildLogger Logger => _manager.Logger;

        public override void AddDependency(string filename)
        {            
        }
    }
}
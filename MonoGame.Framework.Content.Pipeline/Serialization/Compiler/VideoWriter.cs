// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MonoGame.Framework.Content.Pipeline.Serialization.Compiler
{
    [ContentTypeWriter]
    class VideoWriter : BuiltInContentWriter<VideoContent>
    {
        protected internal override void Write(ContentWriter output, VideoContent value)
        {
            output.WriteObject(value.Filename);
            output.WriteObject((int)value.Duration.TotalMilliseconds);
            output.WriteObject(value.Width);
            output.WriteObject(value.Height);
            output.WriteObject(value.FramesPerSecond);
            output.WriteObject((int)value.VideoSoundtrackType);
        }
    }
}

// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using SharpDX;
using SharpDX.MediaFoundation;

namespace MonoGame.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        private Topology _topology;

        internal Topology Topology { get { return _topology; } }

        private void PlatformInitialize(string fileName)
        {
            if (_topology != null)
                return;

            MediaManagerState.CheckStartup();

            MediaFactory.CreateTopology(out _topology);

            SharpDX.MediaFoundation.MediaSource mediaSource;
            {
                SourceResolver resolver = new SourceResolver();

                ComObject source = resolver.CreateObjectFromURL(FilePath, SourceResolverFlags.MediaSource);
                mediaSource = source.QueryInterface<SharpDX.MediaFoundation.MediaSource>();
                resolver.Dispose();
                source.Dispose();
            }

            mediaSource.CreatePresentationDescriptor(out PresentationDescriptor presDesc);

            for (var i = 0; i < presDesc.StreamDescriptorCount; i++)
            {
                presDesc.GetStreamDescriptorByIndex(i, out SharpDX.Mathematics.Interop.RawBool selected, out StreamDescriptor desc);

                if (selected)
                {
                    MediaFactory.CreateTopologyNode(TopologyType.SourceStreamNode, out TopologyNode sourceNode);

                    sourceNode.Set(TopologyNodeAttributeKeys.Source, mediaSource);
                    sourceNode.Set(TopologyNodeAttributeKeys.PresentationDescriptor, presDesc);
                    sourceNode.Set(TopologyNodeAttributeKeys.StreamDescriptor, desc);

                    MediaFactory.CreateTopologyNode(TopologyType.OutputNode, out TopologyNode outputNode);

                    var typeHandler = desc.MediaTypeHandler;
                    var majorType = typeHandler.MajorType;
                    if (majorType != MediaTypeGuids.Audio)
                        throw new NotSupportedException("The song contains video data!");

                    MediaFactory.CreateAudioRendererActivate(out Activate activate);
                    outputNode.Object = activate;

                    _topology.AddNode(sourceNode);
                    _topology.AddNode(outputNode);
                    sourceNode.ConnectOutput(0, outputNode, 0);

                    sourceNode.Dispose();
                    outputNode.Dispose();
                    typeHandler.Dispose();
                    activate.Dispose();
                }

                desc.Dispose();
            }

            presDesc.Dispose();
            mediaSource.Dispose();
        }

        private void PlatformDispose(bool disposing)
        {
            if (_topology != null)
            {
                _topology.Dispose();
                _topology = null;
            }
        }
        
        private Album PlatformGetAlbum()
        {
            return null;
        }

        private Artist PlatformGetArtist()
        {
            return null;
        }

        private Genre PlatformGetGenre()
        {
            return null;
        }

        private TimeSpan PlatformGetDuration()
        {
            return _duration;
        }

        private bool PlatformIsProtected()
        {
            return false;
        }

        private bool PlatformIsRated()
        {
            return false;
        }

        private string PlatformGetName()
        {
            return Path.GetFileNameWithoutExtension(FilePath);
        }

        private int PlatformGetPlayCount()
        {
            return _playCount;
        }

        private int PlatformGetRating()
        {
            return 0;
        }

        private int PlatformGetTrackNumber()
        {
            return 0;
        }
    }
}


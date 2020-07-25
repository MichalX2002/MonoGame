// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MonoGame.OpenGL;

namespace MonoGame.Framework.Graphics
{
    partial class OcclusionQuery
    {
        private GLHandle _glQuery;

        private void PlatformConstruct()
        {
            GL.GenQueries(1, out int query);
            GL.CheckError();

            _glQuery = GLHandle.Query(query);
        }

        private void PlatformBegin()
        {
            GL.BeginQuery(QueryTarget.SamplesPassed, _glQuery);
            GL.CheckError();
        }

        private void PlatformEnd()
        {
            GL.EndQuery(QueryTarget.SamplesPassed);
            GL.CheckError();
        }

        private bool PlatformGetResult(out int pixelCount)
        {
            GL.GetQueryObject(_glQuery, GetQueryObjectParam.QueryResultAvailable, out int resultReady);
            GL.CheckError();

            if (resultReady == 0)
            {
                pixelCount = 0;
                return false;
            }

            GL.GetQueryObject(_glQuery, GetQueryObjectParam.QueryResult, out pixelCount);
            GL.CheckError();

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                GraphicsDevice.DisposeResource(_glQuery);
            }

            base.Dispose(disposing);
        }
    }
}


using System;

namespace MonoGame.Imaging
{
    internal unsafe class GifImage : IDisposable
    {
        public int w;
        public int h;
        public byte* _out_;
        public byte* old_out;
        public int flags;
        public int bgindex;
        public int ratio;
        public int transparent;
        public int eflags;
        public int delay;
        public MarshalPointer pal;
        public MarshalPointer lpal;
        public MarshalPointer codes;
        public byte* color_table;
        public int parse;
        public int step;
        public int lflags;
        public int start_x;
        public int start_y;
        public int max_x;
        public int max_y;
        public int cur_x;
        public int cur_y;
        public int line_size;

        public GifImage()
        {
            pal = Imaging.MAlloc(256 * 4);
            lpal = Imaging.MAlloc(256 * 4);
            codes = Imaging.MAlloc(sizeof(GifLzw) * 4096);
        }
        
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                pal.Dispose();
                lpal.Dispose();
                codes.Dispose();

                _disposed = true;
            }
        }

        ~GifImage()
        {
          Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

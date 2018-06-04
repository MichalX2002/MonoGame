
namespace MonoGame.Imaging
{
    internal unsafe class ReadContext
    {
        public uint img_x;
        public uint img_y;
        public int img_n;
        public int img_out_n;
        public ReadCallbacks io;
        public void* io_user_data;
        public int read_from_callbacks;
        public int buflen;
        public PinnedArray<byte> buffer_start = new PinnedArray<byte>(128);
        public byte* img_buffer;
        public byte* img_buffer_end;
        public byte* img_buffer_original;
        public byte* img_buffer_original_end;
    }
}

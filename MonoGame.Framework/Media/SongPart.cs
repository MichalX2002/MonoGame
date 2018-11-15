
namespace Microsoft.Xna.Framework.Media
{
    internal class SongPart
    {
        public readonly float[] Data;
        public int Count;

        public SongPart(int size)
        {
            Data = new float[size];
        }

        public unsafe void SetData(float[] buffer, int count)
        {
            for (int i = 0; i < count; i++)
                Data[i] = buffer[i];
            Count = count;
        }
    }
}

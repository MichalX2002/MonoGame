using System;
using System.IO;
using System.IO.Compression;
using MonoGame.Imaging;

namespace MonoGame.Imaging.Tests
{
    class Program
    {
        public const string DATA_FOLDER = "testdata.zip";

        static void Main(string[] args)
        {
            ZipArchive archive = ZipFile.OpenRead(DATA_FOLDER);
            MemoryManager manager = new MemoryManager(true);

            TestEntry(manager, archive, "bmp/8bit.bmp");
            TestEntry(manager, archive, "bmp/24bit.bmp");

            TestEntry(manager, archive, "jpg/quality_0.jpg");
            TestEntry(manager, archive, "jpg/quality_25.jpg");
            TestEntry(manager, archive, "jpg/quality_50.jpg");
            TestEntry(manager, archive, "jpg/quality_75.jpg");
            TestEntry(manager, archive, "jpg/quality_100.jpg");

            TestEntry(manager, archive, "png/32bit.png");
            TestEntry(manager, archive, "png/24bit.png");
            TestEntry(manager, archive, "png/8bit.png");

            TestEntry(manager, archive, "tga/32bit.tga");
            TestEntry(manager, archive, "tga/32bit_compressed.tga");
            TestEntry(manager, archive, "tga/24bit.tga");
            TestEntry(manager, archive, "tga/24bit_compressed.tga");

            TestEntry(manager, archive, "32bit.gif");

            archive.Dispose();
            manager.Dispose();

            Console.ReadKey();
        }

        static void TestEntry(MemoryManager manager, ZipArchive archive, string name)
        {
            using(var img = new Image(archive.GetEntry(name).Open(), false, manager, true))
            {
                ImageInfo imageInfo = img.Info;
                
                Console.WriteLine(name + ": " + (img.LastGetInfoFailed ? "Failed to read info" : "Retrieved info successfully"));

                Console.WriteLine("Loading data...");
                IntPtr data = img.GetDataPointer();
                if (data == null)
                    Console.WriteLine("Data Pointer NULL: " + img.LastError);
                else
                {
                    Console.WriteLine("Saving " + img.PointerSize + " bytes...");

                    FileInfo outputInfo = new FileInfo(name);
                    outputInfo.Directory.Create();

                    using (var fs = new FileStream(outputInfo.FullName, FileMode.Create))
                        img.Save(fs, ImageSaveFormat.Png);
                }

                Console.WriteLine();

                Console.WriteLine($"Memory Allocated (Pointers: {manager.AllocatedPointers}): " + manager.AllocatedBytes + " bytes");
                Console.WriteLine($"Lifetime Allocated (Pointers: {manager.LifetimeAllocatedPointers}): " + manager.LifetimeAllocatedBytes + " bytes");
                Console.WriteLine("----------------------------------------------------");
            }
        }
    }
}

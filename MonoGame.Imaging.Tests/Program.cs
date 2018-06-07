using System;
using System.Diagnostics;
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

            //TestEntry(manager, archive, "32bit.gif");

            archive.Dispose();
            manager.Dispose();

            Console.ReadKey();
        }

        static void TestEntry(MemoryManager manager, ZipArchive archive, string name)
        {
            Stopwatch watch = new Stopwatch();
            try
            {
                using (var img = new Image(archive.GetEntry(name).Open(), false, manager, true))
                {
                    watch.Start();
                    ImageInfo imageInfo = img.Info;
                    watch.Stop();
                    Console.WriteLine("Info Read Time: " + Math.Round(watch.Elapsed.TotalMilliseconds, 3) + "ms");

                    Console.WriteLine(name + ": " +
                        (img.LastGetInfoFailed ? "Failed to read info" : "Retrieved info successfully"));

                    Console.WriteLine($"Loading ({imageInfo}) data...");

                    watch.Restart();
                    IntPtr data = img.GetDataPointer();
                    watch.Stop();
                    Console.WriteLine("Pointer Read Time: " + Math.Round(watch.Elapsed.TotalMilliseconds, 3) + "ms");

                    if (data == null)
                        Console.WriteLine("Data Pointer NULL: " + img.LastError);
                    else
                    {
                        Console.WriteLine("Saving " + img.PointerLength + " bytes...");

                        FileInfo outputInfo = new FileInfo(name);
                        outputInfo.Directory.Create();

                        using (var fs = new FileStream(outputInfo.FullName, FileMode.Create))
                        {
                            watch.Restart();
                            img.Save(fs);
                            watch.Stop();
                            Console.WriteLine("Image Save Time: " + Math.Round(watch.Elapsed.TotalMilliseconds, 3) + "ms");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.WriteLine();

            Console.WriteLine($"Memory Allocated (Pointers: {manager.AllocatedPointers}): " + manager.AllocatedBytes + " bytes");
            Console.WriteLine($"Lifetime Allocated (Pointers: {manager.LifetimeAllocatedPointers}): " + manager.LifetimeAllocatedBytes + " bytes");
            Console.WriteLine("----------------------------------------------------");
        }
    }
}

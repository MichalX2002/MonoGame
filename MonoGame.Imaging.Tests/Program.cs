using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime;
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

            /*
            var watch = new Stopwatch();
            var fs = new FileStream("test.png", FileMode.Open);
            using(var img = new Image(fs, false, manager, true))
            {
                watch.Restart();
                img.GetImageInfo();
                watch.Stop();
                Console.WriteLine("Info: " + watch.Elapsed.TotalMilliseconds + "ms");

                watch.Restart();
                img.GetDataPointer();
                watch.Stop();
                Console.WriteLine("Pointer: " + watch.Elapsed.TotalMilliseconds + "ms");

                using (var outFs = new FileStream("out.png", FileMode.Create))
                {
                    watch.Restart();
                    img.Save(outFs);
                    watch.Stop();
                    Console.WriteLine("Saving: " + watch.Elapsed.TotalMilliseconds + "ms");
                }
            }
            */

            //TestEntry(manager, archive, "32bit.gif");

            archive.Dispose();
            manager.Dispose();

            Console.ReadKey();
        }

        static void TestEntry(MemoryManager manager, ZipArchive archive, string name)
        {
            Stopwatch watch = new Stopwatch();
            int tries = 50;

            byte[] buf = new byte[1024 * 128];

            try
            {
                var entry = archive.GetEntry(name);
                MemoryStream dataStream = new MemoryStream((int)entry.Length);
                entry.Open().CopyTo(dataStream);
                dataStream.Position = 0;

                double infoReadTime = 0;
                double pointerReadTime = 0;
                double imageSaveTime = 0;

                for (int i = 0; i < tries; i++)
                {
                    using (var img = new Image(dataStream, false, manager, true))
                    {
                        watch.Start();
                        ImageInfo imageInfo = img.Info;
                        watch.Stop();
                        infoReadTime += watch.Elapsed.TotalMilliseconds;

                        //Console.WriteLine(name + ": " + (img.LastGetInfoFailed ? "Failed to read info" : "Retrieved info successfully"));

                        //Console.WriteLine($"Loading ({imageInfo}) data...");

                        watch.Restart();
                        IntPtr data = img.GetDataPointer();
                        watch.Stop();
                        pointerReadTime += watch.Elapsed.TotalMilliseconds;

                        if (data == null)
                            Console.WriteLine("Data Pointer NULL: " + img.LastError);
                        else
                        {
                            //Console.WriteLine("Saving " + img.PointerLength + " bytes...");

                            FileInfo outputInfo = new FileInfo(name);
                            outputInfo.Directory.Create();

                            //using (var fs = new FileStream(outputInfo.FullName, FileMode.Create))
                            using (var fs = new MemoryStream(buf))
                            {
                                watch.Restart();
                                img.Save(fs);
                                watch.Stop();
                                imageSaveTime += watch.Elapsed.TotalMilliseconds;
                            }
                        }
                        dataStream.Position = 0;
                    }
                }

                Console.WriteLine();

                Console.WriteLine(name);
                Console.WriteLine("Info Read Avg: " + Math.Round(infoReadTime / tries, 2) + "ms");
                Console.WriteLine("Pointer Read Avg: " + Math.Round(pointerReadTime / tries, 2) + "ms");
                Console.WriteLine("Saving Time Avg: " + Math.Round(imageSaveTime / tries, 2) + "ms");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.WriteLine($"Memory Allocated (Pointers: {manager.AllocatedPointers}, Arrays: {manager.AllocatedArrays}): " + manager.AllocatedBytes + " bytes");
            Console.WriteLine($"Lifetime Allocated (Pointers: {manager.LifetimeAllocatedPointers}, Arrays: {manager.LifetimeAllocatedArrays}): " + manager.LifetimeAllocatedBytes + " bytes");
            Console.WriteLine("----------------------------------------------------");
        }
    }
}

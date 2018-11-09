﻿using MonoGame.Utilities.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace MonoGame.Imaging.Tests
{
    struct ColorRGBA
    {
        public byte R;
    }

    class Program
    {
        public const string DATA_ZIP = "testdata.zip";

        static void Main(string[] args)
        {
            using (Image img1 = new Image(File.OpenRead("texture_0.jpg"), ImagePixelFormat.RgbWithAlpha))
            //using (Image img2 = new Image(img1.Width, img1.Height, img1.PixelFormat))
            {
                /*
                unsafe
                {
                    byte* srcPtr = (byte*)img1.Pointer;
                    byte* dstPtr = (byte*)img2.Pointer;
                    int pixels = img1.PointerLength;

                    for (int i = 0; i < pixels; i++)
                    {
                        dstPtr[i] = srcPtr[i];
                    }
                }
                */
                Console.WriteLine(img1.Pointer);
                Console.WriteLine(img1.PointerLength);
                using (var fs = File.OpenWrite("texture.png"))
                    img1.Save(fs, ImageSaveFormat.Png);
            }

            ZipArchive archive = new ZipArchive(File.OpenRead(DATA_ZIP), ZipArchiveMode.Read, false);
            var manager = new RecyclableMemoryManager();

            SaveConfiguration d = new SaveConfiguration(true, 0, manager);
            SaveConfiguration nonD = new SaveConfiguration(false, 0, manager);
            var ms = new MemoryStream();

            TestEntry(ms, d, manager, archive, "bmp/8bit.bmp");
            TestEntry(ms, d, manager, archive, "bmp/24bit.bmp");

            TestEntry(ms, d, manager, archive, "jpg/quality_0.jpg");
            TestEntry(ms, d, manager, archive, "jpg/quality_25.jpg");
            TestEntry(ms, d, manager, archive, "jpg/quality_50.jpg");
            TestEntry(ms, d, manager, archive, "jpg/quality_75.jpg");
            TestEntry(ms, d, manager, archive, "jpg/quality_100.jpg");

            TestEntry(ms, d, manager, archive, "png/32bit.png");
            TestEntry(ms, d, manager, archive, "png/24bit.png");
            TestEntry(ms, d, manager, archive, "png/8bit.png");

            TestEntry(ms, nonD, manager, archive, "tga/32bit.tga");
            TestEntry(ms, d, manager, archive, "tga/32bit_compressed.tga");
            TestEntry(ms, nonD, manager, archive, "tga/24bit.tga");
            TestEntry(ms, d, manager, archive, "tga/24bit_compressed.tga");

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

            Console.ReadKey();
        }

        static void TestEntry(MemoryStream ms, 
            SaveConfiguration config, RecyclableMemoryManager manager, ZipArchive archive, string name)
        {
            Stopwatch watch = new Stopwatch();
            int tries = 1; //3000;

            try
            {
                var entry = archive.GetEntry(name);
                MemoryStream dataStream = new MemoryStream((int)entry.Length);
                using (var es = entry.Open())
                    es.CopyTo(dataStream);

                double infoReadTime = 0;
                double pointerReadTime = 0;
                double imageSaveTime = 0;

                for (int i = 0; i < tries; i++)
                {
                    dataStream.Position = 0;
                    using (var img = new Image(dataStream, true))
                    {
                        watch.Restart();
                        ImageInfo imageInfo = img.Info;
                        watch.Stop();
                        if(tries > 0)
                            infoReadTime += watch.Elapsed.TotalMilliseconds;

                        //Console.WriteLine(name + ": " + (img.LastGetInfoFailed ? "Failed to read info" : "Retrieved info successfully"));

                        //Console.WriteLine($"Loading ({imageInfo}) data...");

                        watch.Restart();
                        IntPtr data = img.Pointer;
                        watch.Stop();
                        if (tries > 0)
                            pointerReadTime += watch.Elapsed.TotalMilliseconds;

                        if (data == null)
                            Console.WriteLine("Data Pointer NULL: " + img.Errors);
                        else
                        {
                            //Console.WriteLine("Saving " + img.PointerLength + " bytes...");
                            
                            watch.Restart();

                            ms.Position = 0;
                            ms.SetLength(0);
                            img.Save(ms, imageInfo.SourceFormat.ToSaveFormat(), config);

                            watch.Stop();
                            if (tries > 0)
                                imageSaveTime += watch.Elapsed.TotalMilliseconds;
                        }
                    }
                }

                FileInfo outputInfo = new FileInfo("testoutput/" + name);
                outputInfo.Directory.Create();
                using (var fs = new FileStream(outputInfo.FullName, FileMode.Create))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }

                Console.WriteLine();

                Console.WriteLine(name);
                Console.WriteLine("Info Read Avg: " + Math.Round(infoReadTime / tries, 2) + "ms");
                Console.WriteLine("Pointer Read Avg: " + Math.Round(pointerReadTime / tries, 2) + "ms");
                Console.WriteLine("Saving Time Avg: " + Math.Round(imageSaveTime / tries, 2) + "ms");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            //Console.WriteLine($"Memory Allocated (Arrays: {manager.AllocatedArrays}): " + manager.AllocatedBytes + " bytes");
            //Console.WriteLine($"Lifetime Allocated (Arrays: {manager.LifetimeAllocatedArrays}): " + manager.AllocatedBytes + " bytes");
            Console.WriteLine("----------------------------------------------------");
        }
    }
}

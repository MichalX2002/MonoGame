﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using MonoGame.Framework;
using MonoGame.Framework.Memory;
using MonoGame.Imaging.Pixels;
using MonoGame.Imaging.Coders.Decoding;
using MonoGame.Imaging.Coders.Encoding;
using System.Numerics;
using StbSharp;
using MonoGame.Framework.Vectors;
using System.Security.Cryptography;
using System.Xml;
using MonoGame.Imaging.Processing;
using MonoGame.Imaging.Coders.Formats;
using StbSharp.ImageResize;
using MonoGame.Imaging.Coders.Formats.Jpeg;
using MonoGame.Imaging.Coders.Formats.Tga;

namespace MonoGame.Imaging.Tests
{
    class Program
    {
        public const string DataZip = "testdata.zip";

        static void Main(string[] args)
        {
            string dir = "savetest";
            Directory.CreateDirectory(dir);

            (ImageFormat, EncoderOptions?)[] tests = new[]
            {
                (ImageFormat.Jpeg, (EncoderOptions)new JpegEncoderOptions(1)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(5)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(10)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(20)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(30)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(40)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(50)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(60)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(70)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(80)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(90)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(95)),
                (ImageFormat.Jpeg, new JpegEncoderOptions(100)),

                (ImageFormat.Png, null),
                (ImageFormat.Tga, new TgaEncoderOptions(useRunLengthEncoding: true)),
                (ImageFormat.Tga, new TgaEncoderOptions(useRunLengthEncoding: false)),
                (ImageFormat.Bmp, null)
            };

            using (var image1 = CreateColorfulImage(256))
            {
                foreach (var (format, options) in tests)
                {
                    string extra = 
                        options is JpegEncoderOptions jpegOpts ? ("_" + jpegOpts.Quality) :
                        options is TgaEncoderOptions tgaOpts ? (tgaOpts.UseRunLengthEncoding ? "_rle" : "_raw") :
                        "";

                    string path = Path.Combine(dir, "write" + extra + format.Extension);
                    image1.Save(path, format, options);
                }
            }
            return;

            TestPng();
            return;

            var archive = new ZipArchive(File.OpenRead(DataZip), ZipArchiveMode.Read, false);

            //var stream = archive.GetEntry("png/32bit.png").Open();
            //var stream = archive.GetEntry("bmp/32bit.bmp").Open();
            //var stream = archive.GetEntry("bmp/24bit.bmp").Open();
            //var stream = archive.GetEntry("bmp/8bit.bmp").Open();

            var encoded = new MemoryStream(1024 * 1024 * 8);
            //using (var stream = new FileStream("big img.png", FileMode.Open))
            //using (var stream = new FileStream("smol img.png", FileMode.Open))
            //stream.CopyTo(encoded);

            //var encoded = new FileStream("big img.png", FileMode.Open);

            //var req = WebRequest.CreateHttp("https://cdn.discordapp.com/attachments/290820360335523840/551171768090492938/unknown.png");
            //var encoded = req.GetResponse().GetResponseStream();
            //encoded = RecyclableMemoryManager.Default.GetBufferedStream(encoded, false);

            int readRepeats = 1;
            int writeRepeats = 1;

            static void OnReadProgress(
                IImageDecoder decoder,
                double percentage,
                Rectangle? rectangle)
            {
                Console.WriteLine("Read: " + Math.Round(percentage * 100, 2) + "%");
            }

            var watch = new Stopwatch();
            Image image = null;
            for (int i = 0; i < readRepeats; i++)
            {
                encoded.Seek(0, SeekOrigin.Begin);

                image?.Dispose();
                watch.Start();
                image = Image.Load(encoded, onProgress: OnReadProgress);
                watch.Stop();

                if (i == 0)
                {
                    Console.WriteLine("Initial Read: " + Math.Round(watch.Elapsed.TotalMilliseconds, 3) + "ms");
                    Console.WriteLine(image.Width + "x" + image.Height + " # " + image.PixelType.Type);
                    watch.Reset();
                }
            }
            Console.WriteLine("Read Average: " +
                Math.Round(watch.Elapsed.TotalMilliseconds / (readRepeats == 1 ? 1 : readRepeats - 1), 3) + "ms");

            Thread.Sleep(500);

            watch.Reset();

            static void OnWriteProgress(
                IImageEncoder encoder,
                double percentage,
                Rectangle? rectangle)
            {
                //Console.WriteLine("Write: " + Math.Round(percentage * 100, 2) + "%");
            }

            var result = new MemoryStream(1024 * 1024 * 85);
            var writeFormat = ImageFormat.Png;
            watch.Restart();
            for (int i = 0; i < writeRepeats; i++)
            {
                result.Seek(0, SeekOrigin.Begin);

                watch.Start();
                image.Save(result, ImageFormat.Png, null, OnWriteProgress, default);
                watch.Stop();

                if (i == 0)
                {
                    Console.WriteLine("Initial Write: " + Math.Round(watch.Elapsed.TotalMilliseconds, 3) + "ms");
                    watch.Reset();
                }
            }

            using (var fs = new FileStream("recoded" + writeFormat.Extension, FileMode.Create))
            {
                result.Seek(0, SeekOrigin.Begin);
                result.CopyTo(fs);
            }
            Console.WriteLine("Write Average: " +
                Math.Round(watch.Elapsed.TotalMilliseconds / (writeRepeats == 1 ? 1 : writeRepeats - 1), 3) + "ms");

            Thread.Sleep(500);

            return;

            using (var resizeDst = Image<Color>.Create(image.Size * 2))
            {
                watch.Restart();

                //fixed (byte* src = image.GetPixelByteSpan())
                //fixed (byte* dst = resizeDst.GetPixelByteSpan())
                //{
                //    int code = StbImageResize2.stbir_resize_uint8(
                //        src, image.Width, image.Height, image.ByteStride,
                //        dst, resizeDst.Width, resizeDst.Height, resizeDst.ByteStride,
                //        num_channels: 4);
                //}
                //
                //resizeDst.GetPixelSpan().Fill(default);

                for (int i = 0; i < 1; i++)
                {
                    ImageResize.Resize(
                        image.GetPixelByteSpan(), image.Width, image.Height, image.ByteStride,
                        resizeDst.GetPixelByteSpan(), resizeDst.Width, resizeDst.Height, resizeDst.ByteStride,
                        numChannels: 4);
                }

                watch.Stop();

                Console.WriteLine("resized in " +
                    Math.Round(watch.Elapsed.TotalMilliseconds / (writeRepeats == 1 ? 1 : writeRepeats - 1), 3) + "ms");

                Thread.Sleep(500);

                watch.Restart();
                resizeDst.Save("resized" + writeFormat.Extension);
                watch.Stop();

                Console.WriteLine("Saved resized in " +
                    Math.Round(watch.Elapsed.TotalMilliseconds / (writeRepeats == 1 ? 1 : writeRepeats - 1), 3) + "ms");

            }

            image.Dispose();
            GC.Collect();

            Console.ReadKey();

            // terraria party: "https://preview.redd.it/2ws638caxh421.png?auto=webp&s=07157d5f791ebec998d2793ff384aa6f8c67a638"
            // nyoom dog: "https://cdn.discordapp.com/attachments/290820360335523840/551171768090492938/unknown.png"

            //string request = "https://cdn.discordapp.com/attachments/290820360335523840/551171768090492938/unknown.png";
            //var req = WebRequest.CreateHttp(request);
            //
            //Console.WriteLine("Requested " + request);
            //Console.WriteLine();
            //
            //using (var rep = req.GetResponse())
            //{
            //    Console.WriteLine("Got response");
            //
            //    using (var repStream = rep.GetResponseStream())
            //    using (var img1 = new Image(repStream, ImagePixelFormat.Rgb))
            //    {
            //        Console.Write("Writing... ");
            //        using (var fs = File.OpenWrite("im done netted.png"))
            //            img1.Save(fs, ImageSaveFormat.Png);
            //        Console.WriteLine("Done");
            //    }
            //}
            //Console.WriteLine();
            //
            //Console.WriteLine("Copying net response");
            //using (var img1 = new Image(File.OpenRead("im done netted.png"), ImagePixelFormat.Rgb))
            //{
            //    Console.Write("Writing... ");
            //    using (var fs = File.OpenWrite("im done.png"))
            //        img1.Save(fs, ImageSaveFormat.Png);
            //    Console.WriteLine("Done");
            //}
            //
            //Console.ReadKey();
            //
            //return;

            //var d = new SaveConfiguration(true, 0, RecyclableMemoryManager.Instance);
            //var nonD = new SaveConfiguration(false, 0, RecyclableMemoryManager.Instance);
            //var ms = new MemoryStream();
            //
            //TestEntry(ms, d, archive, "bmp/4bit.bmp");
            //TestEntry(ms, d, archive, "bmp/8bit.bmp");
            //TestEntry(ms, d, archive, "bmp/24bit.bmp");
            //TestEntry(ms, d, archive, "bmp/32bit.bmp");
            //
            //TestEntry(ms, d, archive, "jpg/quality_0.jpg");
            //TestEntry(ms, d, archive, "jpg/quality_25.jpg");
            //TestEntry(ms, d, archive, "jpg/quality_50.jpg");
            //TestEntry(ms, d, archive, "jpg/quality_75.jpg");
            //TestEntry(ms, d, archive, "jpg/quality_100.jpg");
            //
            //TestEntry(ms, d, archive, "png/32bit.png");
            //TestEntry(ms, d, archive, "png/24bit.png");
            //TestEntry(ms, d, archive, "png/8bit.png");
            //
            //TestEntry(ms, nonD, archive, "tga/32bit.tga");
            //TestEntry(ms, d, archive, "tga/32bit_rle.tga");
            //TestEntry(ms, nonD, archive, "tga/24bit.tga");
            //TestEntry(ms, d, archive, "tga/24bit_rle.tga");
            //
            //Console.WriteLine(RecyclableMemoryManager.Instance.SmallBlocksFree);

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

        static Image<Color> CreateColorfulImage(int size)
        {
            var imagee = new Image<Color>(new Size(size));
            var pixels = imagee.GetPixelSpan();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    pixels[y * size + x] = new Color(
                        x * 127 / (size - 1),
                        y * 127 / (size - 1),
                        127);
                }
            }
            return imagee;
        }

        //static void TestEntry(MemoryStream ms,
        //    SaveConfiguration config, ZipArchive archive, string name)
        //{
        //    var watch = new Stopwatch();
        //    int tries = 1; //3000;
        //
        //    try
        //    {
        //        var entry = archive.GetEntry(name);
        //        MemoryStream dataStream = new MemoryStream((int)entry.Length);
        //        using (var es = entry.Open())
        //            es.CopyTo(dataStream);
        //
        //        double infoReadTime = 0;
        //        double pointerReadTime = 0;
        //        double imageSaveTime = 0;
        //
        //        for (int i = 0; i < tries; i++)
        //        {
        //            dataStream.Position = 0;
        //            using (var img = new Image(dataStream, true))
        //            {
        //                watch.Restart();
        //                ImageInfo imageInfo = img.Info;
        //                watch.Stop();
        //                if (tries > 0)
        //                    infoReadTime += watch.Elapsed.TotalMilliseconds;
        //
        //                //Console.WriteLine(name + ": " + (img.LastGetInfoFailed ? "Failed to read info" : "Retrieved info successfully"));
        //
        //                //Console.WriteLine($"Loading ({imageInfo}) data...");
        //
        //                watch.Restart();
        //                IntPtr data = img.GetPointer();
        //                watch.Stop();
        //                if (tries > 0)
        //                    pointerReadTime += watch.Elapsed.TotalMilliseconds;
        //
        //                if (data == null)
        //                    Console.WriteLine("Data Pointer NULL: " + img.Errors);
        //                else
        //                {
        //                    //Console.WriteLine("Saving " + img.PointerLength + " bytes...");
        //
        //                    watch.Restart();
        //
        //                    ms.Position = 0;
        //                    ms.SetLength(0);
        //                    img.Save(ms, imageInfo.SourceFormat.ToSaveFormat(), config);
        //
        //                    watch.Stop();
        //                    if (tries > 0)
        //                        imageSaveTime += watch.Elapsed.TotalMilliseconds;
        //                }
        //            }
        //        }
        //
        //        FileInfo outputInfo = new FileInfo("testoutput/" + name);
        //        outputInfo.Directory.Create();
        //        using (var fs = new FileStream(outputInfo.FullName, FileMode.Create))
        //        {
        //            ms.Position = 0;
        //            ms.CopyTo(fs);
        //        }
        //
        //        Console.WriteLine();
        //
        //        Console.WriteLine(name);
        //        Console.WriteLine("Info Read Avg: " + Math.Round(infoReadTime / tries, 2) + "ms");
        //        Console.WriteLine("Pointer Read Avg: " + Math.Round(pointerReadTime / tries, 2) + "ms");
        //        Console.WriteLine("Saving Time Avg: " + Math.Round(imageSaveTime / tries, 2) + "ms");
        //    }
        //    catch (Exception exc)
        //    {
        //        Console.WriteLine(exc);
        //    }
        //
        //    //Console.WriteLine($"Memory Allocated (Arrays: {manager.AllocatedArrays}): " + manager.AllocatedBytes + " bytes");
        //    //Console.WriteLine($"Lifetime Allocated (Arrays: {manager.LifetimeAllocatedArrays}): " + manager.AllocatedBytes + " bytes");
        //    Console.WriteLine("----------------------------------------------------");
        //}


        static void TestBmp(ZipArchive archive)
        {
            Directory.CreateDirectory("test/bmp");

            using (var stream = archive.GetEntry("bmp/32bit.bmp").Open())
            using (var img = Image.Load(stream))
                img.Save("32bit.png");

            using (var stream = archive.GetEntry("bmp/24bit.bmp").Open())
            using (var img = Image.Load(stream))
                img.Save("24bit.png");

            using (var stream = archive.GetEntry("bmp/8bit.bmp").Open())
            using (var img = Image.Load(stream))
                img.Save("8bit.png");

            using (var stream = archive.GetEntry("bmp/4bit.bmp").Open())
            using (var img = Image.Load(stream))
                img.Save("4bit.png");
        }

        static void TestPng()
        {
            Directory.CreateDirectory("test/png/cgbi");

            //using (var archive = new ZipArchive(File.OpenRead("testdata.zip"), ZipArchiveMode.Read, false))
            //{
            //    var entri = archive.GetEntry("png/cgbi/prop.png");
            //    using (var ffff = entri.Open())
            //    using (var ihponeimg = Image.Load(ffff))
            //    {
            //        ihponeimg.Save("test/png/cgbi/prop.png");
            //    }
            //}

            using (var archive = new ZipArchive(File.OpenRead("PngSuite-2017jul19.zip"), ZipArchiveMode.Read, false))
            {
                Directory.CreateDirectory("wtf");

                //var testtt = "basi0g01.png";
                //
                //var endmee = new MemoryStream();
                //archive.GetEntry(testtt).Open().CopyTo(endmee);
                //
                //var original = new MemoryStream();
                //endmee.Position = 0;
                //endmee.CopyTo(original);
                //
                //OriginalLoad(endmee);
                //
                //endmee.Position = 0;
                //original.Position = 0;
                //for (int i = 0; i < endmee.Length; i++)
                //    if (endmee.ReadByte() != original.ReadByte())
                //        throw new Exception();

                foreach (var entry in archive.Entries)
                {
                    if (!entry.Name.EndsWith(".png"))
                        continue;

                    if (entry.Name[0] == 'x')
                        continue;

                    //if (entry.Name != "basi6a16.png")
                    //    continue;

                    Console.WriteLine(entry.Name);

                    using (var stream = entry.Open())
                    {
                        var ms = new MemoryStream();
                        stream.CopyTo(ms);

                        var sha = SHA256.Create();

                        try
                        {
                            Image img = null;

                            for (int i = 0; i < 1; i++)
                            {
                                img?.Dispose();

                                ms.Position = 0;
                                img = Image.Load(ms, VectorType.Get<Color>());

                                byte[] hash = sha.ComputeHash(((Image<Color>)img).GetPixelByteSpan().ToArray());
                                //Console.WriteLine("hash: " + BitConverter.ToString(hash));
                            }

                            string dstName = "wtf/" + entry.Name;
                            img.Save(dstName);

                            //for (int i = 0; i < 100; i++)
                            //{
                            //    File.Copy(dstName, "wtf/" + Guid.NewGuid() + ".png");
                            //}

                            img.Dispose();

                            //fixed (byte* omg = ms.GetBuffer())
                            //{
                            //    var ccc = new StbImageSharp.StbImage.stbi__context();
                            //    ccc.img_buffer = omg;
                            //    ccc.img_buffer_end = omg + ms.Length;
                            //
                            //    int w;
                            //    int h;
                            //    int comp;
                            //    StbImageSharp.StbImage.stbi__result_info ri;
                            //    void* pp = StbImageSharp.StbImage.stbi__png_load(ccc, &w, &h, &comp, 0, &ri);
                            //
                            //    var vec = StbImageDecoderState.GetVectorType(
                            //        new ImageRead.ReadState() { OutComponents = comp, OutDepth = ri.bits_per_channel });
                            //    var uu = new UnmanagedMemory<byte>(w * h * comp * ri.bits_per_channel / 8);
                            //    new Span<byte>(pp, w * h * comp * ri.bits_per_channel / 8).CopyTo(uu.Span);
                            //    Image.WrapMemory(vec, uu, new Size(w, h)).Save("wtf/" + entry.Name[0..^4] + "_.png");
                            //}
                        }
                        //catch (NotImplementedException ex)
                        //{
                        //    //Console.WriteLine("what " + ex);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("ex: " + ex);
                        //}
                        finally
                        {

                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
        }

        //static unsafe void OriginalLoad(MemoryStream endmee)
        //{
        //    fixed (byte* omg = endmee.GetBuffer())
        //    {
        //        var ccc = new StbImageSharp.StbImage.stbi__context();
        //        ccc.img_buffer = omg;
        //        ccc.img_buffer_end = omg + endmee.Length;
        //
        //        int w;
        //        int h;
        //        int comp;
        //        StbImageSharp.StbImage.stbi__result_info ri;
        //        void* pp = StbImageSharp.StbImage.stbi__png_load(ccc, &w, &h, &comp, 0, &ri);
        //
        //        var vec = StbImageDecoderState.GetVectorType(
        //            new ImageRead.ReadState() { OutComponents = comp, OutDepth = ri.bits_per_channel });
        //        var uu = new UnmanagedMemory<byte>(w * h * comp * ri.bits_per_channel / 8);
        //        new Span<byte>(pp, w * h * comp * ri.bits_per_channel / 8).CopyTo(uu.Span);
        //        var mg = Image.WrapMemory(vec, uu, new Size(w, h));
        //        mg.Save("wtf/_OMG.png");
        //    }
        //}
    }
}

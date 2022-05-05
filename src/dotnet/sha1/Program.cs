#define MULTI_THREAD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace sha1
    {
    internal class Program
        {
        private const Int32 NumberOfThreads = 32;
        private static readonly Object o = new Object();
        private static void UpdateTitle(String Label, Int32 TotalCount, Int32 Count)
            {
            lock(o)
                {
                Console.Title = $"{Label}:{TotalCount}:{Count}:{((Single)Count/TotalCount)*100:F2}%";
                }
            }

        private static String Calculate(String filename)
            {
            using (var engine = new SHA1Cng())
                {
                return Calculate(engine, filename);
                }
            }

        private static String Calculate(HashAlgorithm engine, String filename)
            {
            return String.Join(String.Empty, engine.ComputeHash(
                new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)).
                Select(i => i.ToString("x2")));
            }

        private static void Main(string[] args)
            {
            ThreadPool.GetMinThreads(out var prevThreads, out var prevPorts);
            ThreadPool.SetMinThreads(32, prevPorts);
            var InputFolder = String.Empty;
            for (var i = 0; i < args.Length; ++i) {
                if (args[i].StartsWith("folder:"))
                    {
                    InputFolder = args[i].Substring(7);
                    }
                }
            var CurrentFolder = Directory.GetCurrentDirectory();
            var Folder = Path.Combine(CurrentFolder, InputFolder);
            if (!File.Exists(".sha1")) {
                Console.Title = "Scanning...";
                var Count = Directory.EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories).Count();
                var Files = new Dictionary<String,String>();
                var FileIndex = 0;
                #if !MULTI_THREAD
                var Engine = SHA1.Create();
                foreach (var file in Directory.EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories)) {
                    if (file.StartsWith(Folder))
                        {
                        Files[file.Substring(Folder.Length + 1)] = Calculate(Engine, file);
                        }
                    else
                        {
                        Files[file] = Calculate(Engine, file);
                        }
                    UpdateTitle(Count,Interlocked.Increment(ref FileIndex));
                    }
                #else
                Directory.EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories).
                        AsParallel().
                        WithDegreeOfParallelism(NumberOfThreads).
                        ForAll(file =>
                        {
                        var hashvalue = Calculate(file);
                        var filename = file.StartsWith(Folder)
                            ? file.Substring(Folder.Length + 1)
                            : file;
                        lock(o)
                            {
                            UpdateTitle("Calculating", Count,Interlocked.Increment(ref FileIndex));
                            Files[filename] = hashvalue;
                            }
                        });
                #endif
                Console.Title = "Writing...";
                FileIndex = 0;
                using (var writer = new StreamWriter(File.OpenWrite(".sha1"))) {
                    foreach (var file in Files.OrderBy(i => i.Key)) {
                        writer.WriteLine($"{file.Value} {file.Key}");
                        UpdateTitle("Writing", Count,Interlocked.Increment(ref FileIndex));
                        }
                    }
                File.SetAttributes(".sha1", FileAttributes.Hidden);
                }
            else
                {
                var Files = File.ReadAllLines(".sha1");
                var FileIndex = 0;
                var Count = Files.Length;
                #if MULTI_THREAD
                Files.AsParallel().
                WithDegreeOfParallelism(NumberOfThreads).
                ForAll(file =>
                    {
                    if (String.IsNullOrWhiteSpace(file)) { return; }
                    var filename   = file.Substring(41);
                    if (File.Exists(Path.Combine(CurrentFolder,filename))) {
                        var hashvalueX = file.Substring(0, 40);
                        var hashvalueY = Calculate(Path.Combine(CurrentFolder,filename));
                        lock(o)
                            {
                            UpdateTitle("Checking", Count,Interlocked.Increment(ref FileIndex));
                            if (!String.Equals(hashvalueX,hashvalueY,StringComparison.OrdinalIgnoreCase)) {
                                Console.Error.WriteLine($"{{error}}:invalid hash value:{{{filename}}}");
                                }
                            }
                        }
                    else
                        {
                        lock(o)
                            {
                            UpdateTitle("Checking", Count,Interlocked.Increment(ref FileIndex));
                            Console.Error.WriteLine($"{{error}}:file not found:{{{filename}}}");
                            }
                        }
                    });
                #else
                var Engine = SHA1.Create();
                foreach (var file in Files) {
                    if (String.IsNullOrWhiteSpace(file)) { continue; }
                    var filename   = file.Substring(41);
                    var hashvalueX = file.Substring(0, 40);
                    var hashvalueY = Calculate(Engine, Path.Combine(CurrentFolder,filename));
                    UpdateTitle(Count,Interlocked.Increment(ref FileIndex));
                    if (!String.Equals(hashvalueX,hashvalueY,StringComparison.OrdinalIgnoreCase)) {
                        Console.Error.WriteLine($"{{error}}:invalid hash value:{{{filename}}}");
                        }
                    }
                #endif
                }
            }

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetACP();
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetOEMCP();
        }
    }

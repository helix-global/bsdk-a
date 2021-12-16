using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
using BinaryStudio.PlatformComponents.Win32;
using Newtonsoft.Json;
using Operations;
using Options;
#if USE_WINDOWS_API_CODE_PACK
using Microsoft.WindowsAPICodePack.Taskbar;
#endif
using Formatting = Newtonsoft.Json.Formatting;

namespace Kit
    {
    public class Program
        {
        #if USE_WINDOWS_API_CODE_PACK
        internal static TaskbarManager taskbar = TaskbarManager.Instance;
        #endif

        [Flags]
        internal enum Flags
            {
            None              = 0x00000000,
            List              = 0x00000001,
            Xml               = 0x10000000,
            Json              = 0x20000000,
            }

        #region M:Validate(Boolean)
        protected static void Validate(Int32 status) {
            Exception e;
            var i = status;
            if ((i >= 0xFFFF) || (i < 0))
                {
                e = new COMException($"[HRESULT:{(HRESULT)i}]", i);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            else
                {
                e = new COMException($"[{(Win32ErrorCode)i}]", i);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            }
        #endregion
        internal static void InternalLoad(String[] args)
            {
            try
                {
                using (new TraceScope()) {
                    Boolean randomgeneration;
                    String[] inputfilename;
                    String outputfilename;
                    Boolean dumpcodes;
                    Int64 sizevalue;
                    return;
                    for (var i = 0; i < args.Length; ++i) {
                        if (args[i][0] == '-') {
                                  if (args[i] == "-dumpcodes")     { dumpcodes = true; }
                            else if (args[i].StartsWith("-size:")) {
                                var r = Regex.Match(args[i].Substring(6).ToUpper(), @"^(\p{N}+)([KMGКМГ])?");
                                if (r.Success) {
                                    sizevalue = Int64.Parse(r.Groups[1].Value);
                                    switch (r.Groups[2].Value) {
                                        case "K":
                                        case "К":
                                            {
                                            sizevalue *= 1024;
                                            }
                                            break;
                                        case "M":
                                        case "М":
                                            {
                                            sizevalue *= 1024*1024;
                                            }
                                            break;
                                        case "G":
                                        case "Г":
                                            {
                                            sizevalue *= 1024*1024*1024;
                                            }
                                            break;
                                        }
                                    }
                                }
                            else if (args[i] == "-random")  { randomgeneration = true; }
                            }
                        }
                    if (dumpcodes) {
                        using (var scope = new MetadataScope()) {
                            if (scope.LoadObject(inputfilename[0]) is PortableExecutableSource o) {
                                using (var writer = !String.IsNullOrWhiteSpace(outputfilename)
                                    ? new StreamWriter(File.Create(outputfilename))
                                    : null)
                                    {
                                    var messagetable = o.Resources.FirstOrDefault(i => i.ToString() == "RT_MESSAGETABLE");
                                    if (messagetable != null) {
                                        if (messagetable.Resources[0].Resources[0] is IDictionary<UInt32,String> values) {
                                            foreach (var value in values) {
                                                if (writer != null)
                                                    {
                                                    writer.WriteLine($"{value.Key}:{value.Value}");
                                                    }
                                                else
                                                    {
                                                    Console.WriteLine($"{value.Key}:{value.Value}");
                                                    }
                                                }
                                            }
                                        }
                                    if (writer != null)
                                        {
                                        writer.Write("------------------- RT_STRING ------------------------");
                                        }
                                    var strings = o.Resources.FirstOrDefault(i => i.ToString() == "RT_STRING");
                                    if (strings.Resources[0] is IDictionary<UInt32,String> src) {
                                        foreach (var value in src) {
                                            if (writer != null)
                                                {
                                                writer.WriteLine($"{value.Key}:{value.Value}");
                                                }
                                            else
                                                {
                                                Console.WriteLine($"{value.Key}:{value.Value}");
                                                }
                                            }
                                        }
                                    if (writer != null)
                                        {
                                        writer.Flush();
                                        }
                                    }
                                }
                            }
                        }
                    else if (randomgeneration)
                        {
                        if (String.IsNullOrWhiteSpace(outputfilename)) { throw new ArgumentOutOfRangeException("outputfilename"); }
                        if (sizevalue == -1) { throw new ArgumentOutOfRangeException("sizevalue"); }
                        var random = new Random();
                        var sz = sizevalue;
                        var buffersize = 8*1024*1024;
                        var buffer = new Byte[buffersize];
                        var i = 0L;
                        using (var target = File.Create(outputfilename)) {
                            for (;;) {
                                Thread.Yield();
                                random.NextBytes(buffer);
                                var count = (Int32)Math.Max(0, Math.Min(buffersize, sz - i));
                                if (count == 0) { break; }
                                target.Write(buffer, 0, count);
                                i = i + buffersize;
                                }
                            }
                        }
                    }
                }
            catch(OptionRequiredException exception)
                {
                Console.WriteLine("error: required option missing");
                exception.Descriptor.Usage(Console.Error);
                Console.WriteLine();
                }
            catch(Exception x)
                {
                (new ExceptionFormatter()).Write(Console.Error, x);
                }
            }

        //private static void OnPercentageChanged(Object sender, PercentageChangedEventArgs e)
        //    {
        //    switch (e.State)
        //        {
        //        case ProgressState.Indeterminate:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, WindowHandle);
        //            taskbar.SetProgressValue(0, 0, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        case ProgressState.None:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, WindowHandle);
        //            taskbar.SetProgressValue(0, 0, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        case ProgressState.Normal:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.Normal, WindowHandle);
        //            taskbar.SetProgressValue((Int32)(e.Value*100), 100, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        }
        //    }

        [MTAThread]
        internal static void Main(String[] args) {
            using (var client = new LocalClient()) {
                Environment.Exit(client.Main(args));
                }
            var options = Operation.Parse(args);
            if (HasOption(options, typeof(TraceOption))) {
                using (new ConsoleColorScope(ConsoleColor.DarkGray))
                    {
                    TraceScope.WriteTo(Console.Out);
                    //TraceManager.Instance.Write(Console.Out);
                    }
                }
            }

        #region M:GetFolderAbsolutePath(String):String
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetFullPathNameW")] private static extern Int32 GetFullPathName(String filename, Int32 bufferlength, [MarshalAs(UnmanagedType.LPArray)] Byte[] buffer, IntPtr filepart);
        private static String GetFolderAbsolutePath(String folder)
            {
            var buffer = new Byte[32767*2];
            var r = GetFullPathName(folder, 32767, buffer, IntPtr.Zero);
            return Encoding.Unicode.GetString(buffer, 0, r*2);
            }
        #endregion
        #region M:ToStringArray(String):String[]
        private static String[] ToStringArray(String source)
            {
            if (source == null) { return new String[0]; }
            return source.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(Replace).ToArray();
            }
        #endregion
        #region M:Replace(String):String
        private static String Replace(String source) {
            if (source == null) { return null; }
            return source.ToUpper().Replace(" ", "");
            }
        #endregion

        [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();

        public static IntPtr WindowHandle { get {
            var r = GetConsoleWindow();
            //Debug.Print($"{(Int64)r:X}");
            return r;
            }}

        #region M:JsonSerialize(Object,TextWriter)
        private static void JsonSerialize(Object value, TextWriter output) {
            //Console.WriteLine(value.ToString());
            using (var writer = new JsonTextWriter(output){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    })
                {
                var serializer = new JsonSerializer();
                if (value is IJsonSerializable o)
                    {
                    o.WriteJson(writer, serializer);
                    }
                else
                    {
                    serializer.Serialize(writer, value);
                    }
                writer.Flush();
                }
            }
        #endregion
        #region M:XmlSerialize(Object,TextWriter)
        private static void XmlSerialize(Object value, TextWriter output)
            {
            if (value is IXmlSerializable serializable) {
                using (var writer = XmlWriter.Create(output, new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineOnAttributes = true
                    }))
                    {
                    serializable.WriteXml(writer);
                    }
                }
            }
        #endregion

        private static Boolean HasOption(IList<OperationOption> source, Type type) {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return source.Any(i => i.GetType() == type);
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)] private static extern IntPtr LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern Boolean FreeLibrary(IntPtr module);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, String procedure);
        [DllImport("kernel32.dll")] private static extern Int32 GetProcessId(IntPtr handle);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
        }
    }

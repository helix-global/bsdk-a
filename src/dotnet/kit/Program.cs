﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using kit;
using log4net;
using Newtonsoft.Json;
using Options;
using srv;
#if USE_WINDOWS_API_CODE_PACK
using Microsoft.WindowsAPICodePack.Taskbar;
#endif
using Formatting = Newtonsoft.Json.Formatting;
using Process = System.Diagnostics.Process;

namespace Kit
    {
    public class Program
        {
        private static readonly ILogger logger = new ClientLogger(LogManager.GetLogger(nameof(Program)));
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
        protected static void Validate(Boolean status) {
            if (!status) {
                throw PlatformContext.GetExceptionForHR(Marshal.GetLastWin32Error());
                }
            }

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

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        static void M1()
            {

            }

        private static void F1()
            {
            try
                {
                try
                    {
                    F2();
                    }
                catch (Exception e)
                    {
                    e.Data["Item1"] = "Item1";
                    throw new InvalidDataException(e.Message, e);
                    }
                }
            catch (Exception e)
                {
                e.Data["Item3"] = "Item3";
                throw new ArgumentException(e.Message, e);
                }
            }

        private static void F2()
            {
            try
                {
                F3();
                }
            catch (Exception e)
                {
                e.Data["Item2"] = "Item2";
                throw new InvalidOperationException("Message2", e);
                }
            }
        private static void F3()
            {
            F4();
            }
        private static void F4() {
            var x = new List<Exception>();
            for (var i = 0; i < 2; i++) {
                try
                    {
                    //F5();
                    }
                catch (Exception e)
                    {
                    x.Add(e);
                    }
                try
                    {
                    F8();
                    }
                catch (Exception e)
                    {
                    x.Add(e);
                    }
                try
                    {
                    F8();
                    }
                catch (Exception e)
                    {
                    x.Add(e);
                    }
                try
                    {
                    F5();
                    }
                catch (Exception e)
                    {
                    x.Add(e);
                    }
                }
            throw new AggregateException(x);
            }

        private static void F5()
            {
            try
                {
                F6();
                }
            catch(Exception e)
                {
                e.Data["Item5"] = "Item5";
                throw;
                }
            }

        private static void F8()
            {
            try
                {
                F7();
                }
            catch(Exception e)
                {
                throw new Exception(e.Message , e);
                }
            }

        private static void F6()
            {
            throw new Exception("Message6");
            }

        private static void F7()
            {
            throw new Exception("Message7");
            }

        [MTAThread]
        internal static void Main(String[] args) {
            var color = Console.ForegroundColor;
            try
                {
                if (PlatformContext.IsParentProcess("kit.exe")) {
                    FreeConsole();
                    Validate(AttachConsole(-1));
                    }

                Int32 exitcode;
                using (var client = PlatformContext.IsRunningUnderServiceControl
                        ? (ILocalClient)(new LocalService())
                        : (ILocalClient )(new LocalClient()))
                    {
                    Console.CancelKeyPress += client.OnCancelKeyPress;
                    exitcode = client.Main(args);
                    Console.CancelKeyPress -= client.OnCancelKeyPress;
                    }
                Environment.ExitCode = exitcode;
                }
            catch (ControlBreakException)
                {
                Console.WriteLine("[Ctrl+Break]");
                Environment.ExitCode = -1;
                }
            catch (Exception e)
                {
                Console.WriteLine(e);
                logger.Log(LogLevel.Error, e);
                Environment.ExitCode = -1;
                }
            finally
                {
                Console.ForegroundColor = color;
                }
            }

        private static String JsonSerialize(Object o) {
            var r = new StringBuilder();
            using (var output = new StringWriter(r)) {
                using (var writer = new JsonTextWriter(output){
                        Formatting = Formatting.Indented,
                        Indentation = 2,
                        IndentChar = ' '
                        })
                    {
                    var serializer = new JsonSerializer();
                    if (o is IJsonSerializable serializable)
                        {
                        serializable.WriteJson(writer, serializer);
                        }
                    else
                        {
                        serializer.Serialize(writer, o);
                        }
                    writer.Flush();
                    }
                }
            return r.ToString();
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
        [DllImport("kernel32.dll")] private static extern Boolean AttachConsole(Int32 process);
        [DllImport("kernel32.dll")] private static extern Boolean FreeConsole();
        [DllImport("kernel32.dll")] private static extern Boolean AllocConsole();
        [DllImport("ntdll.dll", CharSet = CharSet.Auto)] private static extern unsafe UInt32 NtQueryInformationProcess(IntPtr process, Int32 iclass, void* pi, UInt32 pisz, out UInt32 r);

        private const Int32 ProcessBasicInformation = 0;
        private const Int32 ProcessDebugPort = 7;
        private const Int32 ProcessWow64Information = 26;
        private const Int32 ProcessImageFileName = 27;
        private const Int32 ProcessBreakOnTermination = 29;
        private const Int32 PROCESS_QUERY_INFORMATION          = 0x0400;
        }
    }

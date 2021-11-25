using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.PlatformComponents.Unix
    {
    public sealed class UnixSharedObject : SharedObject
        {
        public override String FileName { get; }
        public UnixSharedObject(String filename)
            : base(filename)
            {
            LDConfigEnsure();
            FileName = filename.Trim();
            if (!IsRelativeOrAbsolute(FileName)) {
                foreach (var i in LDLibraryPath.Select(i => Path.Combine(i, filename))) {
                    if (File.Exists(i)) {
                        FileName = i;
                        break;
                        }
                    }
                if (cache.TryGetValue(filename, out String fullpath)) {
                    FileName = fullpath;
                    }
                else
                    {
                    if (!filename.StartsWith("lib")) {
                        if (!Path.HasExtension(filename)) {
                            if (cache.TryGetValue($"lib{filename}.so", out fullpath)) {
                                FileName = fullpath;
                                }
                            }
                        }
                    }
                }
            }

        [DllImport("dl", CharSet = CharSet.Ansi, EntryPoint = "dlopen")]  private static extern IntPtr DLOpen([MarshalAs(UnmanagedType.LPStr)] String filename, Int32 flags);
        [DllImport("dl", CharSet = CharSet.Ansi, EntryPoint = "dlsym")]   private static extern IntPtr DLSym(SafeHandleZeroOrMinusOneIsInvalid module, String name);
        [DllImport("dl", CharSet = CharSet.Ansi, EntryPoint = "dlerror")] [return: MarshalAs(UnmanagedType.LPStr)] private static extern String DLError();
        [DllImport("c",  CharSet = CharSet.Ansi, EntryPoint = "getenv")]  private static extern IntPtr GetEnv(String name);

        private const Int32 RTLD_LAZY         = 0x00001;
        private const Int32 RTLD_NOW          = 0x00002;
        private const Int32 RTLD_BINDING_MASK = 0x00003;
        private const Int32 RTLD_NOLOAD       = 0x00004;
        private const Int32 RTLD_DEEPBIND     = 0x00008;
        private const Int32 RTLD_GLOBAL       = 0x00100;
        private const Int32 RTLD_LOCAL        = 0x00000;

        #region M:IsRelativeOrAbsolute(String):Boolean
        private static Boolean IsRelativeOrAbsolute(String filename) {
            if (filename.StartsWith("./")) { return true; }
            if (filename.StartsWith("/"))  { return true; }
            return false;
            }
        #endregion

        private class LDConfigItem
            {
            public String FileName { get; }
            public String FullPath { get; }
            public LDConfigItem(String source) {
                var regex = new Regex(@"(.+)\s+[(](.+)[)]\s+=>\s+(.+)");
                var match = regex.Match(source);
                if (match.Success)
                    {
                    FileName = match.Groups[1].Value;
                    FullPath = match.Groups[3].Value;
                    Platform = match.Groups[2].Value.Split(new Char[]{','}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToArray();
                    }
                }

            public Boolean IsValid(String platform)
                {
                if (Platform == null) { return false; }
                if (String.IsNullOrEmpty(platform)) { return true; }
                return Platform.Contains(platform);
                }

            public String[] Platform { get; }

            /// <summary>Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</summary>
            /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
            public override String ToString()
                {
                return $"{FileName}:[{String.Join(";", Platform)}]:{FullPath}";
                }
            }

        private static Dictionary<String, String> cache;
        private static void LDConfigEnsure()
            {
            if (cache != null) { return; }
            var r = new List<LDConfigItem>();
            var process = new Process {
                StartInfo = new ProcessStartInfo
                    {
                    FileName = "/sbin/ldconfig",
                    Arguments = "-p",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                    }
                };
            process.Start();
            for (;;)
                {
                var line = process.StandardOutput.ReadLine();
                if (line == null) { break; }
                var i = new LDConfigItem(line.Trim());
                if (i.IsValid(String.Empty))
                    {
                    #if DEBUG
                    Debug.Print(i.FullPath);
                    #endif
                    r.Add(i);
                    }
                }
            process.WaitForExit();
            cache = new Dictionary<String, String>();
            foreach (var i in r) {
                if (i.IsValid("x86-64"))
                    {
                    cache[i.FileName] = i.FullPath;
                    }
                }
            }

        #region M:GetPathEnvironment(String):IEnumerable<String>
        private static IEnumerable<String> GetPathEnvironment(String e)
            {
            var r = GetEnv(e);
            return (r != IntPtr.Zero)
                ? Marshal.PtrToStringAnsi(r).Split(new Char[]{ ':' }, StringSplitOptions.RemoveEmptyEntries)
                : new String[0];
            }
        #endregion
        #region P:LDLibraryPath:IEnumerable<String>
        private static IEnumerable<String> LDLibraryPath { get {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null) {
                if (!String.IsNullOrEmpty(assembly.Location)) { yield return Path.GetDirectoryName(assembly.Location); }
                }
            yield return Directory.GetCurrentDirectory();
            foreach (var i in GetPathEnvironment("LD_LIBRARY_PATH")) { yield return i; }
            #if NET35
            yield return (IntPtr.Size == 8)
                ? "/lib64"
                : "/lib";
            #else
            yield return Environment.Is64BitProcess
                ? "/lib64"
                : "/lib";
            #endif
            yield return "/usr/lib";
            }}
        #endregion
        #region M:EnsureOverride:SafeHandleZeroOrMinusOneIsInvalid
        protected override SafeHandleZeroOrMinusOneIsInvalid EnsureOverride()
            {
            DLError();
            var r = DLOpen(FileName, RTLD_NOW);
            if (r == IntPtr.Zero) {
                throw new InvalidOperationException(DLError());
                }
            return new UnixSharedObjectHandle(r);
            }
        #endregion
        #region M:Get(String):IntPtr
        public override IntPtr Get(String methodname)
            {
            if (methodname == null) { throw new ArgumentNullException(nameof(methodname)); }
            return DLSym(Handle, methodname);
            }
        #endregion
        }
    }
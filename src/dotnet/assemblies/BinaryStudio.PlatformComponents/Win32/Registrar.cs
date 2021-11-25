using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public static class Registrar
        {
        public static void StringRegister(String script)
            {
            EnsureCore();
            //((DAtlStringRegister)FAtlStringRegister)(script);
            }

        public static void StringUnregister(String script)
            {
            EnsureCore();
            //((DAtlStringUnregister)FAtlStringUnregister)(script);
            }

        #region M:Validate(HRESULT)
        private static void Validate(HRESULT hr) {
            if (hr != HRESULT.S_OK) {
                throw new COMException($"{FormatMessage((Int32)hr)} [HRESULT:{hr}]");
                }
            }
        #endregion
        #region M:FormatMessage(Int32):String
        private static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    return (new String((Char*)r)).Trim();
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);

        private static void EnsureCore()
            {
        //    if (so == null) {
        //        var executingassembly = Assembly.GetExecutingAssembly();
        //        var location = executingassembly.Location;
        //        if (String.IsNullOrEmpty(location))
        //            {
        //            location = AppDomain.CurrentDomain.BaseDirectory;
        //            }
        //        else
        //            {
        //            location = Path.GetDirectoryName(location);
        //            }
        //        #if DEBUG
        //        so = new SharedObject(Path.Combine(location, (IntPtr.Size == 8)
        //            ? "reghlp64d.dll"
        //            : "reghlp32d.dll"));
        //        #else
        //        so = new SharedObject(Path.Combine(location, (IntPtr.Size == 8)
        //            ? "reghlp64.dll"
        //            : "reghlp32.dll"));
        //        #endif
        //        FAtlStringRegister = new SharedMethod<DAtlStringRegister>(so, "AtlStringRegister");
        //        FAtlStringUnregister = new SharedMethod<DAtlStringUnregister>(so, "AtlStringUnregister");
        //        }
            }

        //private static SharedObject so;
        //[UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DAtlStringRegister([MarshalAs(UnmanagedType.BStr)] String script);
        //[UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DAtlStringUnregister([MarshalAs(UnmanagedType.BStr)] String script);
        //private static SharedMethod<DAtlStringRegister> FAtlStringRegister;
        //private static SharedMethod<DAtlStringUnregister> FAtlStringUnregister;
        }
    }
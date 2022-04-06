using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class NTStatusException : Exception
        {
        public NTSTATUS Status { get; }
        public override String Message { get; }

        public NTStatusException(NTSTATUS status)
            {
            Status = status;
            #if DEBUG
            Message = $"{FormatMessage((Int32)status)?.TrimEnd('\n', '\r')} [{status}]";
            #else
            Message = FormatMessage((Int32)status)?.TrimEnd('\n', '\r');
            #endif
            }

        static NTStatusException()
            {
            LibraryHandle = LoadLibrary("ntdll.dll");
            }

        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, int nSize, IntPtr[] arguments);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr LoadLibrary(String filename);
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        #region M:FormatMessage(Int32):String
        internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_HMODULE, LibraryHandle, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    return String.Join(" ", (new String((char*)r)).Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries));
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion
        private const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private const UInt32 FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
        private const UInt32 LANG_NEUTRAL                   = 0x00;
        private const UInt32 SUBLANG_DEFAULT                = 0x01;
        private static IntPtr LibraryHandle;
        }
    }
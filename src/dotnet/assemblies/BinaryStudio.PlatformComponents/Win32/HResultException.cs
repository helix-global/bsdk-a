using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class HResultException : COMException
        {
        public HResultException(Int32 code)
            : base(FormatMessage(code), code)
            {
            }

        #region M:FormatMessage(Int32):String
        protected internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    var o = (new String((Char*)r)).Trim();
                    if ((source >= 0xFFFF) || (source < 0))
                        {
                        return $"{o} {{HRESULT:{(HRESULT)source}}}";
                        }
                    else
                        {
                        return $"{o} {{Win32ErrorCode:{(Win32ErrorCode)source}}}";
                        }
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
        private const UInt32 LANG_NEUTRAL                   = 0x00;
        private const UInt32 SUBLANG_DEFAULT                = 0x01;

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);

        public static Exception GetExceptionForHR(Int32 i) {
            if ((i > 0xFFFF) || (i < 0)) {
                switch ((HRESULT)i) {
                    }
                }
            else
                {
                switch ((Win32ErrorCode)i) {
                    case Win32ErrorCode.ERROR_ACCESS_DENIED: return new UnauthorizedAccessException(FormatMessage(i));
                    }
                }
            return new HResultException(i);
            }
        }
    }
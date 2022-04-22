using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    public class HResultException : COMException
        {
        public HResultException(Int32 code, CultureInfo culture)
            :base(FormatMessage(code,culture), code)
            {
            }

        public HResultException(HRESULT code, CultureInfo culture)
            :this((Int32)code, culture)
            {
            }

        public HResultException(HRESULT code)
            :this(code, null)
            {
            }

        public HResultException(Int32 code)
            :this(code, null)
            {
            }

        #region M:FormatMessage(Int32):String
        protected internal static unsafe String FormatMessage(Int32 source, CultureInfo culture = null) {
            void* r;
            var LangId = ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL));
            if (culture != null) {
                LangId = unchecked((UInt32)(culture.LCID));
                }

            try
                {
                if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                    LangId, &r, 0, IntPtr.Zero)) {
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
                }
            catch
                {
                if ((source >= 0xFFFF) || (source < 0))
                    {
                    return $"{{HRESULT:{(HRESULT)source}}}";
                    }
                else
                    {
                    return $"{{Win32ErrorCode:{(Win32ErrorCode)source}}}";
                    }
                }
            return NTStatusException.FormatMessage(source);
            }
        #endregion

        private const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private const UInt32 LANG_NEUTRAL                   = 0x00;
        private const UInt32 SUBLANG_DEFAULT                = 0x01;
        private const Int32  FACILITY_NT_BIT                = 0x10000000;

        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr arguments);

        public static Exception GetExceptionForHR(Int32 i) { return GetExceptionForHR(i, null); }
        public static Exception GetExceptionForHR(Int32 i, CultureInfo culture) {
            if ((i > 0xFFFF) || (i < 0)) {
                switch ((HRESULT)i) {
                    }
                }
            else
                {
                switch ((Win32ErrorCode)i) {
                    case Win32ErrorCode.ERROR_ACCESS_DENIED: return new UnauthorizedAccessException(FormatMessage(i, culture));
                    }
                }
            return new HResultException(i, culture);
            }
        }
    }
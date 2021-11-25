using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
    #endif
    public struct _CRYPT_KEY_PROV_PARAM
        {
        public UInt32 dwParam;
        [ComConversionLoss]
        public IntPtr pbData;
        public UInt32 cbData;
        public UInt32 dwFlags;
        }
    }

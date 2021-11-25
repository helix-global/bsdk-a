using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields")]
    #endif
    public struct __MIDL___MIDL_itf_capicom_0001_0064_0024
        {
        [FieldOffset(0)] public UInt32 hCryptProv;
        [FieldOffset(0)]
        public UInt32 hNCryptKey;
        }
    }

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "<Pending>")]
    #endif
    public struct _CERT_KEY_CONTEXT
        {
        public UInt32 cbSize;
        public __MIDL___MIDL_itf_capicom_0001_0064_0024 __MIDL____MIDL_itf_capicom_0001_00640106;
        public UInt32 dwKeySpec;
        }
    }

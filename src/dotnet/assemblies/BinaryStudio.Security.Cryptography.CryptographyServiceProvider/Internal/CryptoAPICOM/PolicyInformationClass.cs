using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("OID"), ClassInterface(ClassInterfaceType.None), Guid("0AAF88F4-1C22-4F65-A0E3-289D97DCE994")]
    [ComImport]
    public class PolicyInformationClass : IPolicyInformation
        {
        [DispId(0)]
        public virtual extern IOID OID
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(1)]
        public virtual extern IQualifiers Qualifiers
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }
        }
    }

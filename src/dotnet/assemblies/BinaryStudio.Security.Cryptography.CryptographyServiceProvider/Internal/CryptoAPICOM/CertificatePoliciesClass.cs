using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("988583C2-00C7-4D22-9241-E810E35EED1B")]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
    #endif
    public class CertificatePoliciesClass : ICertificatePolicies
        {
        [DispId(0)]
        public virtual extern Object this[[In] Int32 Index]
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            }

        [DispId(1)]
        public virtual extern Int32 Count
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(-4), TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        public virtual extern IEnumerator GetEnumerator();
        }
    }

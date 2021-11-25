using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("F1800663-5BFC-4D1A-8D44-56CE02DDA34F")]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
    #endif
    public class EKUsClass : IEKUs
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

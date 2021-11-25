using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("6ADC653E-D5B9-422A-991A-A2B0119CEDAC")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
    #endif
    public interface IAttributes : IEnumerable
        {
        [DispId(0)]
        Object this[[In] Int32 Index]
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            }

        [DispId(1)]
        Int32 Count
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        //[DispId(-4), TypeLibFunc(1)]
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        //IEnumerator GetEnumerator();

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Add([MarshalAs(UnmanagedType.Interface)] [In] Attribute pVal);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([In] Int32 Index);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Clear();
        }
    }

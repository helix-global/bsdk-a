using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("96A1B8B0-8F9A-436A-84DE-E23CD6818DA5")]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
    #endif
    public class RecipientsClass : IRecipients
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

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Remove([In] Int32 Index);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Clear();
        }
    }

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("4782FBB8-ECD5-11D0-BFF0-00AA003DEF5B")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    internal class REICoClassStringCollection : REICOMStringCollection
        {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public extern RoseStringCollectionClass();

        [DispId(50)]
        public virtual extern Int16 Count { [DispId(50), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(50), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(51)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String GetAt(Int16 id);
        }
    }

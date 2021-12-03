using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BA376ECD-A44E-11D0-BC02-00A024C67143")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    public class REICoClassMessageCollection : IREICOMMessageCollection, REICOMMessageCollection
        {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public extern RoseMessageCollectionClass();

        [DispId(202)]
        public virtual extern Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(203)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMMessage GetAt(Int16 Index);

        [DispId(204)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Boolean Exists([MarshalAs(UnmanagedType.Interface)] REICOMMessage pObject);

        [DispId(205)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(206)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(207)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 IndexOf([MarshalAs(UnmanagedType.Interface)] REICOMMessage theObject);

        [DispId(208)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] REICOMMessage theObject);

        [DispId(209)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddCollection([MarshalAs(UnmanagedType.Interface)] REICOMMessageCollection theCollection);

        [DispId(210)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Remove([MarshalAs(UnmanagedType.Interface)] REICOMMessage theObject);

        [DispId(211)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoveAll();

        [DispId(212)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMMessage GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(213)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMMessage GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
        }
    }

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("67448182-4553-11D1-883B-3C8B00C10000")]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    public class REICoClassRealizeRelationCollection : IREICOMRealizeRelationCollection, RoseRealizeRelationCollection
        {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public extern RoseRealizeRelationCollectionClass();

        [DispId(202)]
        public virtual extern Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(203)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMRealizeRelation GetAt(Int16 Index);

        [DispId(204)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Boolean Exists([MarshalAs(UnmanagedType.Interface)] REICOMRealizeRelation pObject);

        [DispId(205)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(206)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(207)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern Int16 IndexOf([MarshalAs(UnmanagedType.Interface)] REICOMRealizeRelation theObject);

        [DispId(208)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] REICOMRealizeRelation theObject);

        [DispId(209)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void AddCollection([MarshalAs(UnmanagedType.Interface)] RoseRealizeRelationCollection theCollection);

        [DispId(210)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Remove([MarshalAs(UnmanagedType.Interface)] REICOMRealizeRelation theObject);

        [DispId(211)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoveAll();

        [DispId(212)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMRealizeRelation GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(213)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern REICOMRealizeRelation GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
        }
    }

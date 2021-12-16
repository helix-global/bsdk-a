using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("ED042E4F-6CDE-11D1-BC1E-00A024C67143")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMClassDependencyCollection
        {
        [DispId(202)]
        Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(203)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMClassDependency GetAt(Int16 Index);

        [DispId(204)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean Exists([MarshalAs(UnmanagedType.Interface)] IREICOMClassDependency pObject);

        [DispId(205)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(206)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(207)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int16 IndexOf([MarshalAs(UnmanagedType.IDispatch)] Object theObject);

        [DispId(208)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Add([MarshalAs(UnmanagedType.Interface)] IREICOMClassDependency theObject);

        [DispId(209)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddCollection([MarshalAs(UnmanagedType.Interface)] IREICOMClassDependencyCollection theCollection);

        [DispId(210)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Remove([MarshalAs(UnmanagedType.Interface)] IREICOMClassDependency theObject);

        [DispId(211)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveAll();

        [DispId(212)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMClassDependency GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(213)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMClassDependency GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
        }
    }

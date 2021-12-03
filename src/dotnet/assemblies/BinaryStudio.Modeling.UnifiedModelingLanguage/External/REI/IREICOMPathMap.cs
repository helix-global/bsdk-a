using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [Guid("4C9E2241-84C5-11D0-A214-444553540000")]
    [ComImport]
    public interface IREICOMPathMap
        {
        [DispId(50)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteEntry([MarshalAs(UnmanagedType.BStr)] String Symbol);

        [DispId(51)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetActualPath([MarshalAs(UnmanagedType.BStr)] String VirtualPath);

        [DispId(52)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetVirtualPath([MarshalAs(UnmanagedType.BStr)] String ActualPath);

        [DispId(53)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean HasEntry([MarshalAs(UnmanagedType.BStr)] String Symbol);

        [DispId(54)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean AddEntry([MarshalAs(UnmanagedType.BStr)] String Symbol, [MarshalAs(UnmanagedType.BStr)] String Path, [MarshalAs(UnmanagedType.BStr)] String Comment);

        [DispId(12674)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetActualPathWithContext([MarshalAs(UnmanagedType.BStr)] String VirtualPath, [MarshalAs(UnmanagedType.BStr)] String Context);

        [DispId(12675)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetVirtualPathWithContext([MarshalAs(UnmanagedType.BStr)] String ActualPath, [MarshalAs(UnmanagedType.BStr)] String Context);
        }
    }

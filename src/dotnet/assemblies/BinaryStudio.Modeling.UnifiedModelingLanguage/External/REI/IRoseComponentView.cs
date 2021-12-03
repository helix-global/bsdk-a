using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(4096)]
    [Guid("14028C94-C06C-11D0-89F5-0020AFD6C181")]
    [ComImport]
    public interface IRoseComponentView : IRoseItemView
        {
        [DispId(12585)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModule GetComponent();
        }
    }

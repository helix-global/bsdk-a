using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
    [TypeLibType(4096)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A69CAB22-9179-11D0-A214-00A024FFFE40")]
    [ComImport]
    public interface IRoseEvent : IRoseElement
    {
        [DispId(215)]
        String Arguments { [DispId(215), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(215), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12622)]
        String GuardCondition { [DispId(12622), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12622), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12634)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseAction GetAction();
    }
}

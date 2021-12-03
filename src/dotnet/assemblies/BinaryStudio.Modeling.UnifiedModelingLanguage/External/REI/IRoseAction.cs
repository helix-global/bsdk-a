using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace RationalRose
    {
    [TypeLibType(4096)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("13881143-93C1-11D0-A214-00A024FFFE40")]
    [ComImport]
    public interface IRoseAction : IRoseItem
        {
        [DispId(12620)]
        String Arguments { [DispId(12620), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12620), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12621)]
        String Target { [DispId(12621), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12621), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }
        }
    }

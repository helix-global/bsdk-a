using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("14028C92-C06C-11D0-89F5-0020AFD6C181")]
    [TypeLibType(4096)]
    [ComImport]
    public interface IRoseSubsystemView : IRoseItemView
        {
        [DispId(12592)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSubsystem GetSubsystem();
        }
    }

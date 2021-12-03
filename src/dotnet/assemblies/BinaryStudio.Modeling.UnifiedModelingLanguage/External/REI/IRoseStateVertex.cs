using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5E2-578D-11D2-92AA-004005141253")]
    [TypeLibType(4096)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IRoseStateVertex : IRoseItem
        {
        [DispId(413)]
        RoseStateMachine ParentStateMachine { [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(422)]
        RoseTransitionCollection Transitions { [DispId(422), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(422), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12747)]
        RoseStateVertex Parent { [DispId(12747), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12747), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(426)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseTransition AddTransition([MarshalAs(UnmanagedType.BStr)] String OnEvent, [MarshalAs(UnmanagedType.Interface)] RoseState Target);

        [DispId(427)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteTransition([MarshalAs(UnmanagedType.Interface)] RoseTransition Transition);

        [DispId(12748)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSwimLaneCollection GetSwimLanes();

        [DispId(12814)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseTransition AddTransitionToVertex([MarshalAs(UnmanagedType.BStr)] String OnEvent, [MarshalAs(UnmanagedType.Interface)] RoseStateVertex Target);
        }
    }

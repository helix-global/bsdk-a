using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [Guid("94CA1882-5D13-11D2-92AA-004005141253")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IRoseStateMachineOwner : IRoseElement
    {
        [DispId(12744)]
        RoseStateMachineCollection StateMachines { [DispId(12744), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12744), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }


        [DispId(12745)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseStateMachine CreateStateMachine([MarshalAs(UnmanagedType.BStr)] String theStateMachineName);

        [DispId(12746)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteStateMachine([MarshalAs(UnmanagedType.Interface)] RoseStateMachine theStateMachine);


        [DispId(12830)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseItem GetParentItem();
    }
}

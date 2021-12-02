using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(2)]
    [Guid("94CA1888-5D13-11D2-92AA-004005141253")]
    [TypeLibType(4096)]
    [ComImport]
    public interface IRoseSyncItemView : IRoseItemView
        {
        [DispId(12789)]
        Boolean Horizontal { [DispId(12789), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12789), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12758)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSyncItem GetSynchronization();
        }
    }

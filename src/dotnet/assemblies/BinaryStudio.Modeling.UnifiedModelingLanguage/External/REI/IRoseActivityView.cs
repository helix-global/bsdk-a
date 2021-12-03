using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace RationalRose
    {
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("BEAED5FC-578D-11D2-92AA-004005141253")]
    [ComImport]
    public interface IRoseActivityView : IRoseItemView
        {
        [DispId(12741)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseActivity GetActivity();
        }
    }

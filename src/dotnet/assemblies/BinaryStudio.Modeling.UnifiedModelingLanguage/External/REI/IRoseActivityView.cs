using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(4096)]
    [InterfaceType(2)]
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

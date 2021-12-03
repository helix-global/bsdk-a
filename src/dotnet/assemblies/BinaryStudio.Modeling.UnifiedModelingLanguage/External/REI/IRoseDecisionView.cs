using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5F9-578D-11D2-92AA-004005141253")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(4096)]
    [ComImport]
    public interface IRoseDecisionView : IRoseItemView
        {
        [DispId(12742)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseDecision GetDecision();
        }
    }

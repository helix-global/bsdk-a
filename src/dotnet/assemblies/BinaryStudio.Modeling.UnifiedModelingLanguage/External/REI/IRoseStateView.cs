using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(2)]
    [Guid("7BD909E1-9AF9-11D0-A214-00A024FFFE40")]
    [TypeLibType(4096)]
    [ComImport]
    public interface IRoseStateView : IRoseItemView
        {
        [DispId(415)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseState GetState();
        }
    }

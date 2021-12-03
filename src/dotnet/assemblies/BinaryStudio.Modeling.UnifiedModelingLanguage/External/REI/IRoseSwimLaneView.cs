using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(4096)]
    [Guid("68F63C21-B047-11D2-92AA-004005141253")]
    [ComImport]
    public interface IRoseSwimLaneView : IRoseItemView
        {
        }
    }

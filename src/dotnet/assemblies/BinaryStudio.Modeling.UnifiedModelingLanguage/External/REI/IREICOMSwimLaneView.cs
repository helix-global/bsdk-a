using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [Guid("68F63C21-B047-11D2-92AA-004005141253")]
    [ComImport]
    public interface IREICOMSwimLaneView : IREICOMItemView
        {
        }
    }

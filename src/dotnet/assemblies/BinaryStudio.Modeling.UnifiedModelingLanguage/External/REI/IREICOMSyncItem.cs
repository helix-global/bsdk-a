using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("94CA188B-5D13-11D2-92AA-004005141253")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMSyncItem : IREICOMStateVertex
        {
        }
    }

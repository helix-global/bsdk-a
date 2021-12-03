using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("BEAED5E3-578D-11D2-92AA-004005141253")]
    [ComImport]
    public interface IREICOMDecision : IREICOMStateVertex
        {
        }
    }

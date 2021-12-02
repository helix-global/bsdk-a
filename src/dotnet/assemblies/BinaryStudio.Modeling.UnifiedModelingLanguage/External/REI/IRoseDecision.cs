using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(4096)]
    [InterfaceType(2)]
    [Guid("BEAED5E3-578D-11D2-92AA-004005141253")]
    [ComImport]
    public interface IRoseDecision : IRoseStateVertex
        {
        }
    }

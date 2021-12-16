using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5E2-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassStateVertex))]
    [ComImport]
    internal interface REICOMStateVertex : IREICOMStateVertex
        {
        }
    }

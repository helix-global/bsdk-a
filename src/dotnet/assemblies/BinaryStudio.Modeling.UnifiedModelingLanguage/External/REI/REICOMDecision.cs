using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassDecision))]
    [Guid("BEAED5E3-578D-11D2-92AA-004005141253")]
    [ComImport]
    internal interface REICOMDecision : IREICOMDecision
        {
        }
    }

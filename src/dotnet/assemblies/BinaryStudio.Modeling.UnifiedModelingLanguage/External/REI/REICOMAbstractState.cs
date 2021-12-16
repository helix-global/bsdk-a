using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassAbstractState))]
    [Guid("BEAED5EC-578D-11D2-92AA-004005141253")]
    [ComImport]
    internal interface REICOMAbstractState : IREICOMAbstractState
        {
        }
    }

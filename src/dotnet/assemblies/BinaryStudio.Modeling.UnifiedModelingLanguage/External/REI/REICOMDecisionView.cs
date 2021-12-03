using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5F9-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassDecisionView))]
    [ComImport]
    internal interface REICOMDecisionView : IREICOMDecisionView
        {
        }
    }

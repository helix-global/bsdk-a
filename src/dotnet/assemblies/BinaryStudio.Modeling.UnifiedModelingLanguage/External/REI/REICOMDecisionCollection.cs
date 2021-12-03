using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassDecisionCollection))]
    [Guid("BEAED5F2-578D-11D2-92AA-004005141253")]
    [ComImport]
    internal interface REICOMDecisionCollection : IREICOMDecisionCollection
        {
        }
    }

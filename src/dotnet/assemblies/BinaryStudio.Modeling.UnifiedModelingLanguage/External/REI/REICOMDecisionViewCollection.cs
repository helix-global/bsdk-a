using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED601-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassDecisionViewCollection))]
    [ComImport]
    internal interface REICOMDecisionViewCollection : IREICOMDecisionViewCollection
        {
        }
    }

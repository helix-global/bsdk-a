using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5EE-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassAbstractStateCollection))]
    [ComImport]
    internal interface REICOMAbstractStateCollection : IREICOMAbstractStateCollection
        {
        }
    }

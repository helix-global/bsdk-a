using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5E7-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassActivity))]
    [ComImport]
    internal interface REICOMActivity : IREICOMActivity
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseDecisionCollectionClass))]
    [Guid("BEAED5F2-578D-11D2-92AA-004005141253")]
    [ComImport]
    public interface REICOMDecisionCollection : IREICOMDecisionCollection
        {
        }
    }

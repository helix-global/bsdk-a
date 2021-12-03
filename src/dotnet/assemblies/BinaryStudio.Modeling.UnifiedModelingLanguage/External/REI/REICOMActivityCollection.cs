using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BEAED5F0-578D-11D2-92AA-004005141253")]
    [CoClass(typeof(RoseActivityCollectionClass))]
    [ComImport]
    public interface REICOMActivityCollection : IREICOMActivityCollection
        {
        }
    }

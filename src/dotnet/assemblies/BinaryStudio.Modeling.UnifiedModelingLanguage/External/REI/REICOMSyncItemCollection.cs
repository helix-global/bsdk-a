using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("94CA188F-5D13-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassSyncItemCollection))]
    [ComImport]
    internal interface REICOMSyncItemCollection : IREICOMSyncItemCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("94CA188B-5D13-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassSyncItem))]
    [ComImport]
    internal interface REICOMSyncItem : IREICOMSyncItem
        {
        }
    }

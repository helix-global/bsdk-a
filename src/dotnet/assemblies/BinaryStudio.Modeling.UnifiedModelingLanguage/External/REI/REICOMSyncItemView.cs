using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("94CA1888-5D13-11D2-92AA-004005141253")]
    [CoClass(typeof(REICoClassSyncItemView))]
    [ComImport]
    internal interface REICOMSyncItemView : IREICOMSyncItemView
        {
        }
    }

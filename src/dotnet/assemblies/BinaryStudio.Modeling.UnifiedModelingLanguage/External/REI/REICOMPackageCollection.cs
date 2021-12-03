using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38364-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassPackageCollection))]
    [ComImport]
    internal interface REICOMPackageCollection : IREICOMPackageCollection
        {
        }
    }

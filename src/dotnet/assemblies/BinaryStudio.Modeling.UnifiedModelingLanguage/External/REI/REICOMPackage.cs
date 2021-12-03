using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("47D975C1-8A8D-11D0-A214-444553540000")]
    [CoClass(typeof(REICoClassPackage))]
    [ComImport]
    internal interface REICOMPackage : IREICOMPackage
        {
        }
    }

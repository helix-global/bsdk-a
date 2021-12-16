using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BA242E00-8961-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassRole))]
    [ComImport]
    internal interface REICOMRole : IREICOMRole
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BC57D1C2-863E-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassItem))]
    [ComImport]
    internal interface REICOMItem : IREICOMItem
        {
        }
    }

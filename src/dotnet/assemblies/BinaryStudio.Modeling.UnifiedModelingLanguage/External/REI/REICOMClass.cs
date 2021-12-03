using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassClass))]
    [Guid("BC57D1C0-863E-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    internal interface REICOMClass : IREICOMClass
        {
        }
    }

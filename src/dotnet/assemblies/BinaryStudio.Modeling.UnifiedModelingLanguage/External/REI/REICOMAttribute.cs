using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassAttribute))]
    [Guid("C78E7024-86E4-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    internal interface REICOMAttribute : IREICOMAttribute
        {
        }
    }

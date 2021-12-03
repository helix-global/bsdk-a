using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C78E7020-86E4-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassOperation))]
    [ComImport]
    internal interface REICOMOperation : IREICOMOperation
        {
        }
    }

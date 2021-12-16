using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C78E7026-86E4-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassAssociation))]
    [ComImport]
    internal interface REICOMAssociation : IREICOMAssociation
        {
        }
    }

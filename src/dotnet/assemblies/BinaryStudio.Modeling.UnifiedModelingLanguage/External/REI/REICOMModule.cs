using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C78E702A-86E4-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassModule))]
    [ComImport]
    internal interface REICOMModule : IREICOMModule
        {
        }
    }

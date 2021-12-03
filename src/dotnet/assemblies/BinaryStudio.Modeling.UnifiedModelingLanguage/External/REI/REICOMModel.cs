using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("E38942A0-8621-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassModel))]
    [ComImport]
    internal interface REICOMModel : IREICOMModel
        {
        }
    }

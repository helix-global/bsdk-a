using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C87D2BC1-352A-11D1-883B-3C8B00C10000")]
    [CoClass(typeof(REICoClassAddInCollection))]
    [ComImport]
    internal interface REICOMAddInCollection : IREICOMAddInCollection
        {
        }
    }

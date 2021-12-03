using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassCategoryCollection))]
    [Guid("97B3835B-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    internal interface REICOMCategoryCollection : IREICOMCategoryCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38362-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassItemViewCollection))]
    [ComImport]
    internal interface REICOMItemViewCollection : IREICOMItemViewCollection
        {
        }
    }

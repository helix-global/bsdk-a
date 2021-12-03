using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38366-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassProcessCollection))]
    [ComImport]
    internal interface REICOMProcessCollection : IREICOMProcessCollection
        {
        }
    }

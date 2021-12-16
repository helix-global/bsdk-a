using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassPropertyCollection))]
    [Guid("97B3835D-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    internal interface REICOMPropertyCollection : IREICOMPropertyCollection
        {
        }
    }

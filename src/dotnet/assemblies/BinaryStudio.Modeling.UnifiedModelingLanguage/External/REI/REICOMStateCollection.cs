using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38367-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassStateCollection))]
    [ComImport]
    internal interface REICOMStateCollection : IREICOMStateCollection
        {
        }
    }

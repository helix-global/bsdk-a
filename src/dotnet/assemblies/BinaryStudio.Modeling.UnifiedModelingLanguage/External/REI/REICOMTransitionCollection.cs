using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3836B-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassTransitionCollection))]
    [ComImport]
    internal interface REICOMTransitionCollection : IREICOMTransitionCollection
        {
        }
    }

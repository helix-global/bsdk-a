using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3836A-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassStateViewCollection))]
    [ComImport]
    internal interface REICOMStateViewCollection : IREICOMStateViewCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3834D-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassOperationCollection))]
    [ComImport]
    internal interface REICOMOperationCollection : IREICOMOperationCollection
        {
        }
    }

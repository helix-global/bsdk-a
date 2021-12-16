using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassAttributeCollection))]
    [Guid("97B3834C-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    internal interface REICOMAttributeCollection : IREICOMAttributeCollection
        {
        }
    }

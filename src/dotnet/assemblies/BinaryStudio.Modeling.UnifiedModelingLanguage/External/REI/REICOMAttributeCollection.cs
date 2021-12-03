using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseAttributeCollectionClass))]
    [Guid("97B3834C-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMAttributeCollection : IREICOMAttributeCollection
        {
        }
    }

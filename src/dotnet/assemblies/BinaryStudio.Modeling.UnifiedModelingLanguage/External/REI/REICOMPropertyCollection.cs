using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RosePropertyCollectionClass))]
    [Guid("97B3835D-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMPropertyCollection : IREICOMPropertyCollection
        {
        }
    }

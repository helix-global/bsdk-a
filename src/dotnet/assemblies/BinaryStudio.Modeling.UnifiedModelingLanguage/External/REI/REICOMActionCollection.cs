using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseActionCollectionClass))]
    [Guid("97B3835F-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMActionCollection : IREICOMActionCollection
        {
        }
    }

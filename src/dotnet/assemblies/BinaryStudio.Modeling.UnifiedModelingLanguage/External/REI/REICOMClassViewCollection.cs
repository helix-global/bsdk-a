using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseClassViewCollectionClass))]
    [Guid("97B38341-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMClassViewCollection : IREICOMClassViewCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseClassCollectionClass))]
    [Guid("97B38349-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMClassCollection : IREICOMClassCollection
        {
        }
    }

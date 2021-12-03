using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseAssociationCollectionClass))]
    [Guid("97B3834E-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMAssociationCollection : IREICOMAssociationCollection
        {
        }
    }

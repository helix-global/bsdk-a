using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseHasRelationshipCollectionClass))]
    [Guid("97B38351-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMHasRelationshipCollection : IREICOMHasRelationshipCollection
        {
        }
    }

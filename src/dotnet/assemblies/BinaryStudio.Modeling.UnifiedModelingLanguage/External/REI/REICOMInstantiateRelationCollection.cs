using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("B91D8F06-DDBB-11D1-9FAD-0060975306FE")]
    [CoClass(typeof(RoseInstantiateRelationCollectionClass))]
    [ComImport]
    public interface REICOMInstantiateRelationCollection : IREICOMInstantiateRelationCollection
        {
        }
    }

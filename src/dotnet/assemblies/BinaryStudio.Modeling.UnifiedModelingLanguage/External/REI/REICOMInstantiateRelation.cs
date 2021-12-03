using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseInstantiateRelationClass))]
    [Guid("B91D8F03-DDBB-11D1-9FAD-0060975306FE")]
    [ComImport]
    public interface REICOMInstantiateRelation : IREICOMInstantiateRelation
        {
        }
    }

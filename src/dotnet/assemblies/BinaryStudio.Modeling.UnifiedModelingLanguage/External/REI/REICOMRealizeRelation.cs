using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseRealizeRelationClass))]
    [Guid("6AC2BA81-454D-11D1-883B-3C8B00C10000")]
    [ComImport]
    public interface REICOMRealizeRelation : IREICOMRealizeRelation
        {
        }
    }

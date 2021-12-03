using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseDiagramCollectionClass))]
    [Guid("38E8FEC2-969A-11D3-92AA-004005141253")]
    [ComImport]
    public interface REICOMDiagramCollection : IREICOMDiagramCollection
        {
        }
    }

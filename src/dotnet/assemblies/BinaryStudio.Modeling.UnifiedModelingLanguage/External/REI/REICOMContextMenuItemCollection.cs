using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseContextMenuItemCollectionClass))]
    [Guid("EE0B16E2-FF91-11D1-9FAD-0060975306FE")]
    [ComImport]
    public interface REICOMContextMenuItemCollection : IREICOMContextMenuItemCollection
        {
        }
    }

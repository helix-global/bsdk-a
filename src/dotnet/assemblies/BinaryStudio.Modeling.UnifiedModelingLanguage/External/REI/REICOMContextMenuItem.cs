using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("EE0B16E0-FF91-11D1-9FAD-0060975306FE")]
    [CoClass(typeof(REICoClassContextMenuItem))]
    [ComImport]
    internal interface REICOMContextMenuItem : IREICOMContextMenuItem
        {
        }
    }

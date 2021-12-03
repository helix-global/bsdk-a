using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("7DFAFE40-A29D-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(RoseItemViewClass))]
    [ComImport]
    public interface REICOMItemView : IREICOMItemView
        {
        }
    }

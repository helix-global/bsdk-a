using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("14028C94-C06C-11D0-89F5-0020AFD6C181")]
    [CoClass(typeof(RoseComponentViewClass))]
    [ComImport]
    public interface REICOMComponentView : IREICOMComponentView
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseAddInManagerClass))]
    [Guid("D5352FC2-346C-11D1-883B-3C8B00C10000")]
    [ComImport]
    public interface REICOMAddInManager : IREICOMAddInManager
        {
        }
    }

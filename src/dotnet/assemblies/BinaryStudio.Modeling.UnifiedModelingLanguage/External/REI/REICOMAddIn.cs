using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseAddInClass))]
    [Guid("D5352FC0-346C-11D1-883B-3C8B00C10000")]
    [ComImport]
    public interface REICOMAddIn : IREICOMAddIn
        {
        }
    }

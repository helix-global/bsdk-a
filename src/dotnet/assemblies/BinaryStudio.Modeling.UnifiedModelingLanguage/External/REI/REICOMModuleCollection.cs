using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassModuleCollection))]
    [Guid("97B3834B-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    internal interface REICOMModuleCollection : IREICOMModuleCollection
        {
        }
    }

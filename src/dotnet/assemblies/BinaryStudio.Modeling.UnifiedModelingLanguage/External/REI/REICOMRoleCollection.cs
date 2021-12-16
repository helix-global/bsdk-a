using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38353-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassRoleCollection))]
    [ComImport]
    internal interface REICOMRoleCollection : IREICOMRoleCollection
        {
        }
    }

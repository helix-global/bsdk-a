using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3835A-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassObjectInstanceCollection))]
    [ComImport]
    internal interface REICOMObjectInstanceCollection : IREICOMObjectInstanceCollection
        {
        }
    }

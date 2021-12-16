using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3834A-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassSubsystemCollection))]
    [ComImport]
    internal interface REICOMSubsystemCollection : IREICOMSubsystemCollection
        {
        }
    }

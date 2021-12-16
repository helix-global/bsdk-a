using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassControllableUnitCollection))]
    [Guid("97B38360-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    internal interface REICOMControllableUnitCollection : IREICOMControllableUnitCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38356-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassUseCaseCollection))]
    [ComImport]
    internal interface REICOMUseCaseCollection : IREICOMUseCaseCollection
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B38359-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassMessageCollection))]
    [ComImport]
    internal interface REICOMMessageCollection : IREICOMMessageCollection
        {
        }
    }

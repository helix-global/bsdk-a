using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("97B3835C-A4E3-11D0-BFF0-00AA003DEF5B")]
    [CoClass(typeof(REICoClassProcessorCollection))]
    [ComImport]
    internal interface REICOMProcessorCollection : IREICOMProcessorCollection
        {
        }
    }

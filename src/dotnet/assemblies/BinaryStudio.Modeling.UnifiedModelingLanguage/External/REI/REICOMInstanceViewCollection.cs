using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C640C864-F2D3-11D0-883A-3C8B00C10000")]
    [CoClass(typeof(REICoClassInstanceViewCollection))]
    [ComImport]
    internal interface REICOMInstanceViewCollection : IREICOMInstanceViewCollection
        {
        }
    }

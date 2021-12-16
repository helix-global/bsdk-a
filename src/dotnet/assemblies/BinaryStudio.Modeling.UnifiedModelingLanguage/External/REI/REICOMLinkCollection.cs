using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassLinkCollection))]
    [Guid("9DE9A9C1-F2D0-11D0-883A-3C8B00C10000")]
    [ComImport]
    internal interface REICOMLinkCollection : IREICOMLinkCollection
        {
        }
    }

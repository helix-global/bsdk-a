using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("0DD9ACF8-D06E-11D0-BC0B-00A024C67143")]
    [CoClass(typeof(REICoClassItemCollection))]
    [ComImport]
    internal interface REICOMItemCollection : IREICOMItemCollection
        {
        }
    }

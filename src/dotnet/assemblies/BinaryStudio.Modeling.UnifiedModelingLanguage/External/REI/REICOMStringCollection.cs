using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("6A7FC311-C893-11D0-BC0B-00A024C67143")]
    [CoClass(typeof(REICoClassStringCollection))]
    [ComImport]
    internal interface REICOMStringCollection : IREICOMStringCollection
        {
        }
    }

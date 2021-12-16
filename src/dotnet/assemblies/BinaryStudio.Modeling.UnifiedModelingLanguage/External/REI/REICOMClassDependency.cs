using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("4ACE1899-6CD3-11D1-BC1E-00A024C67143")]
    [CoClass(typeof(REICoClassClassDependency))]
    [ComImport]
    internal interface REICOMClassDependency : IREICOMClassDependency
        {
        }
    }

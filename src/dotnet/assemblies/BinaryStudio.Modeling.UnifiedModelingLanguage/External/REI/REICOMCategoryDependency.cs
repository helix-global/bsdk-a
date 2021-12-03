using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseCategoryDependencyClass))]
    [Guid("4ACE189B-6CD3-11D1-BC1E-00A024C67143")]
    [ComImport]
    public interface REICOMCategoryDependency : IREICOMCategoryDependency
        {
        }
    }

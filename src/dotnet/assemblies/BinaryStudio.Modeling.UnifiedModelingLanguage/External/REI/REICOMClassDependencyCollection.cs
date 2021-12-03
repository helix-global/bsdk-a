using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("ED042E4F-6CDE-11D1-BC1E-00A024C67143")]
    [CoClass(typeof(RoseClassDependencyCollectionClass))]
    [ComImport]
    public interface REICOMClassDependencyCollection : IREICOMClassDependencyCollection
        {
        }
    }

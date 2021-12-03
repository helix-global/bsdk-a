using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseComponentViewCollectionClass))]
    [Guid("C640C861-F2D3-11D0-883A-3C8B00C10000")]
    [ComImport]
    public interface REICOMComponentViewCollection : IREICOMComponentViewCollection
        {
        }
    }

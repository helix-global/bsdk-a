using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseExternalDocumentCollectionClass))]
    [Guid("97B38357-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMExternalDocumentCollection : IREICOMExternalDocumentCollection
        {
        }
    }

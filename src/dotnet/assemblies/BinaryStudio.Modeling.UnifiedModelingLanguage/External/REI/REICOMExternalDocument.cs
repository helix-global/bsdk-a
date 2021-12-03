using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseExternalDocumentClass))]
    [Guid("906FF583-276B-11D0-8980-00A024774419")]
    [ComImport]
    public interface REICOMExternalDocument : IREICOMExternalDocument
        {
        }
    }

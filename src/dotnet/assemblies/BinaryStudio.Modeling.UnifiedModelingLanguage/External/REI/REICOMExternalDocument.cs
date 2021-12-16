using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassExternalDocument))]
    [Guid("906FF583-276B-11D0-8980-00A024774419")]
    [ComImport]
    internal interface REICOMExternalDocument : IREICOMExternalDocument
        {
        }
    }

using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("7E7F6EE0-16DE-11D0-8976-00A024774419")]
    [CoClass(typeof(REICoClassUseCase))]
    [ComImport]
    internal interface REICOMUseCase : IREICOMUseCase
        {
        }
    }

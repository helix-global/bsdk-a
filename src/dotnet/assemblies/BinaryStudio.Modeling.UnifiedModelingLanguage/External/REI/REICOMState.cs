using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassState))]
    [Guid("A69CAB23-9179-11D0-A214-00A024FFFE40")]
    [ComImport]
    internal interface REICOMState : IREICOMState
        {
        }
    }

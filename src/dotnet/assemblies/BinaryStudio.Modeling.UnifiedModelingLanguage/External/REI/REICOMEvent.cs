using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("A69CAB22-9179-11D0-A214-00A024FFFE40")]
    [CoClass(typeof(REICoClassEvent))]
    [ComImport]
    internal interface REICOMEvent : IREICOMEvent
        {
        }
    }

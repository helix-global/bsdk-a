using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("A69CAB21-9179-11D0-A214-00A024FFFE40")]
    [CoClass(typeof(REICoClassStateMachine))]
    [ComImport]
    internal interface REICOMStateMachine : IREICOMStateMachine
        {
        }
    }

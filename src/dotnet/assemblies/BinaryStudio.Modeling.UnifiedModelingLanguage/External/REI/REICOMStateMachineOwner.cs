using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassStateMachineOwner))]
    [Guid("94CA1882-5D13-11D2-92AA-004005141253")]
    [ComImport]
    internal interface REICOMStateMachineOwner : IREICOMStateMachineOwner
        {
        }
    }

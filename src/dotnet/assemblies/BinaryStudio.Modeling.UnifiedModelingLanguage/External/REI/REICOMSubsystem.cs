using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassSubsystem))]
    [Guid("C78E702C-86E4-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    internal interface REICOMSubsystem : IREICOMSubsystem
        {
        }
    }

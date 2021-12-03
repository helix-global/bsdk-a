using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("14028C92-C06C-11D0-89F5-0020AFD6C181")]
    [CoClass(typeof(REICoClassSubsystemView))]
    [ComImport]
    internal interface REICOMSubsystemView : IREICOMSubsystemView
        {
        }
    }

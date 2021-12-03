using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("CA3AD902-BFCE-11D0-89F5-0020AFD6C181")]
    [CoClass(typeof(REICoClassSubsystemViewCollection))]
    [ComImport]
    internal interface REICOMSubsystemViewCollection : IREICOMSubsystemViewCollection
        {
        }
    }

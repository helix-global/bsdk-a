using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("195D7852-D5B6-11D0-89F8-0020AFD6C181")]
    [CoClass(typeof(RoseLinkClass))]
    [ComImport]
    public interface REICOMLink : IREICOMLink
        {
        }
    }

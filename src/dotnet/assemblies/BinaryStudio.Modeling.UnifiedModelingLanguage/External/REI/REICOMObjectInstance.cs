using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseObjectInstanceClass))]
    [Guid("F8198337-FC55-11CF-BBD3-00A024C67143")]
    [ComImport]
    public interface REICOMObjectInstance : IREICOMObjectInstance
        {
        }
    }

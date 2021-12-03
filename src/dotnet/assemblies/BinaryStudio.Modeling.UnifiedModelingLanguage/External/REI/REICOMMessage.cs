using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseMessageClass))]
    [Guid("F819833C-FC55-11CF-BBD3-00A024C67143")]
    [ComImport]
    public interface REICOMMessage : IREICOMMessage
        {
        }
    }

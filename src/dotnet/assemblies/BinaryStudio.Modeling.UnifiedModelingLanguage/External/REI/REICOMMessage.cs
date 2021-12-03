using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassMessage))]
    [Guid("F819833C-FC55-11CF-BBD3-00A024C67143")]
    [ComImport]
    internal interface REICOMMessage : IREICOMMessage
        {
        }
    }

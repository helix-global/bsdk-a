using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassElement))]
    [Guid("D067F15F-6987-11D0-BBF0-00A024C67143")]
    [ComImport]
    internal interface REICOMElement : IREICOMElement
        {
        }
    }

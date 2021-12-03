using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("62C43882-DB5A-11CF-B091-00A0241E3F73")]
    [CoClass(typeof(REICoClassDevice))]
    [ComImport]
    internal interface REICOMDevice : IREICOMDevice
        {
        }
    }

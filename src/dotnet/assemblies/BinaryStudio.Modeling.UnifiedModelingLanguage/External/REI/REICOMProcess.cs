using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("62C43884-DB5A-11CF-B091-00A0241E3F73")]
    [CoClass(typeof(REICoClassProcess))]
    [ComImport]
    internal interface REICOMProcess : IREICOMProcess
        {
        }
    }

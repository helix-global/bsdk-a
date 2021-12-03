using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassProcessor))]
    [Guid("62C43886-DB5A-11CF-B091-00A0241E3F73")]
    [ComImport]
    internal interface REICOMProcessor : IREICOMProcessor
        {
        }
    }

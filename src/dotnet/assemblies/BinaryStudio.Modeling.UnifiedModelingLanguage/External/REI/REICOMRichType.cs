using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassRichType))]
    [Guid("EB7AAB60-939C-11CF-B091-00A0241E3F73")]
    [ComImport]
    internal interface REICOMRichType : IREICOMRichType
        {
        }
    }

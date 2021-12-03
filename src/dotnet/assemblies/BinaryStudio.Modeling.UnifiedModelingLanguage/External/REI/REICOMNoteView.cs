using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseNoteViewClass))]
    [Guid("015655CA-72DF-11D0-95EB-0000F803584A")]
    [ComImport]
    public interface REICOMNoteView : IREICOMNoteView
        {
        }
    }

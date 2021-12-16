using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassDiagram))]
    [Guid("3FD9D000-93B0-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    internal interface REICOMDiagram : IREICOMDiagram
        {
        }
    }

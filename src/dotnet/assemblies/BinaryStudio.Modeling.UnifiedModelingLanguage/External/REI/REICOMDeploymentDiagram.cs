using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("C2C15EC4-E028-11CF-B091-00A0241E3F73")]
    [CoClass(typeof(RoseDeploymentDiagramClass))]
    [ComImport]
    public interface REICOMDeploymentDiagram : IREICOMDeploymentDiagram
        {
        }
    }

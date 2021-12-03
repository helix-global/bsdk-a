using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseDeploymentUnitClass))]
    [Guid("4335FBE3-F0A0-11D1-9FAD-0060975306FE")]
    [ComImport]
    public interface REICOMDeploymentUnit : IREICOMDeploymentUnit
        {
        }
    }

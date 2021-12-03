using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassDeploymentUnit))]
    [Guid("4335FBE3-F0A0-11D1-9FAD-0060975306FE")]
    [ComImport]
    internal interface REICOMDeploymentUnit : IREICOMDeploymentUnit
        {
        }
    }

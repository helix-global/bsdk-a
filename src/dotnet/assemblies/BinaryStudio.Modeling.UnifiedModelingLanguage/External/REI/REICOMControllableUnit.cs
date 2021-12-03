using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseControllableUnitClass))]
    [Guid("32C862A7-8AA9-11D0-A70B-0000F803584A")]
    [ComImport]
    public interface REICOMControllableUnit : IREICOMControllableUnit
        {
        }
    }

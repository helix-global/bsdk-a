using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("BA242E04-8961-11CF-B3D4-00A0241DB1D0")]
    [CoClass(typeof(REICoClassHasRelationship))]
    [ComImport]
    internal interface REICOMHasRelationship : IREICOMHasRelationship
        {
        }
    }

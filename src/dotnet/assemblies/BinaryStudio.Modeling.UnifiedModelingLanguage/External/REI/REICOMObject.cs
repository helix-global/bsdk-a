using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("7D8474B2-2C33-11D0-BBDA-00A024C67143")]
    [CoClass(typeof(REICoClassObject))]
    [ComImport]
    internal interface REICOMObject : IREICOMObject
        {
        }
    }

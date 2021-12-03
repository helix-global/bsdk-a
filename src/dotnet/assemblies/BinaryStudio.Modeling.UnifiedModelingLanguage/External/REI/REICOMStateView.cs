using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(REICoClassStateView))]
    [Guid("7BD909E1-9AF9-11D0-A214-00A024FFFE40")]
    [ComImport]
    internal interface REICOMStateView : IREICOMStateView
        {
        }
    }

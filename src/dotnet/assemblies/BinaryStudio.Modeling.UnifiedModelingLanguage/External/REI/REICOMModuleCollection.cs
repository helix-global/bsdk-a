﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseModuleCollectionClass))]
    [Guid("97B3834B-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMModuleCollection : IREICOMModuleCollection
        {
        }
    }

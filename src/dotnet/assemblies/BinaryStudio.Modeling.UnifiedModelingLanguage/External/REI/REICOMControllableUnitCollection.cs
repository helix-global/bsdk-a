﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseControllableUnitCollectionClass))]
    [Guid("97B38360-A4E3-11D0-BFF0-00AA003DEF5B")]
    [ComImport]
    public interface REICOMControllableUnitCollection : IREICOMControllableUnitCollection
        {
        }
    }

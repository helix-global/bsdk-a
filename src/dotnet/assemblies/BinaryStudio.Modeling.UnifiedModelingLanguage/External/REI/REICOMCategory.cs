﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseCategoryClass))]
    [Guid("D7BC1B45-8618-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    public interface REICOMCategory : IREICOMCategory
        {
        }
    }

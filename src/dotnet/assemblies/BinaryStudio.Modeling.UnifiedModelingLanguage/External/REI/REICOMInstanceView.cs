﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseInstanceViewClass))]
    [Guid("348B1AD4-D5C4-11D0-89F8-0020AFD6C181")]
    [ComImport]
    public interface REICOMInstanceView : IREICOMInstanceView
        {
        }
    }

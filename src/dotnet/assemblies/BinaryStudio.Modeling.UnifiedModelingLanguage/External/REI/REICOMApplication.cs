﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseApplicationClass))]
    [Guid("D7BC1B40-8618-11CF-B3D4-00A0241DB1D0")]
    [ComImport]
    public interface REICOMApplication : IREICOMApplication
        {
        }
    }

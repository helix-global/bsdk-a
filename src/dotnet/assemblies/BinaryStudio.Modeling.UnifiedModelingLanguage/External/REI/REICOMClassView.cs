﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("5F735F36-F9EA-11CF-BBD3-00A024C67143")]
    [CoClass(typeof(RoseClassViewClass))]
    [ComImport]
    public interface REICOMClassView : IREICOMClassView
        {
        }
    }

﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("13881143-93C1-11D0-A214-00A024FFFE40")]
    [CoClass(typeof(RoseActionClass))]
    [ComImport]
    public interface REICOMAction : IRoseAction
        {
        }
    }

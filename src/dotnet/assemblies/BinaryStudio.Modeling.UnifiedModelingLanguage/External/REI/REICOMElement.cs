﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseElementClass))]
    [Guid("D067F15F-6987-11D0-BBF0-00A024C67143")]
    [ComImport]
    public interface REICOMElement : IREICOMElement
        {
        }
    }

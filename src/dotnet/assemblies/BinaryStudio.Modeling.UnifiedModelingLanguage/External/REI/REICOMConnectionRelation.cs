﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseConnectionRelationClass))]
    [Guid("4467F442-F24E-11D2-92AA-004005141253")]
    [ComImport]
    public interface REICOMConnectionRelation : IREICOMConnectionRelation
        {
        }
    }

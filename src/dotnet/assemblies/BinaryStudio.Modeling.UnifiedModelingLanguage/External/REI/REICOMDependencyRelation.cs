﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("882D2F85-BD12-11D3-92AA-004005141253")]
    [CoClass(typeof(RoseDependencyRelationClass))]
    [ComImport]
    public interface REICOMDependencyRelation : IREICOMDependencyRelation
        {
        }
    }

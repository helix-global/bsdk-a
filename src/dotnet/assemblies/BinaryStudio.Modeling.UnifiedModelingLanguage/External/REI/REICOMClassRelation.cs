﻿using System.Runtime.InteropServices;

namespace RationalRose
    {
    [CoClass(typeof(RoseClassRelationClass))]
    [Guid("00C99564-9200-11CF-B1B0-D227D5210B2C")]
    [ComImport]
    public interface REICOMClassRelation : IREICOMClassRelation
        {
        }
    }

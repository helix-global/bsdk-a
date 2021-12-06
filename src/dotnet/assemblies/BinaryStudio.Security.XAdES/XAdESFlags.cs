using System;

namespace BinaryStudio.Security.XAdES
    {
    [Flags]
    public enum XAdESFlags
        {
        IncludeSigningCertificate = 1,
        IncludeTimeStampCertificate = 2,
        IncludeTimeStamp = 4
        }
    }
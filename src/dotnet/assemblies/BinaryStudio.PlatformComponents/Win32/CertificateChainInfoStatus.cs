using System;

namespace Microsoft.Win32
    {
    [Flags]
    public enum CertificateChainInfoStatus
        {
        HasExactMatchIssuer         = 0x00000001,
        HasKeyMatchIssuer           = 0x00000002,
        HasNameMatchIssuer          = 0x00000004,
        IsSelfSigned                = 0x00000008,
        HasPreferredIssuer          = 0x00000100,
        HasIssuanceChainPolicy      = 0x00000200,
        HasValidNameConstraints     = 0x00000400,
        IsPeerTrusted               = 0x00000800,
        HasCRLValidityExtended      = 0x00001000,
        IsFromExclusiveTrustStore   = 0x00002000,
        IsComplexChain              = 0x00010000,
        IsCATrusted                 = 0x00004000
        }
    }
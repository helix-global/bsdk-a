namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    public enum ReasonFlags
        {
        Unused                  = 0,
        KeyCompromise           = 1,
        CACompromise            = 2,
        AffiliationChanged      = 3,
        Superseded              = 4,
        CessationOfOperation    = 5,
        CertificateHold         = 6,
        PrivilegeWithdrawn      = 7,
        AACompromise            = 8
        }
    }
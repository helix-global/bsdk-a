namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public enum X509CrlReason
        {
        Unspecified,
        KeyCompromise,
        CACompromise,
        AffiliationChanged,
        Superseded,
        CessationOfOperation,
        CertificateHold,
        RemoveFromCRL
        }
    }
using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateResolver
        {
        IX509Certificate Find(String serialnumber, String issuer);
        IX509Certificate Find(String thumbprint);
        }
    }
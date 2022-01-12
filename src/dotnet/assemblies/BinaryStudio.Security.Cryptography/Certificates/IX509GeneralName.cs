using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509GeneralName
        {
        Boolean IsEmpty { get; }
        X509GeneralNameType Type { get; }
        }
    }

using System;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public interface IX509GeneralName
        {
        Boolean IsEmpty { get; }
        X509GeneralNameType Type { get; }
        }
    }

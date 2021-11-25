using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public interface IIcaoCertificate
        {
        IcaoCertificateType Type { get; }
        }
    }
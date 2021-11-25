using System.Linq;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [Asn1SpecificObject("1.2.840.113549.1.1.10")]
    public sealed class RSAWithPSSSignatureAlgorithm : Asn1SignatureAlgorithm
        {
        public override Asn1ObjectIdentifier HashAlgorithm { get; }
        public RSAWithPSSSignatureAlgorithm(Asn1Object source)
            : base(source)
            {
            if (source.Count > 1) {
                var contextspecifics = source[1].OfType<Asn1ContextSpecificObject>().ToArray();
                var contextspecific = contextspecifics.FirstOrDefault(i => i.Type == 0);
                if (contextspecific != null) {
                    HashAlgorithm = (Asn1ObjectIdentifier)contextspecific[0][0];
                    }
                }
            }
        }
    }
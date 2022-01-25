using System.Linq;
using System.Security.Cryptography;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [Asn1SpecificObject("1.2.840.113549.1.1.10")]
    public sealed class RSAWithPSSSignatureAlgorithm : Asn1SignatureAlgorithm
        {
        public override Oid HashAlgorithm { get; }
        public RSAWithPSSSignatureAlgorithm(Asn1Object source)
            : base(source)
            {
            if (source.Count > 1) {
                var contextspecifics = source[1].OfType<Asn1ContextSpecificObject>().ToArray();
                var contextspecific = contextspecifics.FirstOrDefault(i => i.Type == 0);
                if (contextspecific != null) {
                    HashAlgorithm = new Oid(((Asn1ObjectIdentifier)contextspecific[0][0]).ToString());
                    }
                }
            HashAlgorithm = HashAlgorithm ?? new Oid(szOID_OIWSEC_sha1);
            }
        }
    }
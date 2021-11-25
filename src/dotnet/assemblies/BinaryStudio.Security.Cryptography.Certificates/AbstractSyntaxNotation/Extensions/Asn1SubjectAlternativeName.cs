namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) subjectAltName(17)}
     * 2.5.29.17
     * /Joint-ISO-ITU-T/5/29/17
     * Subject alternative name ("subjectAltName" extension)
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.17")]
    public class Asn1SubjectAlternativeName : Asn1IssuerAlternativeName
        {
        public Asn1SubjectAlternativeName(Asn1CertificateExtension source)
            : base(source)
            {
            }
        }
    }
using System;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
{
    public sealed class X509CertificateAlgorithmIdentifier
        {
        public Asn1ObjectIdentifier Identifier { get; }
        //public X509CertificateAlgorithmParameters Parameters { get; }

        internal X509CertificateAlgorithmIdentifier(Asn1Sequence source) {
            if (ReferenceEquals(source, null)) { throw new ArgumentNullException(nameof(source)); }
            Identifier = (Asn1ObjectIdentifier)source[0];
            //Parameters = X509CertificateAlgorithmParameters.From(Identifier.ToString(), new X509CertificateAlgorithmParameters(source[1]));
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return (Identifier != null)
                ? Identifier.ToString()
                : "(none)";
            }
        }

}
using System;
using System.ComponentModel;
using System.Globalization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /**
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) basicConstraints(19)}
     * {2.5.29.19}
     * {/Joint-ISO-ITU-T/5/29/19}
     * IETF RFC 5280,3280
     *
     * id-ce-basicConstraints OBJECT IDENTIFIER ::=  { id-ce 19 }
     * BasicConstraints ::= SEQUENCE
     * {
     *   cA                      BOOLEAN DEFAULT FALSE,
     *   pathLenConstraint       INTEGER (0..MAX) OPTIONAL
     * }
     */
    [Asn1SpecificObject("2.5.29.19")]
    internal sealed class Asn1CertificateBasicConstraintsExtension : Asn1CertificateExtension
        {
        [Browsable(false)] public Boolean CertificateAuthority { get; }
        public Int32 PathLengthConstraint { get; }
        public X509SubjectType SubjectType { get; }
        public Asn1CertificateBasicConstraintsExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (ReferenceEquals(octet, null)) { throw new ArgumentOutOfRangeException(nameof(source), "Extension should contains [OctetString]"); }
            if (octet.Count == 0)             { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (octet[0] is Asn1Sequence sequence) {
                var c = sequence.Count;
                if (c > 0) {
                    var i = 0;
                    if (sequence[i] is Asn1Boolean) {
                        CertificateAuthority = (Asn1Boolean)sequence[i];
                        i++;
                        }
                    if (i < c)
                        {
                        PathLengthConstraint = (Asn1Integer)sequence[i];
                        }
                    }
                }
            else
                {
                throw new ArgumentOutOfRangeException(nameof(source));
                }
            SubjectType = CertificateAuthority
                ? X509SubjectType.CA
                : X509SubjectType.EndEntity;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Resources.ResourceManager.GetString(nameof(Asn1CertificateBasicConstraintsExtension) + "." + nameof(SubjectType))}={X509SubjectTypeConverter.ToString(SubjectType)}({Resources.ResourceManager.GetString(nameof(Asn1CertificateBasicConstraintsExtension) + "." + nameof(PathLengthConstraint))}={PathLengthConstraint})";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(PathLengthConstraint), PathLengthConstraint);
                writer.WriteValue(serializer, nameof(SubjectType), SubjectType.ToString());
                }
            }
        }
    }
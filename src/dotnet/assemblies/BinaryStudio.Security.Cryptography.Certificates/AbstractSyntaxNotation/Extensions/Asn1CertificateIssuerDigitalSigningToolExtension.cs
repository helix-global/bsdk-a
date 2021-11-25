using System;
using System.Collections.Generic;
using System.Globalization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [Asn1SpecificObject("1.2.643.100.112")]
    internal sealed class Asn1CertificateIssuerDigitalSigningToolExtension : Asn1CertificateExtension
        {
        public String SoftwareUsedToCreateDigitalSignature { get; }
        public String CertificationAuthorityDescriptiveName { get; }
        public String ConformityPropertiesOfSoftwareUsedToCreateDigitalSignature { get; }
        public String ConformityPropertiesOfCertificationAuthority { get; }

        public Asn1CertificateIssuerDigitalSigningToolExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (octet != null) {
                if (octet.Count > 0) {
                    var sequence = octet[0];
                    if (sequence.Count > 0) { SoftwareUsedToCreateDigitalSignature = ((Asn1String)sequence[0]).Value; }
                    if (sequence.Count > 1) { CertificationAuthorityDescriptiveName = ((Asn1String)sequence[1]).Value; }
                    if (sequence.Count > 2) { ConformityPropertiesOfSoftwareUsedToCreateDigitalSignature = ((Asn1String)sequence[2]).Value; }
                    if (sequence.Count > 3) { ConformityPropertiesOfCertificationAuthority = ((Asn1String)sequence[3]).Value; }
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new List<String>();
            if (SoftwareUsedToCreateDigitalSignature != null)  { r.Add($"{Resources.ResourceManager.GetString(nameof(Asn1CertificateIssuerDigitalSigningToolExtension) + "." + nameof(SoftwareUsedToCreateDigitalSignature))}={SoftwareUsedToCreateDigitalSignature}"); }
            if (CertificationAuthorityDescriptiveName != null) { r.Add($"{Resources.ResourceManager.GetString(nameof(Asn1CertificateIssuerDigitalSigningToolExtension) + "." + nameof(CertificationAuthorityDescriptiveName))}={CertificationAuthorityDescriptiveName}"); }
            return String.Join(";", r);
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                if (!String.IsNullOrEmpty(SoftwareUsedToCreateDigitalSignature))                       { writer.WriteValue(serializer, nameof(SoftwareUsedToCreateDigitalSignature), SoftwareUsedToCreateDigitalSignature);                                             }
                if (!String.IsNullOrEmpty(CertificationAuthorityDescriptiveName))                      { writer.WriteValue(serializer, nameof(CertificationAuthorityDescriptiveName), CertificationAuthorityDescriptiveName);                                           }
                if (!String.IsNullOrEmpty(ConformityPropertiesOfSoftwareUsedToCreateDigitalSignature)) { writer.WriteValue(serializer, nameof(ConformityPropertiesOfSoftwareUsedToCreateDigitalSignature), ConformityPropertiesOfSoftwareUsedToCreateDigitalSignature); }
                if (!String.IsNullOrEmpty(ConformityPropertiesOfCertificationAuthority))               { writer.WriteValue(serializer, nameof(ConformityPropertiesOfCertificationAuthority), ConformityPropertiesOfCertificationAuthority);                             }
                }
            }
        }
    }
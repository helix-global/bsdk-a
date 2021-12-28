using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) authorityKeyIdentifier(35)}
     * 2.5.29.35
     * /Joint-ISO-ITU-T/5/29/35
     * Authority key identifier
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.35")]
    public sealed class Asn1CertificateAuthorityKeyIdentifierExtension : Asn1CertificateExtension
        {
        public Byte[] KeyIdentifier { get; }
        public String SerialNumber { get; }
        public IX509GeneralName CertificateIssuer { get; }

        public Asn1CertificateAuthorityKeyIdentifierExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    var contextspecifics = octet[0].Find(i => (i.Class == Asn1ObjectClass.ContextSpecific)).ToArray();
                    #region [0]:KeyIdentifier
                    var specific = contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 0);
                    if (specific != null) {
                        KeyIdentifier = specific.Content.ToArray();
                        }
                    #endregion
                    #region [1]:CertificateIssuer
                    specific = contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 1);
                    if (specific != null) {
                        CertificateIssuer = X509GeneralName.From((Asn1ContextSpecificObject)specific[0]);
                        }
                    #endregion
                    #region [2]:SerialNumber
                    specific = contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 2);
                    if (specific != null) {
                        SerialNumber = String.Join(String.Empty, specific.Content.ToArray().Select(i => i.ToString("X2")));
                        }
                    #endregion
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
            return (KeyIdentifier != null)
                ? KeyIdentifier.ToString()
                : base.ToString();
            }

        protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes)
            {
            foreach (var pi in base.GetProperties(attributes)) {
                     if (pi.Name == nameof(CertificateIssuer)) { if (CertificateIssuer != null) { yield return pi; }}
                else if (pi.Name == nameof(SerialNumber))      { if (!String.IsNullOrWhiteSpace(SerialNumber)) { yield return pi; }}
                else
                    {
                    yield return pi;    
                    }
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, nameof(KeyIdentifier), KeyIdentifier.ToString("X"));
                if (!String.IsNullOrWhiteSpace(SerialNumber)) {
                    writer.WriteValue(serializer, nameof(SerialNumber), SerialNumber);
                    }
                if (CertificateIssuer != null) {
                    writer.WriteValue(serializer, nameof(CertificateIssuer), CertificateIssuer);
                    }
                }
            }
        }
    }
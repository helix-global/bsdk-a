using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) privateKeyUsagePeriod(16)}
     * 2.5.29.16
     * /Joint-ISO-ITU-T/5/29/16
     * Private key usage period
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.16")]
    internal class Asn1CertificatePrivateKeyUsagePeriodExtension : Asn1CertificateExtension
        {
        [Asn1DisplayName(nameof(Asn1CertificatePrivateKeyUsagePeriodExtension) + "." + nameof(NotBefore))][TypeConverter(typeof(Asn1DataTimeConverter))] public DateTime? NotBefore { get; }
        [Asn1DisplayName(nameof(Asn1CertificatePrivateKeyUsagePeriodExtension) + "." + nameof(NotAfter))] [TypeConverter(typeof(Asn1DataTimeConverter))] public DateTime? NotAfter  { get; }

        public Asn1CertificatePrivateKeyUsagePeriodExtension(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    var contextspecifics = octet[0].Find(i => (i.Class == Asn1ObjectClass.ContextSpecific)).ToArray();
                    NotBefore = ToDateTime(contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 0));
                    NotAfter  = ToDateTime(contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 1));
                    }
                }
            }

        private static DateTime? ToDateTime(Asn1Object source) {
            if (source != null) {
                var value = source.Content.ToArray();
                if (value.Length > 0) {
                    var r = new Asn1GeneralTime(value);
                    r.Decode();
                    return r.Value;
                    }
                }
            return null;
            }

        private static String ToString(DateTime? value) {
            return (value != null)
                ? Asn1DataTimeConverter.ToString(value.Value)
                : "*";
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"[{ToString(NotBefore)}]-[{ToString(NotAfter)}]";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                writer.WriteValue(serializer, "NotBefore", NotBefore);
                writer.WriteValue(serializer, "NotAfter",  NotAfter);
                }
            }
        }
    }
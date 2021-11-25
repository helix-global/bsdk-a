using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {joint-iso-itu-t(2) ds(5) certificateExtension(29) keyUsage(15)}
     * 2.5.29.15
     * /Joint-ISO-ITU-T/5/29/15
     * Key usage
     * IETF RFC 5280
     * */
    [Asn1SpecificObject("2.5.29.15")]
    internal sealed class CertificateKeyUsage : Asn1CertificateExtension
        {
        #region P:Value:X509KeyUsageFlags
        [TypeConverter(typeof(X509CertificateKeyUsageTypeConverter))]
        [Asn1DisplayName("9001")]
        public X509KeyUsageFlags KeyUsage { get; }
        #endregion
        public CertificateKeyUsage(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    var bitstring = octet[0] as Asn1BitString;
                    if (!ReferenceEquals(bitstring, null)) {
                        if (bitstring.Content.Length > 0) {
                            bitstring.Content.Seek(0, SeekOrigin.Begin);
                            var i = bitstring.Content.ReadByte();
                            KeyUsage = (X509KeyUsageFlags)i;
                            }
                        }
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
            return X509CertificateKeyUsageTypeConverter.ToString(KeyUsage);
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                var r = new List<String>();
                var n = (UInt32)KeyUsage;
                if ((n & (UInt32)X509KeyUsageFlags.EncipherOnly)     != 0) { r.Add(nameof(X509KeyUsageFlags.EncipherOnly)     ); n &= ~(UInt32)X509KeyUsageFlags.EncipherOnly;     }
                if ((n & (UInt32)X509KeyUsageFlags.CrlSign)          != 0) { r.Add(nameof(X509KeyUsageFlags.CrlSign)          ); n &= ~(UInt32)X509KeyUsageFlags.CrlSign;          }
                if ((n & (UInt32)X509KeyUsageFlags.KeyCertSign)      != 0) { r.Add(nameof(X509KeyUsageFlags.KeyCertSign)      ); n &= ~(UInt32)X509KeyUsageFlags.KeyCertSign;      }
                if ((n & (UInt32)X509KeyUsageFlags.KeyAgreement)     != 0) { r.Add(nameof(X509KeyUsageFlags.KeyAgreement)     ); n &= ~(UInt32)X509KeyUsageFlags.KeyAgreement;     }
                if ((n & (UInt32)X509KeyUsageFlags.DataEncipherment) != 0) { r.Add(nameof(X509KeyUsageFlags.DataEncipherment) ); n &= ~(UInt32)X509KeyUsageFlags.DataEncipherment; }
                if ((n & (UInt32)X509KeyUsageFlags.KeyEncipherment)  != 0) { r.Add(nameof(X509KeyUsageFlags.KeyEncipherment)  ); n &= ~(UInt32)X509KeyUsageFlags.KeyEncipherment;  }
                if ((n & (UInt32)X509KeyUsageFlags.NonRepudiation)   != 0) { r.Add(nameof(X509KeyUsageFlags.NonRepudiation)   ); n &= ~(UInt32)X509KeyUsageFlags.NonRepudiation;   }
                if ((n & (UInt32)X509KeyUsageFlags.DigitalSignature) != 0) { r.Add(nameof(X509KeyUsageFlags.DigitalSignature) ); n &= ~(UInt32)X509KeyUsageFlags.DigitalSignature; }
                if ((n & (UInt32)X509KeyUsageFlags.DecipherOnly)     != 0) { r.Add(nameof(X509KeyUsageFlags.DecipherOnly)     ); n &= ~(UInt32)X509KeyUsageFlags.DecipherOnly;     }
                if (!IsNullOrEmpty(r)) {
                    writer.WriteValue(serializer, nameof(KeyUsage), r);
                    }
                }
            }
        }
    }
using System;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1CertificateRevocationListEntry : Asn1LinkObject
        {
        public String SerialNumber { get; }
        public DateTime RevocationDate { get; }
        public Asn1CertificateExtensionCollection Extensions { get; }
        internal Asn1CertificateRevocationListEntry(Asn1Object source)
            : base(source)
            {
            Extensions = new Asn1CertificateExtensionCollection();
            SerialNumber = String.Join(String.Empty, ((Asn1Integer)source[0]).Value.ToByteArray().Select(i => i.ToString("X2")));
            RevocationDate = (Asn1Time)source[1];
            if (source.Count > 2) {
                Extensions = new Asn1CertificateExtensionCollection(source[2].Select(i => Asn1CertificateExtension.From(new Asn1CertificateExtension(i))).ToList());
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return SerialNumber;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                WriteValue(writer, serializer, nameof(SerialNumber), SerialNumber);
                WriteValue(writer, serializer, nameof(RevocationDate), RevocationDate.ToString("O"));
                var extensions = Extensions;
                if (!IsNullOrEmpty(extensions))
                    {
                    WriteValue(writer, serializer, nameof(Extensions), extensions);
                    }
                }
            }
        }
    }
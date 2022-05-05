using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Xml;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    [DefaultProperty(nameof(CertificateSerialNumber))]
    public class CmsIssuerAndSerialNumber : CmsSignerIdentifier, ICmsIssuerAndSerialNumber
        {
        [TypeConverter(typeof(CmsSerialNumberTypeConverter))] public String CertificateSerialNumber { get; }
        public Asn1RelativeDistinguishedNameSequence CertificateIssuer { get; }
        IX509GeneralName ICmsIssuerAndSerialNumber.CertificateIssuer { get { return CertificateIssuer; }}

        public CmsIssuerAndSerialNumber(Asn1Sequence o)
            : base(o)
            {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            if (o.Count != 2) { throw new ArgumentOutOfRangeException(nameof(o)); }
            CertificateSerialNumber = ((BigInteger)(Asn1Integer)o[1]).
                ToByteArray().Reverse().ToArray().ToString("x");
            CertificateIssuer = Asn1Certificate.Make(o[0]);
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return CertificateSerialNumber;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(CertificateSerialNumber), CertificateSerialNumber);
            writer.WriteValue(serializer, nameof(CertificateIssuer), CertificateIssuer);
            writer.WriteEndObject();
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("CmsIssuerAndSerialNumber");
            writer.WriteAttributeString(nameof(CertificateSerialNumber),CertificateSerialNumber);
            if (CertificateIssuer != null) {
                writer.WriteStartElement("CmsIssuerAndSerialNumber.CertificateIssuer");
                CertificateIssuer.WriteXml(writer);
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }
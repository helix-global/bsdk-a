using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    [DefaultProperty(nameof(CertificateSerialNumber))]
    public class CmsIssuerAndSerialNumber : CmsSignerIdentifier, ICmsIssuerAndSerialNumber
        {
        [TypeConverter(typeof(CmsSerialNumberTypeConverter))] public BigInteger CertificateSerialNumber { get; }
        public Asn1RelativeDistinguishedNameSequence CertificateIssuer { get; }
        IX509GeneralName ICmsIssuerAndSerialNumber.CertificateIssuer { get { return CertificateIssuer; }}

        public CmsIssuerAndSerialNumber(Asn1Sequence o)
            : base(o)
            {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            if (o.Count != 2) { throw new ArgumentOutOfRangeException(nameof(o)); }
            CertificateSerialNumber = (Asn1Integer)o[1];
            CertificateIssuer = Asn1Certificate.Make(o[0]);
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return String.Join(String.Empty, CertificateSerialNumber.
                ToByteArray().
                Reverse().
                Select(i => i.ToString("X2")));
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(CertificateSerialNumber), CertificateSerialNumber.ToByteArray().Reverse().ToArray().ToString("X"));
            writer.WriteValue(serializer, nameof(CertificateIssuer), CertificateIssuer);
            writer.WriteEndObject();
            }
        }
    }
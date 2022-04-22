﻿using System;
using System.Linq;
using System.Numerics;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsIssuerSerialSequence : Asn1LinkObject<Asn1Sequence>, ICmsIssuerAndSerialNumber
        {
        public CmsIssuerSerialSequence(Asn1Sequence source)
            : base(source)
            {
            CertificateSerialNumber = ((BigInteger)(Asn1Integer)source[1]).ToByteArray().Reverse().ToArray().ToString("x");
            CertificateIssuer = X509GeneralName.From((Asn1ContextSpecificObject)source[0][0]);
            }

        public String CertificateSerialNumber { get; }
        public IX509GeneralName CertificateIssuer { get; }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(CertificateSerialNumber), CertificateSerialNumber);
            writer.WriteValue(serializer, nameof(CertificateIssuer), CertificateIssuer);
            writer.WriteEndObject();
            }
        }
    }
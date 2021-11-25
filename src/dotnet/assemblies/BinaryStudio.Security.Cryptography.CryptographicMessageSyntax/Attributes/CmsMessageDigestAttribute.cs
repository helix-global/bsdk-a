﻿using System;
using System.Linq;
using BinaryStudio.IO;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) 4}
     * {1.2.840.113549.1.9.4}
     *
     * MessageDigest ::= OCTET STRING
     */
    [CmsSpecific("1.2.840.113549.1.9.4")]
    public class CmsMessageDigestAttribute : CmsAttribute
        {
        public Byte[] MessageDigest { get; }
        protected CmsMessageDigestAttribute(CmsAttribute o)
            : base(o)
            {
            MessageDigest = new Byte[0];
            MessageDigest = Values.FirstOrDefault()?.Content.ToArray();
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteValue(serializer, nameof(MessageDigest), MessageDigest.ToString("X"));
            writer.WriteMultilineHexComment(MessageDigest);
            }
        }
    }
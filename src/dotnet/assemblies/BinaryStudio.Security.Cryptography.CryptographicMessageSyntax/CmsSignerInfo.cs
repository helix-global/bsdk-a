using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * [RFC5652]
     * SignerInfo ::= SEQUENCE
     * {
     *   version            CMSVersion,
     *   sid                SignerIdentifier,
     *   digestAlgorithm    DigestAlgorithmIdentifier,
     *   signedAttrs        [0] IMPLICIT SignedAttributes OPTIONAL,
     *   signatureAlgorithm SignatureAlgorithmIdentifier,
     *   signature          SignatureValue,
     *   unsignedAttrs      [1] IMPLICIT UnsignedAttributes OPTIONAL
     * }
     *
     * SignerIdentifier ::= CHOICE
     * {
     *   issuerAndSerialNumber IssuerAndSerialNumber,
     *   subjectKeyIdentifier [0] SubjectKeyIdentifier
     * }
     *
     * SignedAttributes   ::= SET SIZE (1..MAX) OF Attribute
     * UnsignedAttributes ::= SET SIZE (1..MAX) OF Attribute
     *
     * Attribute ::= SEQUENCE
     * {
     *   attrType   OBJECT IDENTIFIER,
     *   attrValues SET OF AttributeValue
     * }
     *
     * AttributeValue ::= ANY
     * SignatureValue ::= OCTET STRING
     */
    public class CmsSignerInfo : CmsObject
        {
        public Int32 Version { get; }
        public CmsSignerIdentifier SignerIdentifier { get; }
        public X509AlgorithmIdentifier DigestAlgorithm { get; }
        public X509AlgorithmIdentifier SignatureAlgorithm { get; }
        public ISet<CmsAttribute> SignedAttributes { get; }
        public ISet<CmsAttribute> UnsignedAttributes { get; }
        public Asn1OctetString SignatureValue { get; }

        public CmsSignerInfo(Asn1Object o)
            :base(o)
            {
            SignedAttributes   = new HashSet<CmsAttribute>();
            UnsignedAttributes = new HashSet<CmsAttribute>();
            if (o is Asn1Sequence u) {
                var c = u.Count;
                Version = (Asn1Integer)u[0];
                SignerIdentifier = CmsSignerIdentifier.Choice(u[1]);
                DigestAlgorithm = new X509AlgorithmIdentifier((Asn1Sequence)u[2]);
                var i = 3;
                if (u[i] is Asn1ContextSpecificObject contextspecific) {
                    SignedAttributes.UnionWith(u[i].Select(j => CmsAttribute.From(new CmsAttribute(j))));
                    i++;
                    }
                SignatureAlgorithm = new X509AlgorithmIdentifier((Asn1Sequence)u[i++]);
                SignatureValue = (Asn1OctetString)u[i++];
                if (i < c) {
                    SignedAttributes.UnionWith(u[i].Select(j => CmsAttribute.From(new CmsAttribute(j))));
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return SignerIdentifier.ToString();
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(Version), Version);
            writer.WriteValue(serializer, nameof(SignerIdentifier), SignerIdentifier);
            writer.WriteValue(serializer, nameof(DigestAlgorithm), DigestAlgorithm);
            writer.WriteValue(serializer, nameof(SignatureAlgorithm), SignatureAlgorithm);
            SignatureValue.Content.Seek(0, SeekOrigin.Begin);
            writer.WriteMultilineHexComment(SignatureValue.Content);
            writer.WriteBase32PropertyValue(nameof(SignatureValue), SignatureValue.Content.ToArray());
            //writer.WriteValue(serializer, nameof(SignatureValue), SignatureValue.Content.ToArray().ToString("X"));
            #region SignedAttributes
            if (!IsNullOrEmpty(SignedAttributes)) {
                writer.WritePropertyName(nameof(SignedAttributes));
                writer.WriteStartArray();
                foreach (var attribute in SignedAttributes)
                    {
                    attribute.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            #endregion
            #region UnsignedAttributes
            if (!IsNullOrEmpty(UnsignedAttributes)) {
                writer.WritePropertyName(nameof(UnsignedAttributes));
                writer.WriteStartArray();
                foreach (var attribute in UnsignedAttributes)
                    {
                    attribute.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            #endregion
            writer.WriteEndObject();
            }
        }
    }
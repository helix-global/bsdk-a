using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BinaryStudio.DataProcessing;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
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
    [TypeConverter(typeof(ObjectTypeConverter))]
    public class CmsSignerInfo : CmsObject
        {
        private const Int32 ORDER_VERSION           = 0;
        private const Int32 ORDER_SIGNER_IDENTIFIER = 1;
        private const Int32 ORDER_DIGEST_ALGORITHM  = 2;
        private const Int32 ORDER_SIGNED_ATTRIBUTES = 3;

        [Order(1)] public Int32 Version { get; }
        [Order(2)] public CmsSignerIdentifier SignerIdentifier { get; }
        [Order(3)] public X509AlgorithmIdentifier DigestAlgorithm { get; }
        [Order(4)] public X509AlgorithmIdentifier SignatureAlgorithm { get; }
        [Order(5)] [TypeConverter(typeof(CmsAttributeCollectionTypeConverter))] public ISet<CmsAttribute> SignedAttributes   { get; }
        [Order(6)] [TypeConverter(typeof(CmsAttributeCollectionTypeConverter))] public ISet<CmsAttribute> UnsignedAttributes { get; }
        [Order(7)] public Asn1OctetString SignatureValue { get; }

        public CmsSignerInfo(Asn1Object o)
            :base(o)
            {
            SignedAttributes   = new HashSet<CmsAttribute>();
            UnsignedAttributes = new HashSet<CmsAttribute>();
            if (o is Asn1Sequence u) {
                var c = u.Count;
                Version = (Asn1Integer)u[ORDER_VERSION];
                SignerIdentifier = CmsSignerIdentifier.Choice(u[ORDER_SIGNER_IDENTIFIER]);
                DigestAlgorithm = new X509AlgorithmIdentifier((Asn1Sequence)u[ORDER_DIGEST_ALGORITHM]);
                var i = ORDER_SIGNED_ATTRIBUTES;
                if (u[i] is Asn1ContextSpecificObject contextspecific) {
                    SignedAttributesContainer = contextspecific;
                    SignedAttributes.UnionWith(u[i].Select(j => CmsAttribute.From(new CmsAttribute(j))));
                    i++;
                    }
                var SignatureAlgorithmSource = u[i++] as Asn1Sequence;
                if (SignatureAlgorithmSource != null) {
                    SignatureAlgorithm = new X509AlgorithmIdentifier(SignatureAlgorithmSource);
                    SignatureValue = (Asn1OctetString)u[i++];
                    if (i < c) {
                        SignedAttributes.UnionWith(u[i].Select(j => CmsAttribute.From(new CmsAttribute(j))));
                        }
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
            if (SignatureValue != null) {
                SignatureValue.Content.Seek(0, SeekOrigin.Begin);
                writer.WriteMultilineHexComment(SignatureValue.Content);
                writer.WriteBase32PropertyValue(nameof(SignatureValue), SignatureValue.Content.ToArray());
                //writer.WriteValue(serializer, nameof(SignatureValue), SignatureValue.Content.ToArray().ToString("X"));
                }
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

        private IEnumerable<Byte[]> DigestValueSourceSequence { get {
            if (SignedAttributesContainer == null) { throw new NotImplementedException(); }
            if (SignedAttributesContainer != null) {
                #region {header as SET}
                foreach (var i in HeaderSequence(
                    SignedAttributesContainer.IsExplicitConstructed,
                    Asn1ObjectClass.Universal, Asn1ObjectType.Set,
                    SignedAttributesContainer.IsIndefiniteLength
                        ? -1
                        : SignedAttributesContainer.Length))
                    {
                    yield return i;
                    }
                #endregion
                #region {content}
                foreach (var i in SignedAttributesContainer.ContentSequence)
                    {
                    yield return i;
                    }
                #endregion
                }
            }}

        public Stream DigestValueSource { get {
            return new ReadOnlyBlockSequenceStream(DigestValueSourceSequence);
            }}

        private readonly Asn1ContextSpecificObject SignedAttributesContainer;
        }
    }
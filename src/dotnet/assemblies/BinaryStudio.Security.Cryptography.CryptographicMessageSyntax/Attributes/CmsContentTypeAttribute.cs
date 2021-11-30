using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) 3}
     * {1.2.840.113549.1.9.3}
     * [RFC5652]
     *
     * ContentType ::= OBJECT IDENTIFIER
     *
     */
    [CmsSpecific("1.2.840.113549.1.9.3")]
    [DefaultProperty(nameof(ContentType))]
    public sealed class CmsContentTypeAttribute : CmsAttribute
        {
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))] public Oid ContentType { get; }
        [Browsable(false)] public override Object Value { get { return base.Value; }}

        internal CmsContentTypeAttribute(CmsAttribute o)
            : base(o)
            {
            ContentType = new Oid(Values.FirstOrDefault(i => i is Asn1ObjectIdentifier).ToString());
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteValue(serializer, nameof(ContentType), ContentType.Value);
            }
        }
    }
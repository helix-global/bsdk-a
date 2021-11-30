using System;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /**
     * {iso(1) member-body(2) us(840) rsadsi(113549) pkcs(1) pkcs9(9) 5}
     * {1.2.840.113549.1.9.5}
     *
     * SigningTime ::= Time
     * Time ::= CHOICE
     * {
     *   utcTime UTCTime,
     *   generalizedTime GeneralizedTime
     * }
     */
    [CmsSpecific("1.2.840.113549.1.9.5")]
    [DefaultProperty(nameof(SigningTime))]
    public class CmsSigningTimeAttribute : CmsAttribute
        {
        public DateTime SigningTime { get; }
        [Browsable(false)] public override Object Value { get { return base.Value; }}
        internal CmsSigningTimeAttribute(CmsAttribute o)
            : base(o)
            {
            var r = (Asn1Time)Values.FirstOrDefault();
            if (r != null) {
                SigningTime = r;
                }
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteValue(serializer, nameof(SigningTime), SigningTime);
            }
        }
    }
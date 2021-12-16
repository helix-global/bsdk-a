using System;
using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    [DefaultProperty(nameof(Type))]
    internal abstract class ECFieldIdentifier : Asn1LinkObject
        {
        [Order(1)][DisplayName("{Type}")] public Asn1ObjectIdentifier Type { get; }
        [Order(2)] public virtual Object Parameters { get; }
        [Browsable(false)] public override Byte[] Body { get{return base.Body; }}

        protected internal ECFieldIdentifier(Asn1ObjectIdentifier type, Asn1Object parameters)
            : base(parameters)
            {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            Type = type;
            Parameters = parameters;
            }

        public static ECFieldIdentifier Create(Asn1Object source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Count != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (source is Asn1Sequence sequence) {
                var type = sequence[0] as Asn1ObjectIdentifier;
                if (type == null) { throw new ArgumentOutOfRangeException(nameof(source), "ASN.1 {OBJECT IDENTIFIER} missing."); }
                switch (type.ToString()) {
                    case "1.2.840.10045.1.1":
                        {
                        return new ECPrimeField(type, sequence[1]);
                        }
                    default: throw new NotSupportedException($"Field type {{{type}}} is not supported.");
                    }
                }
            throw new ArgumentOutOfRangeException(nameof(source));
            }
        }
    }
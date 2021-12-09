using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using BinaryStudio.DataProcessing;
using BinaryStudio.Numeric;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public sealed class ECCurve : Asn1LinkObject
        {
        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}
        public BigInteger A { get; }
        public BigInteger B { get; }
        [TypeConverter(typeof(X509ByteArrayTypeConverter))] public Object Seed { get; }

        private static BigInteger ToBigInteger(Byte[] source) {
            var c = source.Length;
            if ((source[0] & 0x80) == 0x80) {
                var target = new Byte[c + 1];
                var j = c - 1;
                var i = 0;
                for (; (i < c); i++,j--) {
                    target[j] = source[i];
                    }
                return new BigInteger(target);
                }
            else
                {
                var j = c - 1;
                var i = 0;
                for (; (i < c) && (i < j); i++,j--) {
                    source[j] ^= source[i];
                    source[i] ^= source[j];
                    source[j] ^= source[i];
                    }
                }
            return new BigInteger(source);
            }

        internal ECCurve(Asn1Object source, ECFieldIdentifier field)
            : base(source)
            {
            State |= ObjectState.Failed;
            if (source is Asn1Sequence sequence) {
                if (sequence.Count == 3) {
                    A = ToBigInteger(((Asn1OctetString)sequence[0]).Content.ToArray());
                    B = ToBigInteger(((Asn1OctetString)sequence[1]).Content.ToArray());
                    Seed = sequence[2];
                    switch(field.Type.ToString()) {
                        case "1.2.840.10045.1.1":
                            {
                            Update((ECPrimeField)field);
                            }
                            break;
                        default: throw new NotSupportedException($"Field type {{{field.Type}}} is not supported.");
                        }
                    State &= ~ObjectState.Failed;
                    }
                }
            }

        private static BigInteger? CalculateResidue(BigInteger source, Int32 bits) {
            if (bits >= 96) {
                var r = source >> (bits - 64);
                if (r == -1) {
                    
                    }
                }
            return null;
            }

        private void Update(ECPrimeField field) {
            if (field == null) { throw new ArgumentNullException(nameof(field)); }
            var r = CalculateResidue(
                field.PrimeField,
                field.BitLength);
            }
        }
    }
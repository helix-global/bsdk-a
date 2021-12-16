using System;
using System.ComponentModel;
using System.Numerics;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    internal class ECPrimeField : ECFieldIdentifier
        {
        [Order(2)][DisplayName("prime-field")] public BigInteger PrimeField { get; }
        [Browsable(false)] public override Object Parameters { get { return PrimeField; }}
        public Int32 BitLength { get; }

        internal ECPrimeField(Asn1ObjectIdentifier type, Asn1Object parameters)
            : base(type, parameters)
            {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (parameters is Asn1Integer source) {
                PrimeField = source.Value;
                BitLength = GetBitLength(PrimeField);
                return;
                }
            throw new ArgumentOutOfRangeException(nameof(parameters));
            }

        private static readonly Byte[] BitLengthTable =
            {
            0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4,
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8
            };

        private static Int32 GetBitCount(BigInteger source) {
            var sign = source.Sign;
            if (sign > 0) {
                var r = 0;
                var content = source.ToByteArray();
                var c = content.Length;
                var i = sign;
                for (;i < c; i++) {
                    if (content[i] != 0) { break; }
                    }
                for (;i < c;i++) {
                    if (c != 0) {
                        r += BitLengthTable[content[i]];
                        }
                    }
                return r;
                }
            return 0;
            }

        private static Int32 GetBitLength(BigInteger source) {
            var sign = source.Sign;
            if (sign > 0) {
                var content = source.ToByteArray();
                var c = content.Length;
                return (c - sign)*8;
                }
            return 0;
            }
        }
    }
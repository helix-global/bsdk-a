using System;
using System.ComponentModel;
using System.Numerics;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Asn1SpecificObject("1.2.840.10045.2.1")]
    internal sealed class ECPKParameters : X509PublicKeyParameters
        {
        private const Int32 INDEX_VERSION                   = 0;
        private const Int32 INDEX_FINITE_FIELD              = 1;
        private const Int32 INDEX_CURVE                     = 2;
        private const Int32 INDEX_BASE                      = 3;
        private const Int32 INDEX_ORDER                     = 4;
        private const Int32 INDEX_FACTOR                    = 5;

        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}
        [Order(1)] public Int32 Version { get; }
        [Order(2)] public ECFieldIdentifier FieldIdentifier { get; }
        [Order(3)] public ECCurve Curve { get; }
        [Order(4)][Superscript("SUPERSCRIPT")][Subscript("SUBSCRIPT")] public ECPoint BasePoint { get; }
        [Order(5)][Superscript("Superscript")][Subscript("Subscript")] public BigInteger  Order  { get; }
        [Order(6)][Superscript("superscript")][Subscript("subscript")] public BigInteger? Factor { get; }

        internal ECPKParameters(Asn1Object source)
            : base(source)
            {
            State |= ObjectState.Failed;
            if (source is Asn1Sequence sequence) {
                var c = sequence.Count - 1;
                Version = (Asn1Integer)sequence[INDEX_VERSION];
                FieldIdentifier = ECFieldIdentifier.Create(sequence[INDEX_FINITE_FIELD]);
                Curve = new ECCurve(sequence[INDEX_CURVE],FieldIdentifier);
                BasePoint = new ECPoint(sequence[INDEX_BASE]);
                Order = (Asn1Integer)sequence[INDEX_ORDER];
                if (c > INDEX_ORDER) {
                    Factor = (Asn1Integer)sequence[INDEX_FACTOR];
                    }
                State &= ~ObjectState.Failed;
                }
            }
        }
    }
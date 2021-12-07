using System;
using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public sealed class ECCurve : Asn1LinkObject
        {
        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}
        public Object A { get; }
        public Object B { get; }
        public Object Seed { get; }

        internal ECCurve(Asn1Object source)
            : base(source)
            {
            State |= ObjectState.Failed;
            if (source is Asn1Sequence sequence) {
                if (sequence.Count == 3) {
                    A = sequence[0];
                    B = sequence[1];
                    Seed = sequence[2];
                    State &= ~ObjectState.Failed;    
                    }
                }
            }
        }
    }
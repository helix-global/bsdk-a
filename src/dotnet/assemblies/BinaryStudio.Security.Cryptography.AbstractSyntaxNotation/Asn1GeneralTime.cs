using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="GENERALIZEDTIME"/> type.
    /// </summary>
    public sealed class Asn1GeneralTime : Asn1Time
        {
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.GeneralTime"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.GeneralTime; }}
        public override DateTimeKind Kind { get { return DateTimeKind.Local; }}

        internal Asn1GeneralTime(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        public Asn1GeneralTime(Byte[] source)
            :base(source)
            {
            Value = Parse(Encoding.ASCII.GetString(source), Asn1ObjectType.GeneralTime).GetValueOrDefault();
            State |= ObjectState.Decoded;
            }

        public Asn1GeneralTime(String source)
            :base(new Byte[0])
            {
            Value = Parse(source, Asn1ObjectType.GeneralTime).GetValueOrDefault();
            State |= ObjectState.Decoded;
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return false; }
            var r = new Byte[Length];
            Content.Seek(0, SeekOrigin.Begin);
            Content.Read(r, 0, r.Length);
            Value = Parse(Encoding.ASCII.GetString(r), Asn1ObjectType.GeneralTime).GetValueOrDefault();
            State |= ObjectState.Decoded;
            return true;
            }
        }
    }

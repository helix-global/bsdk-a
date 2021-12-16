using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="UTCTIME"/> type.
    /// </summary>
    internal class Asn1UtcTime : Asn1Time
        {
        public override DateTimeKind Kind { get { return DateTimeKind.Utc; }}
        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.UtcTime"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.UtcTime; }}

        internal Asn1UtcTime(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return false; }
            var r = new Byte[Length];
            Content.Read(r, 0, r.Length);
            var strvalue = Encoding.ASCII.GetString(r);
            if (Regex.IsMatch(strvalue, "^\\d{12}Z")) {
                Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,12), "yyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Utc);
                if (Value.Year < 2000) {
                    Value = Value.AddYears(100);
                    }
                }
            State |= ObjectState.Decoded;
            return true;
            }
        }
    }

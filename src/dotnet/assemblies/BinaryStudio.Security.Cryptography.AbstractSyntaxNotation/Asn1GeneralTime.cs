using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1GeneralTime : Asn1Time
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.GeneralTime; }}
        public override DateTimeKind Kind { get { return DateTimeKind.Local; }}

        public Asn1GeneralTime(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        internal Asn1GeneralTime(Byte[] source)
            :base(source)
            {
            var strvalue = Encoding.ASCII.GetString(source);
            if (Regex.IsMatch(strvalue, "^\\d{14}Z")) {
                Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,14), "yyyyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Utc);
                }
            State |= ObjectState.Decoded;
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return false; }
            var r = new Byte[Length];
            Content.Seek(0, SeekOrigin.Begin);
            Content.Read(r, 0, r.Length);
            var strvalue = Encoding.ASCII.GetString(r);
            if (Regex.IsMatch(strvalue, "^\\d{14}Z")) {
                Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,14), "yyyyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Utc);
                }
            State |= ObjectState.Decoded;
            return true;
            }
        }
    }

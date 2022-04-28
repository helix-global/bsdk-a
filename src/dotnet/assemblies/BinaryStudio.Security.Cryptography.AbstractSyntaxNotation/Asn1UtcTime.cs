using System;
using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="UTCTIME"/> type.
    /// </summary>
    public sealed class Asn1UtcTime : Asn1Time
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

        public Asn1UtcTime(Byte[] source)
            :base(source)
            {
            Value = Parse(Encoding.ASCII.GetString(source), Asn1ObjectType.UtcTime).GetValueOrDefault();
            State |= ObjectState.Decoded;
            }

        public Asn1UtcTime(String source)
            :base(new Byte[0])
            {
            Value = Parse(source, Asn1ObjectType.UtcTime).GetValueOrDefault();
            State |= ObjectState.Decoded;
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return false; }
            var r = new Byte[Length];
            Content.Read(r, 0, r.Length);
            Value = Parse(Encoding.ASCII.GetString(r), Asn1ObjectType.UtcTime).GetValueOrDefault();
            //var strvalue = Encoding.ASCII.GetString(r);
            //if (Regex.IsMatch(strvalue, "^\\d{12}Z")) {
            //    Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,12), "yyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Utc);
            //    if (Value.Year < 2000) {
            //        Value = Value.AddYears(100);
            //        }
            //    }
            //else if (Regex.IsMatch(strvalue, "^\\d{10}Z")) {
            //    Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,10), "yyMMddHHmm", CultureInfo.CurrentCulture), DateTimeKind.Utc);
            //    if (Value.Year < 2000) {
            //        Value = Value.AddYears(100);
            //        }
            //    }
            //else if (Regex.IsMatch(strvalue, "^\\d{12}[+]\\d{4}$")) {
            //    var offsetvalue = strvalue.Substring(13,4);
            //    Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,12), "yyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Unspecified);
            //    if (Value.Year < 2000) {
            //        Value = Value.AddYears(100);
            //        }
            //    var offset = new TimeSpan(
            //        Int32.Parse(offsetvalue.Substring(0,2)),
            //        Int32.Parse(offsetvalue.Substring(2,2)),0);
            //    Value = (new DateTimeOffset(Value, offset)).UtcDateTime;
            //    }
            //else if (Regex.IsMatch(strvalue, "^\\d{12}[-]\\d{4}$")) {
            //    var offsetvalue = strvalue.Substring(13,4);
            //    Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,12), "yyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Unspecified);
            //    if (Value.Year < 2000) {
            //        Value = Value.AddYears(100);
            //        }
            //    var offset = new TimeSpan(
            //        Int32.Parse(offsetvalue.Substring(0,2)),
            //        Int32.Parse(offsetvalue.Substring(2,2)),0);
            //    Value = (new DateTimeOffset(Value, offset)).UtcDateTime;
            //    }
            //else if (Regex.IsMatch(strvalue, "^\\d{12}[-]\\d{4}$")) {
            //    var offsetvalue = strvalue.Substring(13,4);
            //    Value = DateTime.SpecifyKind(DateTime.ParseExact(strvalue.Substring(0,12), "yyMMddHHmmss", CultureInfo.CurrentCulture), DateTimeKind.Unspecified);
            //    if (Value.Year < 2000) {
            //        Value = Value.AddYears(100);
            //        }
            //    var offset = new TimeSpan(
            //        Int32.Parse(offsetvalue.Substring(0,2)),
            //        Int32.Parse(offsetvalue.Substring(2,2)),0);
            //    Value = (new DateTimeOffset(Value, offset)).UtcDateTime;
            //    }
            //else
            //    {
            //    throw (new InvalidDataException()).Add("UtcTime:", strvalue);
            //    }
            State |= ObjectState.Decoded;
            return true;
            }
        }
    }

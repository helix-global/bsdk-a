using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a date-time types:
    /// <table style="font-family: Consolas;width:100%;border-collapse:collapSe;border:none;mso-border-alt:solid windowtext .5pt;mso-yfti-tbllook:1184;mso-padding-alt:0cm 5.4pt 0cm 5.4pt; background-color: white;">
    ///   <tr>
    ///     <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;">
    ///       <see langword="UTCTIME"/>
    ///     </td>
    ///     <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;">
    ///       <see cref="Asn1UtcTime"/>
    ///     </td>
    ///   </tr>
    ///   <tr>
    ///     <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;">
    ///       <see langword="GENERALIZEDTIME"/>
    ///     </td>
    ///     <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm 5.4pt 0cm 5.4pt;">
    ///       <see cref="Asn1GeneralTime"/>
    ///     </td>
    ///   </tr>
    /// </table>
    /// </summary>
    public abstract class Asn1Time : Asn1UniversalObject
        {
        public abstract DateTimeKind Kind { get; }
        public DateTimeOffset Value { get;protected set; }

        internal Asn1Time(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        protected Asn1Time(Byte[] source) {}

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override String ToString()
            {
            var Toffset = Value.Offset;
            var Boffset = TimeZoneInfo.Local.BaseUtcOffset;
            if (Toffset == Boffset) {
                var LocalTime = Value.LocalDateTime;
                return (LocalTime.Millisecond > 0)
                    ? LocalTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")
                    : LocalTime.ToString("yyyy-MM-ddTHH:mm:ss");
                }
            if (Toffset == TimeSpan.Zero) {
                var LocalTime = Value.LocalDateTime;
                return (LocalTime.Millisecond > 0)
                    ? LocalTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ")
                    : LocalTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }
            else
                {
                var LocalTime = Value.LocalDateTime;
                var TimeZone = (Toffset > TimeSpan.Zero)
                    ? ("+" + Toffset.ToString(@"hh\:mm"))
                    : ("-" + Toffset.ToString(@"hh\:mm"));
                return String.Format("{0}{1}",
                    (LocalTime.Millisecond > 0)
                        ? LocalTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")
                        : LocalTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TimeZone);
                }
            }

        public static implicit operator DateTime(Asn1Time source)
            {
            return source.Value.LocalDateTime;
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            WriteValue(writer, serializer, nameof(Value), Value.ToString("O"));
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
            {
            writer.WriteStartElement("Object");
            writer.WriteAttributeString(nameof(Class), Class.ToString());
            if (Offset >= 0) { writer.WriteAttributeString(nameof(Offset), Offset.ToString()); }
            writer.WriteAttributeString("Type", Type.ToString());
            writer.WriteString(Value.ToString("o"));
            writer.WriteEndElement();
            }

        private static String ToString(Group value) {
            return value.Success
                ? value.Value
                : null;
            }

        //protected static String ToString(DateTimeOffset value) {
        //    if (value.Kind == DateTimeKind.Utc) {
        //        var offset = (new DateTimeOffset(value)).Offset;
        //        return String.Format("{0}{1}",
        //            ToString(DateTime.SpecifyKind(value, DateTimeKind.Unspecified)),
        //            offset.ToString(@"hh\:mm"));
        //        }
        //    return (value.Millisecond > 0)
        //        ? value.ToString("o")
        //        : value.ToString("s");
        //    }

        protected static DateTimeOffset? Parse(String value, Asn1ObjectType type) {
            if (String.IsNullOrWhiteSpace(value)) { return null; }
            if (type == Asn1ObjectType.UtcTime)
                {
                /*
                 * yyMMddHH[mm[ss[.fff]]]Z
                 * yyMMddHH[mm[ss[.fff]]]
                 * yyMMddHH[mm[ss[.fff]]]+-HHmm
                 */
                var r = Regex.Match(value, @"^(\d{8})(?:(\d{2})(?:(\d{2})?(?:[.,](\d+))?)?)?(Z|(?:[+-]\d{4})|)$");
                if (r.Success) {
                    var gt = ToString(r.Groups[1]);
                    var mm = ToString(r.Groups[2]) ?? "00";
                    var ss = ToString(r.Groups[3]) ?? "00";
                    var ff = ToString(r.Groups[4]) ?? "0";
                    var tz = ToString(r.Groups[5]) ?? "Z";
                    var o = DateTime.SpecifyKind(DateTime.ParseExact(gt, "yyMMddHH", CultureInfo.CurrentCulture), DateTimeKind.Unspecified);
                    if (o.Year < 2000) { o = o.AddYears(100); }
                    o = o.Add(TimeSpan.Parse($"00:{mm}:{ss}.{ff}"));
                    if ((tz != String.Empty) && (tz != "Z")) {
                        var timespan = new TimeSpan(
                            Int32.Parse(tz.Substring(1,2)),
                            Int32.Parse(tz.Substring(3,2)),0);
                        return (new DateTimeOffset(DateTime.SpecifyKind(o,DateTimeKind.Unspecified))).ToOffset(
                            (tz[0] == '-')
                            ? -timespan
                            : +timespan);
                        }
                    return (new DateTimeOffset(DateTime.SpecifyKind(o,DateTimeKind.Unspecified))).ToOffset(TimeSpan.Zero);
                    }
                }
            else
                {
                /*
                 * yyyyMMddHH[mm[ss[.fff]]]Z
                 * yyyyMMddHH[mm[ss[.fff]]]
                 * yyyyMMddHH[mm[ss[.fff]]]+-HHmm
                 */
                var r = Regex.Match(value, @"^(\d{10})(?:(\d{2})(?:(\d{2})?(?:[.,](\d+))?)?)?(Z|(?:[+-]\d{4})|)$");
                if (r.Success) {
                    var gt = ToString(r.Groups[1]);
                    var mm = ToString(r.Groups[2]) ?? "00";
                    var ss = ToString(r.Groups[3]) ?? "00";
                    var ff = ToString(r.Groups[4]) ?? "0";
                    var tz = ToString(r.Groups[5]);
                    var o = DateTime.SpecifyKind(DateTime.ParseExact(gt, "yyyyMMddHH", CultureInfo.CurrentCulture), DateTimeKind.Local);
                    o = o.Add(TimeSpan.Parse($"00:{mm}:{ss}.{ff}"));
                    if ((tz != String.Empty) && (tz != "Z")) {
                        var timespan = new TimeSpan(
                            Int32.Parse(tz.Substring(1,2)),
                            Int32.Parse(tz.Substring(3,2)),0);
                        return (new DateTimeOffset(DateTime.SpecifyKind(o,DateTimeKind.Unspecified))).ToOffset(
                            (tz[0] == '-')
                            ? -timespan
                            : +timespan);
                        }
                    if (tz == "Z")
                        {
                        return (new DateTimeOffset(DateTime.SpecifyKind(o,DateTimeKind.Unspecified))).ToOffset(TimeSpan.Zero);
                        }
                    return new DateTimeOffset(o).ToOffset(TimeZoneInfo.Local.BaseUtcOffset);
                    }
                }
            return null;
            }
        }
    }

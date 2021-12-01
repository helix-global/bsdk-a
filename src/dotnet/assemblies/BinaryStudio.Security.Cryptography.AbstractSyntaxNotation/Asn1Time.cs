using System;
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
        public DateTime Value { get;protected set; }

        protected Asn1Time(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        protected Asn1Time(Byte[] source) {}

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Value.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            }

        public static implicit operator DateTime(Asn1Time source)
            {
            return source.Value;
            }

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            WriteValue(writer, serializer, nameof(Value), Value.ToString("O"));
            }
        }
    }

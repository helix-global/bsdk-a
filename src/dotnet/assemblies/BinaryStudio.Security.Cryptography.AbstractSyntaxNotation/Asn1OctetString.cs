using System;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="OCTET STRING"/> type.
    /// </summary>
    public sealed class Asn1OctetString : Asn1UniversalObject
        {
        internal Asn1OctetString(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.OctetString"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.OctetString; }}

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),  Type.ToString());
            var c = Count;
            if (c > 0)
                {
                writer.WritePropertyName("(Self)");
                writer.WriteStartArray();
                foreach (var item in this)
                    {
                    item.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            else
                {
                var array = Content.ToArray();
                writer.WritePropertyName("Content");
                if (array.Length > 50)
                    {
                    writer.WriteRaw(" \"");
                    foreach (var value in Convert.ToBase64String(Content.ToArray(), Base64FormattingOptions.InsertLineBreaks).Split('\n')) {
                        //writer.WriteIndent();
                        writer.WriteRaw($"         {value}");
                        }
                    writer.WriteRawValue("\"");
                    }
                else
                    {
                    writer.WriteValue(array);
                    }
                }
            writer.WriteEndObject();
            }
        }
    }

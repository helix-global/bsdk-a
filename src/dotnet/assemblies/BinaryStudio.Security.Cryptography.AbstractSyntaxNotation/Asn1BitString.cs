using System;
using System.IO;
using System.Xml;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="BIT STRING"/> type.
    /// </summary>
    public sealed class Asn1BitString : Asn1UniversalObject
        {
        internal Asn1BitString(ReadOnlyMappingStream source, long forceoffset)
            :base(source, forceoffset)
            {
            }

        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.BitString"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.BitString; }}
        public Int32 UnusedBits { get; private set; }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return base.Decode(); }
            if (Length == 0) {
                State |= ObjectState.Failed;
                return false;
                }
            UnusedBits = Content.ReadByte();
            content = Content.Clone(Length - 1);
            length = Length - 1;
            base.Decode();
            State |= ObjectState.Decoded;
            State |= ObjectState.DisposeContent;
            return true;
            }

        protected override void WriteContent(Stream target)
            {
            base.WriteContent(target);
            }

        protected override void WriteHeader(Stream target)
            {
            target.WriteByte((Byte)UnusedBits);
            base.WriteContent(target);
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),  Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            WriteValue(writer, serializer, nameof(UnusedBits),  UnusedBits.ToString());
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

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        public override void WriteXml(XmlWriter writer)
            {
            base.WriteXml(writer);
            }
        }
    }

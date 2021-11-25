using System;
using System.IO;
using System.Text;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public abstract class Asn1String : Asn1UniversalObject
        {
        public abstract Encoding Encoding { get; }
        public String Value { get;protected set; }

        protected Asn1String(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        protected override void WriteContent(Stream target)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var r = Encoding.GetBytes(Value);
            target.Write(r, 0, r.Length);
            }

        protected internal override Boolean Decode()
            {
            if (IsDecoded) { return true; }
            if (IsIndefiniteLength) { return base.Decode(); }
            var r = new Byte[Length];
            Content.Read(r, 0, r.Length);
            Value = Encoding.GetString(r);
            State |= ObjectState.Decoded;
            return true;
            }

        public override Boolean Equals(Asn1Object other) {
            if (base.Equals(other)) {
                var source = other as Asn1String;
                if (!ReferenceEquals(source, null)) {
                    return String.Equals(source.Value, Value);
                    }
                }
            return false;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Value;
            }

        public static implicit operator String(Asn1String source) { return source.Value; }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),  Type.ToString());
            WriteValue(writer, serializer, nameof(Value), Value);
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            writer.WriteEndObject();
            }
        }
    }

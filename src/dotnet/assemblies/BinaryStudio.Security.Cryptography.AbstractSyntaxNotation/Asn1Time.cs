using System;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
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

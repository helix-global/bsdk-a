using System;
using BinaryStudio.IO;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="NULL"/> type.
    /// </summary>
    public sealed class Asn1Null : Asn1UniversalObject
        {
        internal Asn1Null(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <inheritdoc/>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.Null; }}

        protected override void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, nameof(Type),Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            }
        }
    }

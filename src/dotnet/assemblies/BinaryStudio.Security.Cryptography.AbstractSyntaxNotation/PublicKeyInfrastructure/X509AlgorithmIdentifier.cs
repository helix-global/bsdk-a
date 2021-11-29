using System;
using System.ComponentModel;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure
    {
    [TypeConverter(typeof(X509AlgorithmIdentifierTypeConverter))]
    public sealed class X509AlgorithmIdentifier: IJsonSerializable
        {
        public Asn1ObjectIdentifier Identifier { get; }
        public Asn1Object Parameters { get; }

        public X509AlgorithmIdentifier(Asn1Sequence source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var c = source.Count;
            if (c == 0)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (c >  2)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (!(source[0] is Asn1ObjectIdentifier)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            Identifier = (Asn1ObjectIdentifier)source[0];
            if (c == 2)
                {
                Parameters = source[1];
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Identifier.ToString();
            }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
            //writer.WriteIndentSpace(1);
            writer.WriteComment($"{Identifier.FriendlyName}");
            writer.WriteEndObject();
            }
        }
    }
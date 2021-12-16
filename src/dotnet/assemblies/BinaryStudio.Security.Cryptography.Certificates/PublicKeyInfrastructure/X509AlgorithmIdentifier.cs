using System;
using System.ComponentModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <pre style="font-family: Consolas">
    /// AlgorithmIdentifier  ::=  SEQUENCE  {
    ///   algorithm               OBJECT IDENTIFIER,
    ///   parameters              ANY DEFINED BY algorithm OPTIONAL
    ///   }
    /// </pre>
    /// </remarks>
    [TypeConverter(typeof(X509AlgorithmIdentifierTypeConverter))]
    [DefaultProperty(nameof(Identifier))]
    public sealed class X509AlgorithmIdentifier: IJsonSerializable
        {
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))]
        public Asn1ObjectIdentifier Identifier { get; }

        public Object Parameters { get; }

        public X509AlgorithmIdentifier(Asn1Sequence source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var c = source.Count;
            if (c == 0)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (c >  2)                               { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (!(source[0] is Asn1ObjectIdentifier)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            Identifier = (Asn1ObjectIdentifier)source[0];
            if (c == 2) {
                Parameters = X509PublicKeyParameters.From(
                    Identifier.ToString(),
                    source[1]);
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
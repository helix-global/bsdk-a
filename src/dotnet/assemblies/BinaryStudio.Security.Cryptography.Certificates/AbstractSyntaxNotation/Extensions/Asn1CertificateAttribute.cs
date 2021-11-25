using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal class Asn1CertificateAttribute : Asn1LinkObject<Asn1Sequence>
        {
        public Asn1ObjectIdentifier Type { get; }
        public IList<Asn1Object> Values { get; }
        public Asn1CertificateAttribute(Asn1Sequence source)
            : base(source)
            {
            Values = new List<Asn1Object>();
            Type = (Asn1ObjectIdentifier)source[0];
            foreach (var o in source[1]) {
                Values.Add(o);
                }
            Values = new ReadOnlyCollection<Asn1Object>(Values);
            }

        private static String ToString(Asn1Object source) {
            if (source == null) { return null; }
            return String.Join(String.Empty, source.Content.ToArray().Select(i => i.ToString("X2")));
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(Type), Type.ToString());
            if (!IsNullOrEmpty(Values)) {
                writer.WritePropertyName(nameof(Values));
                writer.WriteStartArray();
                foreach (var value in Values.Select(ToString).Where(i => !String.IsNullOrWhiteSpace(i))) {
                    writer.WriteValue(value);
                    }
                writer.WriteEndArray();
                }
            writer.WriteEndObject();
            }
        }
    }
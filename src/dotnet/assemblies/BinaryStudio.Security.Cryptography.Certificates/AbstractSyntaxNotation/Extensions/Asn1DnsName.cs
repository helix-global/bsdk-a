using System;
using System.Text;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal sealed class Asn1DnsName : Asn1GeneralNameObject
        {
        public String Value { get; }
        public Asn1DnsName(Asn1ContextSpecificObject source)
            : base(source)
            {
            Value = Encoding.UTF8.GetString(source.Content.ToArray());
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.DNS; }}

        public override String ToString()
            {
            return Value;
            }

        public override Boolean IsEmpty
            {
            get { return String.IsNullOrEmpty(Value); }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(Value);
            }
        }
    }
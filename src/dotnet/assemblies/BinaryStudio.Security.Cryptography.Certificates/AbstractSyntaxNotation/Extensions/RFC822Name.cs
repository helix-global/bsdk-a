using System;
using System.Text;
using BinaryStudio.Security.Cryptography.Certificates;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal sealed class RFC822Name : Asn1GeneralNameObject
        {
        public String Value { get; }
        public RFC822Name(Asn1ContextSpecificObject source)
            : base(source)
            {
            Value = Encoding.UTF8.GetString(source.Content.ToArray());
            }

        public override String ToString()
            {
            return Value;
            }

        protected override X509GeneralNameType InternalType { get { return X509GeneralNameType.RFC822; }}

        public override Boolean IsEmpty
            {
            get { return String.IsNullOrEmpty(Value); }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            writer.WriteValue(Value);
            }
        }
    }
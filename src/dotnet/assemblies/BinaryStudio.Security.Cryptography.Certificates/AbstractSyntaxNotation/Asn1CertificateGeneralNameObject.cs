using System;
using System.Collections.Generic;
using System.ComponentModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [TypeConverter(typeof(X509CertificateGeneralNameObjectConverter))]
    internal class Asn1CertificateGeneralNameObject : Asn1LinkObject
        {
        public Object Value { get; }
        private SByte ContextSpecificObjectType { get; }
        public Asn1CertificateGeneralNameObject(Asn1Object contextspecific)
            :base(contextspecific)
            {
            ContextSpecificObjectType = ((Asn1ContextSpecificObject)contextspecific).Type;
            switch (ContextSpecificObjectType)
                {
                case 1:
                case 2:
                case 6:
                    {
                    //contextspecific.Source.Seek(0, SeekOrigin.Begin);
                    //Value = (new Asn1IA5String(contextspecific.Source)).Value;
                    throw new NotImplementedException();
                    }
                    break;
                case 4:
                    {
                    var sequence = ((Asn1Object)contextspecific)[0];
                    Value = Asn1Certificate.Make(sequence);
                    }
                    break;
                default: throw new NotImplementedException();
                }
            }

        private IList<PropertyDescriptor> cache;
        protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            foreach (var descriptor in GetProperties()) {
                yield return descriptor;
                }
            }

        public override String ToString()
            {
            return Value.ToString();
            }

        protected override IEnumerable<PropertyDescriptor> GetProperties() {
            if (cache == null) {
                cache = new List<PropertyDescriptor>();
                foreach (var descriptor in base.GetProperties())
                    {
                    cache.Add(descriptor.Name == nameof(Value)
                        ? TypeDescriptor.CreateProperty(GetType(), descriptor,
                            new Asn1DisplayNameAttribute(
                                $"{nameof(Asn1CertificateAuthorityInformationAccess)}:{ContextSpecificObjectType}"))
                        : descriptor);
                    }
                }
            foreach (var descriptor in cache) {
                yield return descriptor;
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (Value is IJsonSerializable o)
                {
                o.WriteJson(writer, serializer);
                }
            else
                {
                writer.WriteValue(Value);
                }
            }
        }
    }
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    internal class Asn1CertificateAuthorityInformationAccess : Asn1LinkObject
        {
        public Asn1ObjectIdentifier AccessMethod { get; }
        public IX509GeneralName AccessLocation { get; }

        private SByte ContextSpecificObjectType { get; }
        public Asn1CertificateAuthorityInformationAccess(Asn1Object source)
            : base(source)
            {
            var sequence = (Asn1Sequence)source;
            AccessMethod = (Asn1ObjectIdentifier)sequence[0];
            AccessLocation = X509GeneralName.From((Asn1ContextSpecificObject)sequence[1]);
            }

        protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            foreach (var descriptor in base.GetProperties(attributes)) {
                if (descriptor.Name == nameof(AccessLocation)) {
                    yield return TypeDescriptor.CreateProperty(GetType(), descriptor, new Asn1DisplayNameAttribute($"{nameof(Asn1CertificateAuthorityInformationAccess)}:{ContextSpecificObjectType}"));
                    }
                else
                    {
                    yield return descriptor;
                    }
                }
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"{Asn1DecodedObjectIdentifierTypeConverter.ToString(AccessMethod)}: \"{AccessLocation}\"";
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            var type = AccessMethod.ToString();
            var r = OID.ResourceManager.GetString(type, CultureInfo.InvariantCulture);
            if (!String.IsNullOrWhiteSpace(r)) {
                //writer.WriteIndent();
                writer.WriteComment($" {r} ");
                }
            writer.WriteValue(serializer, nameof(AccessMethod),   type);
            writer.WriteValue(serializer, nameof(AccessLocation), AccessLocation);
            writer.WriteEndObject();
            }
        }
    }
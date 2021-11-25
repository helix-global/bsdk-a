using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /*
     * {iso(1) identified-organization(3) dod(6) internet(1) private(4) enterprise(1) 311 21 14}
     * 1.3.6.1.4.1.311.21.14
     * /ISO/Identified-Organization/6/1/4/1/311/21/14
     * szOID_CRL_SELF_CDP
     * */
    [Asn1SpecificObject("1.3.6.1.4.1.311.21.14")]
    internal class Asn1PublishedCRLLocations : Asn1CertificateExtension
        {
        public IList<IX509GeneralName> PublishedCRLLocations { get; }
        public Asn1PublishedCRLLocations(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (!ReferenceEquals(octet, null)) {
                if (octet.Count > 0) {
                    PublishedCRLLocations = new ReadOnlyCollection<IX509GeneralName>(octet[0][0][0][0].
                        OfType<Asn1ContextSpecificObject>().
                        Select(X509GeneralName.From).
                        Where(i => !i.IsEmpty).
                        ToArray());
                    }
                }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                if (PublishedCRLLocations != null)
                    {
                    writer.WritePropertyName(nameof(PublishedCRLLocations));
                    using (writer.ArrayScope(serializer)) {
                        foreach (var name in PublishedCRLLocations.OfType<IJsonSerializable>()) {
                            name.WriteJson(writer, serializer);
                            }
                        }
                    }
                }
            }
        }
    }
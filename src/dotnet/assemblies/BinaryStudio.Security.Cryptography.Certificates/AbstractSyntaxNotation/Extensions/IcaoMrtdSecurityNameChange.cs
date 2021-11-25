using System;
using System.Globalization;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    /**
     * {joint-iso-itu-t(2) international-organizations(23) 136 mrtd(1) security(1) extensions(6) nameChange(1)}
     * {2.23.136.1.1.6.1}
     * {/Joint-ISO-ITU-T/International-Organizations/136/1/1/6/1}
     *
     * id-icao                                     OBJECT IDENTIFIER ::= {2.23.136}
     * id-icao-mrtd                                OBJECT IDENTIFIER ::= {id-icao 1}
     * id-icao-mrtd-security                       OBJECT IDENTIFIER ::= {id-icao-mrtd 1}
     * id-icao-mrtd-security-extensions            OBJECT IDENTIFIER ::= {id-icao-mrtdsecurity 6}
     * id-icao-mrtd-security-extensions-nameChange OBJECT IDENTIFIER ::= {idicaomrtd-security-extensions 1}
     * nameChange EXTENSION ::=
     * {
     *   SYNTAX NULL IDENTIFIED BY id-icao-mrtd-security-extensions-nameChange
     * }
     */
    [Asn1SpecificObject("2.23.136.1.1.6.1")]
    public class IcaoMrtdSecurityNameChange : Asn1CertificateExtension
        {
        public IcaoMrtdSecurityNameChange(Asn1CertificateExtension source)
            : base(source)
            {
            var octet = Body;
            if (octet == null)           { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (octet.Count != 1)        { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (!(octet[0] is Asn1Null)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                }
            }
        }
    }
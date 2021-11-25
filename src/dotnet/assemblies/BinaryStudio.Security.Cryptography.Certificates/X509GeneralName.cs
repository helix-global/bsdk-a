using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public static class X509GeneralName
        {
        public static IX509GeneralName From(Asn1ContextSpecificObject source)
            {
            switch ((X509GeneralNameType)source.Type)
                {
                case X509GeneralNameType.Other:            { return new Asn1OtherName(source);       }
                case X509GeneralNameType.RFC822:           { return new RFC822Name(source);          }
                case X509GeneralNameType.DNS:              { return new Asn1DnsName(source);         }
                case X509GeneralNameType.X400Address:      { return new X400Address(source);         }
                case X509GeneralNameType.Directory:        { return Asn1Certificate.Make(source[0]); }
                case X509GeneralNameType.EDIParty:         { return new EdiPartyName(source);        }
                case X509GeneralNameType.IA5String:        { return new Asn1Uri(source);             }
                case X509GeneralNameType.OctetString:      { return new Asn1IpAddress(source);       }
                case X509GeneralNameType.ObjectIdentifier: { return new Asn1RegisteredId(source);    }
                default: throw new ArgumentOutOfRangeException(nameof(source));
                }
            }

        public static String ToString(X509GeneralNameType type) {
            switch(type) {
                case X509GeneralNameType.Other:            return "other";
                case X509GeneralNameType.RFC822:           return "rfc822-name";
                case X509GeneralNameType.DNS:              return "dns-name";
                case X509GeneralNameType.X400Address:      return "x400-address";
                case X509GeneralNameType.Directory:        return "dir-name";
                case X509GeneralNameType.EDIParty:         return "edi-party";
                case X509GeneralNameType.IA5String:        return "uri";
                case X509GeneralNameType.OctetString:      return "ip-address";
                case X509GeneralNameType.ObjectIdentifier: return "registered-id";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }
    }
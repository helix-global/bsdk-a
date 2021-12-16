using System;

namespace BinaryStudio.Security.XAdES
    {
    public abstract class XAdESOperation
        {
        protected const String XmlDSigSchema = "http://www.w3.org/2000/09/xmldsig#";
        protected const String XAdESSchema   = "http://uri.etsi.org/01903/v1.4.1#";
        protected const String XAdESSignedProperties = "http://uri.etsi.org/01903#SignedProperties";
        }
    }
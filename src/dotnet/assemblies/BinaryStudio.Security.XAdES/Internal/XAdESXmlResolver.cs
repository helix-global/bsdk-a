using System;
using System.Net;
using System.Xml;

namespace BinaryStudio.Security.XAdES.Internal
    {
    public class XAdESXmlResolver : XmlResolver
        {
        public override Object GetEntity(Uri absoluteUri, String role, Type ofObjectToReturn)
            {
            throw new NotImplementedException();
            }

        public override Uri ResolveUri(Uri baseUri, String relativeUri)
            {
            return base.ResolveUri(baseUri, relativeUri);
            }

        public override Boolean SupportsType(Uri absoluteUri, Type type)
            {
            return base.SupportsType(absoluteUri, type);
            }

        public override ICredentials Credentials
            {
            set { throw new NotImplementedException(); }
            }
        }
    }
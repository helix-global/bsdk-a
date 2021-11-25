using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public abstract class CmsSignerIdentifier : CmsObject
        {
        protected CmsSignerIdentifier(Asn1Object o)
            : base(o)
            {
            }

        public static CmsSignerIdentifier Choice(Asn1Object o)
            {
            if (o == null) { throw new ArgumentNullException(nameof(o)); }
            if (o is Asn1Sequence u) { return new CmsIssuerAndSerialNumber(u); }
            if (o is Asn1ContextSpecificObject contextspecific) {
                if (contextspecific.Type == 0) {
                    // TODO: ????
                    return null;
                    return new CmsSubjectKeyIdentifier(contextspecific);
                    }
                }
            throw new ArgumentOutOfRangeException(nameof(o));
            }
        }
    }
using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public class CmsSubjectKeyIdentifier : CmsSignerIdentifier
        {
        public CmsSubjectKeyIdentifier(Asn1ContextSpecificObject o)
            : base(o)
            {
            throw new NotImplementedException();
            }
        }
    }
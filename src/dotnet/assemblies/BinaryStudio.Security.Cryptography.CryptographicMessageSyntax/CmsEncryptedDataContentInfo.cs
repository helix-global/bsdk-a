using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public class CmsEncryptedDataContentInfo : CmsContentInfo
        {
        public CmsEncryptedDataContentInfo(Asn1Object source)
            : base(source)
            {
            }
        }
    }
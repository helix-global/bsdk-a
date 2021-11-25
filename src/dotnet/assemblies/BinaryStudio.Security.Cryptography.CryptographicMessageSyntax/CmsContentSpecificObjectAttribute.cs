using System;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public class CmsContentSpecificObjectAttribute : Attribute
        {
        public String Key { get; }
        public CmsContentSpecificObjectAttribute(String key) {
            Key = key;
            }
        }
    }
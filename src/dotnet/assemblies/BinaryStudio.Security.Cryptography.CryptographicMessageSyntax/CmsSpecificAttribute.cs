using System;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsSpecificAttribute : Attribute
        {
        public String Key { get; }
        public CmsSpecificAttribute(String key)
            {
            Key = key;
            }
        }
    }
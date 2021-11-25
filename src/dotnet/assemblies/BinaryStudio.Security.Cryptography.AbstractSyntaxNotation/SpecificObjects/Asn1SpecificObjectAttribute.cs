using System;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class Asn1SpecificObjectAttribute : Attribute
        {
        public String Key { get; }
        public Asn1SpecificObjectAttribute(String key) {
            Key = key;
            }

        public override String ToString()
            {
            return Key;
            }
        }
    }
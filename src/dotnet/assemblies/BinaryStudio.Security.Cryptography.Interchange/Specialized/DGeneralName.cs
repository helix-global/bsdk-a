using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DGeneralName :
        IEquatable<DGeneralName>,
        IEquatable<IX509GeneralName>
        {
        public bool Equals(DGeneralName other)
            {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && string.Equals(Value, other.Value);
            }

        public Boolean Equals(IX509GeneralName other)
            {
            if (other == null) { return false; }
            if ((Byte)(Int32)other.Type != Type) { return false; }
            if (other.Type == X509GeneralNameType.Directory) {
                if (RelativeDistinguishedNameSequence != null) {
                    return RelativeDistinguishedNameSequence.Equals((Asn1RelativeDistinguishedNameSequence)other);
                    }
                }
            return String.Equals(Value, other.ToString());
            }

        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DGeneralName)obj);
            }

        public override int GetHashCode()
            {
            unchecked
                {
                return (Type.GetHashCode() * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                }
            }
        }
    }
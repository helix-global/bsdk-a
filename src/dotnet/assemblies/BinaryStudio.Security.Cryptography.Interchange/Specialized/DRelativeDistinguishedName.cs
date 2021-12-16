using System;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DRelativeDistinguishedName : IEquatable<DRelativeDistinguishedName>
        {
        public bool Equals(DRelativeDistinguishedName other)
            {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ObjectIdentifier, other.ObjectIdentifier) && Equals(String, other.String);
            }

        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DRelativeDistinguishedName) obj);
            }

        public override int GetHashCode()
            {
            unchecked
                {
                return ((ObjectIdentifier != null ? ObjectIdentifier.GetHashCode() : 0) * 397) ^ (String != null ? String.GetHashCode() : 0);
                }
            }
        }
    }
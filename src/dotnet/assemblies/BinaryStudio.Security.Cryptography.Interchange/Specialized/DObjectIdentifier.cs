using System;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DObjectIdentifier :
        IDInstance<String>,
        IEquatable<DObjectIdentifier>
        {
        public override Int32 GetHashCode()
            {
            return (Value != null ? Value.GetHashCode() : 0);
            }

        public bool Equals(DObjectIdentifier other)
            {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
            }

        public override bool Equals(object obj)
            {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DObjectIdentifier) obj);
            }
        }
    }
using System;
using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryDescriptiorKey : IEquatable<TypeLibraryDescriptiorKey>
        {
        public Guid Identifier { get; }
        public Version Version { get; }
        public SYSKIND SysKind { get; }

        public TypeLibraryDescriptiorKey(Guid g, Version v, SYSKIND o)
            {
            if (v == null) { throw new ArgumentNullException(nameof(v)); }
            Identifier = g;
            Version = v;
            SysKind = o;
            }

        public override Int32 GetHashCode() {
            unchecked
                {
                var hashCode = Identifier.GetHashCode();
                hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Int32) SysKind;
                return hashCode;
                }
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals(TypeLibraryDescriptiorKey other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return Identifier.Equals(other.Identifier) && Equals(Version, other.Version) && (SysKind == other.SysKind);
            }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <filterpriority>2</filterpriority>
        public override Boolean Equals(Object obj)
            {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true;  }
            return (obj is TypeLibraryDescriptiorKey r) && Equals(r);
            }

        public override String ToString()
            {
            return $"{Identifier}:{Version}:{SysKind}";
            }
        }
    }
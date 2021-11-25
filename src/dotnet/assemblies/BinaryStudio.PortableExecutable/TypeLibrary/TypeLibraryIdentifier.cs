using System;
using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryIdentifier : IEquatable<TypeLibraryIdentifier>
        {
        public Guid Identifier { get; }
        public Version Version { get; }
        public SYSKIND TargetOperatingSystemPlatform { get; }

        internal TypeLibraryIdentifier(Guid g, Version v, SYSKIND o)
            {
            Identifier = g;
            Version = v;
            TargetOperatingSystemPlatform = o;
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return $"{Identifier},{Version},{TargetOperatingSystemPlatform}";
            }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals(TypeLibraryIdentifier other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (Identifier.Equals(other.Identifier))
                && (Equals(Version, other.Version))
                && (TargetOperatingSystemPlatform == other.TargetOperatingSystemPlatform);
            }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><see langword="true"/> if the specified object  is equal to the current object; otherwise, <see langword="false"/>.</returns>
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            if (!(other is TypeLibraryIdentifier)) { return false; }
            return Equals((TypeLibraryIdentifier) other);
            }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode()
            {
            return HashCodeCombiner.GetHashCode(
                Identifier,
                Version,
                TargetOperatingSystemPlatform);
            }
        }
    }
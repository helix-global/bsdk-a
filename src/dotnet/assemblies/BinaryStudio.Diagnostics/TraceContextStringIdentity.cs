using System;

namespace BinaryStudio.Diagnostics
    {
    public class TraceContextStringIdentity : TraceContextIdentity, IEquatable<TraceContextStringIdentity>
        {
        public String Key { get; }
        public override String ShortName { get; }

        public TraceContextStringIdentity(String shortname, String key)
            {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #if NET35 || NETCOREAPP2_1
            if (String.IsNullOrEmpty(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
            #else
            if (String.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
            #endif
            Key = key;
            ShortName = shortname;
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(TraceContextStringIdentity other)
            {
            if (other == null) { return false; }
            return ReferenceEquals(this, other) || String.Equals(Key, other.Key);
            }

        /**
         * <summary>Serves as the default hash function.</summary>
         * <returns>A hash code for the current object.</returns>
         * */
        public override Int32 GetHashCode()
            {
            return Key.GetHashCode();
            }

        /**
         * <summary>Determines whether the specified object is equal to the current object.</summary>
         * <param name="other">The object to compare with the current object.</param>
         * <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
         * */
        public override Boolean Equals(Object other)
            {
            return (other is TraceContextStringIdentity r) && Equals(r);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * */
        public override String ToString()
            {
            return Key;
            }
        }
    }
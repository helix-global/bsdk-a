using System;
using System.Reflection;

namespace BinaryStudio.Diagnostics
    {
    public class TraceContextMethodIdentity : TraceContextIdentity, IEquatable<TraceContextMethodIdentity>
        {
        public override String ShortName { get { return Method.Name; }}
        private MethodBase Method { get; }
        public TraceContextMethodIdentity(MethodBase mi)
            {
            if (mi == null) { throw new ArgumentNullException(nameof(mi)); }
            Method = mi;
            }

        /**
         * <summary>Serves as the default hash function.</summary>
         * <returns>A hash code for the current object.</returns>
         * */
        public override Int32 GetHashCode()
            {
            return Method.GetHashCode();
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(TraceContextMethodIdentity other)
            {
            if (other == null) { return false; }
            return ReferenceEquals(this, other) ||
                Method.Equals(other.Method);
            }

        /**
         * <summary>Determines whether the specified object is equal to the current object.</summary>
         * <param name="other">The object to compare with the current object.</param>
         * <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
         * */
        public override Boolean Equals(Object other)
            {
            return (other is TraceContextMethodIdentity r) && Equals(r);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * */
        public override String ToString()
            {
            return $"{ToString(Method)}";
            }
        }
    }
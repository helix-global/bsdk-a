using System;
using System.ComponentModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
        public class X509ObjectIdentifier : Asn1LinkObject<Asn1ObjectIdentifier>, IEquatable<String>
        {
        public X509ObjectIdentifier(Asn1ObjectIdentifier source)
            : base(source)
            {
            }

        /**
         * <summary>Gets the service object of the specified type.</summary>
         * <param name="service">An object that specifies the type of service object to get.</param>
         * <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Object GetService(Type service)
            {
            var r = base.GetService(service);
            if (r != null) { return r; }
            if (service == typeof(Asn1ObjectIdentifier)) { return UnderlyingObject; }
            return null;
            }

        [Browsable(false)] public String FrendlyName { get { return UnderlyingObject.FriendlyName; }}

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return UnderlyingObject.ToString();
            }

        /**
         * <summary>Indicates whether the current object is equal to another string object.</summary>
         * <param name="key">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="key"/> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(String key) {
            return (Equals(ToString(), key));
            }

        /**
         * <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.</summary>
         * <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.</returns>
         * <param name="other">The object to compare with the current object.</param>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(this, other)) { return true; }
            if (other is String) { return Equals((String)other); }
            return base.Equals(other);
            }

        /**
         * <summary>Serves as a hash function for a particular type.</summary>
         * <returns>A hash code for the current <see cref="Object"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            return ToString().GetHashCode();
            }
        }
    }
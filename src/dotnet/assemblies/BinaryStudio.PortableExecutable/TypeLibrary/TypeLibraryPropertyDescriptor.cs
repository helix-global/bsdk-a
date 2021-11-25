using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    internal abstract class TypeLibraryPropertyDescriptor : TypeLibraryMemberDescriptor, ITypeLibraryPropertyDescriptor
    {
        protected TypeLibraryPropertyDescriptor(ITypeLibraryTypeDescriptor type)
            : base(type)
            {
            }

        public abstract Int32 Id { get; }
        public override TypeLibraryMemberTypes MemberType { get { return TypeLibraryMemberTypes.Property; }}
        public virtual Boolean CanRead  { get { return (GetMethod != null); }}
        public virtual Boolean CanWrite { get { return (SetMethod != null); }}
        public abstract ITypeLibraryTypeDescriptor PropertyType { get; }
        public abstract ITypeLibraryMethodDescriptor GetMethod { get; }
        public abstract ITypeLibraryMethodDescriptor SetMethod { get; }
        public virtual IList<ITypeLibraryParameterDescriptor> Parameters { get { return EmptyList<ITypeLibraryParameterDescriptor>.Value; }}

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
            r.AppendFormat("{1} {0} ", Name, PropertyType.Name);
            r.Append("{ ");
                 if (CanRead && CanWrite) { r.Append("get;set;"); }
            else if (CanRead)             { r.Append("get;");     }
            else                          { r.Append("set;");     }
            r.Append(" }");
            return r.ToString();
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public sealed override Boolean Equals(ITypeLibraryMemberDescriptor other)
            {
            return base.Equals(other)
                && Equals(other as ITypeLibraryPropertyDescriptor);
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public virtual Boolean Equals(ITypeLibraryPropertyDescriptor other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (CanRead == other.CanRead) && (Id == other.Id)
                && (MemberType == other.MemberType)
                && (CanWrite == other.CanWrite)
                && (Equals(PropertyType, other.PropertyType))
                && (Equals(GetMethod, other.GetMethod))
                && (Equals(SetMethod, other.SetMethod))
                && (String.Equals(Name, other.Name))
                && (Equals(Parameters, other.Parameters));
            }

        /**
         * <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
         * <param name="other">The object to compare with the current object. </param>
         * <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return Equals(other as ITypeLibraryPropertyDescriptor);
            }

        /**
         * <summary>Serves as a hash function for a particular type. </summary>
         * <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            return HashCodeCombiner.GetHashCode(
                base.GetHashCode(),
                Id,
                PropertyType);
            }
        }
    }
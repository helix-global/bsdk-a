using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    [DefaultProperty(nameof(LibraryQualifiedName))]
    internal abstract class TypeLibraryTypeDescriptor : TypeLibraryMemberDescriptor, ITypeLibraryTypeDescriptor
        {
        public override String HelpString { get { return null; }}
        #region P:LibraryQualifiedName:String
        [JsonIgnore] public virtual String LibraryQualifiedName { get {
            var r = new StringBuilder();
            r.Append(Name);
            if (Library != null) {
                r.Append(", ");
                r.Append(Library);
                }
            return r.ToString();
            }}
        #endregion
        [JsonIgnore] public virtual ITypeLibraryDescriptor Library { get { return null; }}
        public abstract Version Version { get; }
        public abstract Guid? UniqueIdentifier { get; }
        public abstract ITypeLibraryTypeDescriptor UnderlyingType { get; }
        public abstract ITypeLibraryTypeDescriptor BaseType { get; }
        public override TypeLibraryMemberTypes MemberType { get { return TypeLibraryMemberTypes.Type; }}
        public abstract TypeLibTypeFlags Flags { get; }
        public abstract Boolean IsAlias     { get; }
        public abstract Boolean IsPrimitive { get; }
        public abstract Boolean IsPointer   { get; }
        public abstract Boolean IsArray     { get; }
        public abstract Boolean IsEnum      { get; }
        public abstract Boolean IsUnion     { get; }
        public abstract Boolean IsStructure { get; }
        public abstract Boolean IsInterface { get; }
        public abstract Boolean IsDispatch  { get; }
        public abstract Boolean IsModule    { get; }
        public abstract Boolean IsClass     { get; }
        public virtual ITypeLibraryFixedArrayTypeDescriptor FixedArrayTypeDescriptor { get { return null; }}
        public virtual IList<ITypeLibraryFieldDescriptor>    DeclaredFields        { get { return EmptyList<ITypeLibraryFieldDescriptor>.Value;    }}
        public virtual IList<ITypeLibraryMethodDescriptor>   DeclaredMethods       { get { return EmptyList<ITypeLibraryMethodDescriptor>.Value;   }}
        public virtual IList<ITypeLibraryPropertyDescriptor> DeclaredProperties    { get { return EmptyList<ITypeLibraryPropertyDescriptor>.Value; }}
        public virtual IList<ITypeLibraryTypeReference>      ImplementedInterfaces { get { return EmptyList<ITypeLibraryTypeReference>.Value; }}

        protected TypeLibraryTypeDescriptor(ITypeLibraryTypeDescriptor type)
            :base(type)
            {
            }

        #region M:ToString:String
        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
                 if (IsClass)     { r.Append("coclass ");       }
            else if (IsDispatch)  { r.Append("dispinterface "); }
            else if (IsInterface) { r.Append("interface ");     }
            else if (IsStructure) { r.Append("struct ");        }
            else if (IsUnion)     { r.Append("union ");         }
            else if (IsModule)    { r.Append("module ");        }
            else if (IsAlias)     { r.Append("alias ");         }
            else if (IsEnum)      { r.Append("enum ");          }
            r.Append(Name);
            return r.ToString();
            }
        #endregion

        protected override IEnumerable<PropertyDescriptor> GetProperties() {
            foreach (var pi in base.GetProperties()) {
                var i = pi;
                switch (pi.Name) {
                    case nameof(HelpString):
                        {
                        if (String.IsNullOrWhiteSpace(HelpString)) {
                            i = null;
                            //var attributes = new List<Attribute>(i.Attributes.OfType<Attribute>()) {
                            //    new DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)
                            //    };
                            //i = TypeDescriptor.CreateProperty(GetType(), i, attributes.ToArray());
                            }
                        }
                        break;
                    }
                if (i != null)
                    {
                    yield return i;
                    }
                }
            }

        /**
         * <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
         * <param name="other">The object to compare with the current object. </param>
         * <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other){
            return Equals(this, other as ITypeLibraryTypeDescriptor);
            }

        /**
         * <summary>Indicates whether the one object is equal to another object of the same type.</summary>
         * <returns>true if the object specified with <paramref name="x" /> is equal to the <paramref name="y" /> parameter; otherwise, false.</returns>
         * */
        public static Boolean Equals(ITypeLibraryTypeDescriptor x, ITypeLibraryTypeDescriptor y) {
            if (ReferenceEquals(x, y)) {  return true;  }
            if ((x == null) || (y == null)) { return false; }
            return x.Equals(y);
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public sealed override Boolean Equals(ITypeLibraryMemberDescriptor other)
            {
            if (other == null) { return false; }
            return base.Equals(other)
                && Equals(other as ITypeLibraryTypeDescriptor);
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public virtual Boolean Equals(ITypeLibraryTypeDescriptor other) {
            if (other == null) { return false; }
            return (MemberType == other.MemberType)
                && (String.Equals(LibraryQualifiedName, other.LibraryQualifiedName))
                && (IsDispatch == other.IsDispatch)
                && (IsAlias == other.IsAlias)
                && (IsClass == other.IsClass)
                && (IsInterface == other.IsInterface)
                && (IsStructure == other.IsStructure)
                && (IsUnion == other.IsUnion)
                && (IsModule == other.IsModule)
                && (IsEnum == other.IsEnum)
                && (IsArray == other.IsArray)
                && (IsPointer == other.IsPointer)
                && (UniqueIdentifier == other.UniqueIdentifier)
                && Equals(other.UnderlyingType, other.UnderlyingType);
            }

        /**
         * <summary>Serves as a hash function for a particular type. </summary>
         * <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode() {
            return HashCodeCombiner.GetHashCode(
                LibraryQualifiedName,
                UniqueIdentifier);
            }
        }
    }
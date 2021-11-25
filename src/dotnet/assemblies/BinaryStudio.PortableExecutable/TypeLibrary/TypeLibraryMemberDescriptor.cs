using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    internal abstract class TypeLibraryMemberDescriptor : ITypeLibraryMemberDescriptor, ICustomTypeDescriptor
    {
        public abstract String Name { get; }
        public abstract String HelpString { get; }
        [JsonIgnore] public abstract TypeLibraryMemberTypes MemberType { get; }
        [JsonIgnore] public virtual ITypeLibraryTypeDescriptor DeclaringType { get; }
        public virtual IList<TypeLibraryCustomAttribute> CustomAttributes { get { return EmptyList<TypeLibraryCustomAttribute>.Value; }}

        protected TypeLibraryMemberDescriptor(ITypeLibraryTypeDescriptor type) {
            DeclaringType = type;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Name;
            }

        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            return TypeDescriptor.GetAttributes(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetClassName:String
        String ICustomTypeDescriptor.GetClassName()
            {
            return TypeDescriptor.GetClassName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetComponentName:String
        String ICustomTypeDescriptor.GetComponentName()
            {
            return Name;
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            return TypeDescriptor.GetConverter(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultEvent:EventDescriptor
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            return TypeDescriptor.GetDefaultEvent(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            return TypeDescriptor.GetDefaultProperty(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        Object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
            return TypeDescriptor.GetEditor(GetType(), editorBaseType);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents:EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            return TypeDescriptor.GetEvents(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            return TypeDescriptor.GetEvents(GetType(), attributes);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
            return new PropertyDescriptorCollection(GetProperties().ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
            /* TODO: */
            var r = GetProperties().ToArray();
            return new PropertyDescriptorCollection(r);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor):Object
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
            return this;
            }
        #endregion
        #region M:GetProperties:IEnumerable<PropertyDescriptor>
        protected virtual IEnumerable<PropertyDescriptor> GetProperties() { return TypeDescriptor.GetProperties(GetType()).OfType<PropertyDescriptor>(); }
        #endregion

        /**
         * <summary>Indicates whether the one object is equal to another object of the same type.</summary>
         * <returns>true if the object specified with <paramref name="x" /> is equal to the <paramref name="y" /> parameter; otherwise, false.</returns>
         * */
        public static Boolean Equals(ITypeLibraryMemberDescriptor x, ITypeLibraryMemberDescriptor y) {
            if (ReferenceEquals(x, y)) { return true; }
            if ((x == null) || (y == null)) { return false; }
            return x.Equals(y);
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public virtual Boolean Equals(ITypeLibraryMemberDescriptor other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (MemberType == other.MemberType)
                && String.Equals(Name, other.Name)
                && Equals(DeclaringType, other.DeclaringType);
            }

        /**
         * <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
         * <param name="other">The object to compare with the current object. </param>
         * <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other) {
            return Equals(other as ITypeLibraryMemberDescriptor);
            }

        /**
         * <summary>Serves as a hash function for a particular type. </summary>
         * <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            return HashCodeCombiner.GetHashCode(
                Name,
                DeclaringType,
                MemberType);
            }

        protected static Boolean Equals(IList<ITypeLibraryParameterDescriptor> x, IList<ITypeLibraryParameterDescriptor> y) {
            if (ReferenceEquals(x, y)) { return true; }
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) { return false; }
            var c = x.Count;
            if (c == y.Count) {
                for (var i = 0; i < c; ++i) {
                    if (!Equals(x[i], y[i])) { return false; }
                    }
                return true;
                }
            return false;
            }

        private static String ToString(Object source) {
            var defaultproperty = TypeDescriptor.GetDefaultProperty(source);
            return (defaultproperty != null)
                ? defaultproperty.GetValue(source).ToString()
                : source.ToString();
            }

        protected void WriteJsonValue(JsonWriter writer, Object value, JsonSerializer serializer) {
            var converterattribute = TypeDescriptor.GetAttributes(value).OfType<JsonConverterAttribute>().FirstOrDefault();
            if (converterattribute != null) {
                ((JsonConverter)Activator.CreateInstance(converterattribute.ConverterType)).
                    WriteJson(writer, value, serializer);
                return;
                }
            writer.WriteValue(value);
            }
        }
    }
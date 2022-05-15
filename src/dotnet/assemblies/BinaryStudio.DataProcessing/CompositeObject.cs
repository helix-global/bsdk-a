using System;
using System.Collections.Generic;
using System.ComponentModel;
using BinaryStudio.DataProcessing.Properties;

namespace BinaryStudio.DataProcessing
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public class CompositeObject : ICustomTypeDescriptor
        {
        protected internal Object Source {get; }
        protected internal List<PropertyDescriptor> Properties {get; }
        protected internal String DisplayName {get; }
        protected internal TypeConverter Converter {get;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="descriptors"></param>
        /// <param name="name"></param>
        internal CompositeObject(Object source, IEnumerable<PropertyDescriptor> descriptors, String name) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Converter = new ObjectTypeConverter();
            Source = source;
            Properties = new List<PropertyDescriptor>();
            if (descriptors != null) {
                Properties.AddRange(descriptors);
                }
            DisplayName = name;
            }

        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        AttributeCollection ICustomTypeDescriptor.GetAttributes() {
            return TypeDescriptor.GetAttributes(Source.GetType());
            }
        #endregion

        string ICustomTypeDescriptor.GetClassName()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            throw new NotImplementedException();
        }

        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        TypeConverter ICustomTypeDescriptor.GetConverter() {
            return Converter;
            }
        #endregion

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(Source.GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        Object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
            return TypeDescriptor.GetEditor(Source, editorBaseType);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents:EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
            return new PropertyDescriptorCollection(Properties.ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor)
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor) {
            return Source;
            }
        #endregion
        #region M:ToString:String
        public override String ToString() {
            return String.IsNullOrEmpty(DisplayName)
                ? Resources.CompositeObjectConverterText
                : DisplayName;
            }
        #endregion
        }
    }
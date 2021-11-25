using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Xml;

namespace BinaryStudio.PlatformUI.Shell
    {
    public abstract class DependencyObjectCustomSerializer : ICustomXmlSerializer
        {
        public IDependencyObjectCustomSerializerAccess Owner { get; }

        public Boolean ExcludeLocalizable { get; set; }

        public Boolean ExcludeOptional { get; set; }

        protected abstract IEnumerable<DependencyProperty> SerializableProperties { get; }

        public virtual Object Content
            {
            get
                {
                return null;
                }
            }

        public DependencyObjectCustomSerializer(IDependencyObjectCustomSerializerAccess owner)
            {
            Owner = owner;
            }

        protected Boolean ShouldSerializeProperty(DependencyProperty property, ref Object value)
            {
            if (!Owner.ShouldSerializeProperty(property))
                return false;
            var obj = Owner.GetValue(property);
            if (IsDefaultValue(property, obj))
                return false;
            value = obj;
            return true;
            }

        private void SerializeNonDefaultProperties(XmlWriter writer)
            {
            foreach (var serializableProperty in SerializableProperties)
                {
                var obj = (Object)null;
                if (ShouldSerializeProperty(serializableProperty, ref obj))
                    writer.WriteAttributeString(serializableProperty.Name, Convert.ToString(obj, CultureInfo.InvariantCulture));
                }
            }

        private static Boolean IsDefaultValue(DependencyProperty dp, Object value)
            {
            var defaultValue = dp.DefaultMetadata.DefaultValue;
            if (value == defaultValue)
                return true;
            if (value != null)
                return value.Equals(defaultValue);
            return false;
            }

        public virtual void WriteXmlAttributes(XmlWriter writer)
            {
            SerializeNonDefaultProperties(writer);
            }

        public virtual IEnumerable<KeyValuePair<String, Object>> GetNonContentPropertyValues()
            {
            yield break;
            }
        }
    }
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell {
    public class WindowProfileSerializer {
        private Dictionary<String, String> namespaceAssembly = new Dictionary<String, String>();
        private Dictionary<String, String> namespacePrefix = new Dictionary<String, String>();
        private const String XamlNull = "Null";
        private const String XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        public WindowProfileSerializerMode Mode { get; set; }

        public Boolean ExcludeOptional { get; set; }

        public Boolean ExcludeLocalizable { get; set; }

        public WindowProfileSerializationVariants SerializationVariant { get; set; }

        public void MapNamespaceToAssembly(String namespaceName, String assemblyName, String prefix) {
            namespaceName.ThrowIfNullOrEmpty("No namespace provide to map");
            assemblyName.ThrowIfNullOrEmpty("The assemblyName must be specified");
            prefix.ThrowIfNullOrEmpty("The prefix must be specified");
            namespaceAssembly.Add(namespaceName, assemblyName);
            namespacePrefix.Add(namespaceName, prefix);
            }

        public void Serialize(Object element, Stream stream) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (IsSequenceType(element.GetType()))
                throw new ArgumentException("Root serialized element must not be a sequence type", nameof(element));
            var settings = new XmlWriterSettings
            {
                CheckCharacters = false,
                CloseOutput = false,
                Indent = false,
                NewLineOnAttributes = false,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8
                };
            using (var writer = XmlWriter.Create(stream, settings))
                Serialize(element, writer);
            }

        public void Serialize(Object element, XmlWriter writer) {
            Validate.IsNotNull(element, "element");
            Validate.IsNotNull(writer, "writer");
            if (IsSequenceType(element.GetType()))
                throw new ArgumentException("Root serialized element must not be a sequence type", nameof(element));
            SerializeInternal(element, writer, true);
            }

        private void SerializeInternal(Object element, XmlWriter writer, Boolean isRootElement) {
            if (element == null)
                return;
            var customXmlSerializer = (ICustomXmlSerializer)null;
            var customXmlSerializable = element as ICustomXmlSerializable;
            var type = element.GetType();
            if (customXmlSerializable != null)
                type = customXmlSerializable.GetSerializedType();
            if (Mode == WindowProfileSerializerMode.Custom && customXmlSerializable != null) {
                customXmlSerializer = customXmlSerializable.CreateSerializer();
                if (customXmlSerializer == null)
                    return;
                customXmlSerializer.ExcludeLocalizable = ExcludeLocalizable;
                customXmlSerializer.ExcludeOptional = ExcludeOptional;
                }
            if (Mode == WindowProfileSerializerMode.Reflection && IsTypeNonSerializable(type))
                return;
            if (IsSequenceType(type)) {
                SerializeSequence(element as IEnumerable, writer);
                }
            else {
                var flag = true;
                if (customXmlSerializer != null && !customXmlSerializable.SerializationVariants.HasFlag(SerializationVariant))
                    flag = false;
                if (flag) {
                    if (GetPrefix(type) != null)
                        writer.WriteStartElement(GetPrefix(type), type.Name, GetClrNamespace(type));
                    else
                        writer.WriteStartElement(type.Name, GetClrNamespace(type));
                    if (isRootElement)
                        WriteNamespaceDeclarations(writer);
                    }
                var contentPropertyValue = (Object)null;
                IEnumerable<KeyValuePair<String, Object>> keyValuePairs;
                if (customXmlSerializer != null) {
                    if (flag)
                        customXmlSerializer.WriteXmlAttributes(writer);
                    keyValuePairs = customXmlSerializer.GetNonContentPropertyValues();
                    contentPropertyValue = customXmlSerializer.Content;
                    }
                else
                    keyValuePairs = GetChildPropertiesAndContent(element, writer, type, ref contentPropertyValue);
                if (flag) {
                    foreach (var keyValuePair in keyValuePairs) {
                        var localName = type.Name + "." + keyValuePair.Key;
                        if (GetPrefix(type) != null)
                            writer.WriteStartElement(GetPrefix(type), localName, GetClrNamespace(type));
                        else
                            writer.WriteStartElement(localName, GetClrNamespace(type));
                        SerializeInternal(keyValuePair.Value, writer, false);
                        writer.WriteEndElement();
                        }
                    }
                if (contentPropertyValue != null)
                    SerializeInternal(contentPropertyValue, writer, false);
                if (!flag)
                    return;
                writer.WriteEndElement();
                }
            }

        private static Boolean IsTypeNonSerializable(Type type) {
            return GetAttribute<NonXamlSerializedAttribute>(type) != null;
            }

        protected void WriteNamespaceDeclarations(XmlWriter writer) {
            writer.WriteAttributeString("xmlns", "x", null, "http://schemas.microsoft.com/winfx/2006/xaml");
            foreach (var key in namespacePrefix.Keys)
                writer.WriteAttributeString("xmlns", namespacePrefix[key], null, GetClrNamespace(key, namespaceAssembly[key]));
            }

        private void SerializeSequence(IEnumerable element, XmlWriter writer) {
            foreach (var element1 in element)
                SerializeInternal(element1, writer, false);
            }

        private String GetClrNamespace(Type type) {
            var assemblyName = (String)null;
            if (!namespaceAssembly.TryGetValue(type.Namespace, out assemblyName))
                assemblyName = type.Assembly.GetName().Name;
            return GetClrNamespace(type.Namespace, assemblyName);
            }

        private static String GetClrNamespace(String namespaceName, String assemblyName) {
            return String.Format("clr-namespace:{0};assembly={1}", namespaceName, assemblyName);
            }

        private String GetPrefix(Type type) {
            var str = (String)null;
            namespacePrefix.TryGetValue(type.Namespace, out str);
            return str;
            }

        private static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : class {
            var customAttributes = member.GetCustomAttributes(typeof(TAttribute), true);
            if (customAttributes.Length != 0)
                return (TAttribute)customAttributes[0];
            return default(TAttribute);
            }

        private Boolean IsPropertySerializable(PropertyInfo property) {
            var attribute1 = GetAttribute<DesignerSerializationVisibilityAttribute>(property);
            if (attribute1 != null && attribute1.Visibility == DesignerSerializationVisibility.Hidden)
                return false;
            if (ExcludeLocalizable) {
                var attribute2 = GetAttribute<LocalizableAttribute>(property);
                if (attribute2 != null && attribute2.IsLocalizable)
                    return false;
                }
            if (!property.CanRead)
                return false;
            if (!property.CanWrite)
                return IsSequenceType(property.PropertyType);
            return true;
            }

        private static Boolean IsSequenceType(Type type) {
            return typeof(IList).IsAssignableFrom(type);
            }

        private static Boolean IsContentProperty(ContentPropertyAttribute attribute, PropertyInfo property) {
            if (attribute == null)
                return false;
            return attribute.Name == property.Name;
            }

        private static Boolean IsDefaultValue(PropertyInfo property, Object value) {
            var attribute = GetAttribute<DefaultValueAttribute>(property);
            if (attribute == null)
                return false;
            if (value == attribute.Value)
                return true;
            if (value != null)
                return value.Equals(attribute.Value);
            return false;
            }

        private static void WriteAttributeValue(TypeConverter typeConverter, Object value, XmlWriter writer) {
            if (value == null) {
                writer.WriteString("{");
                writer.WriteQualifiedName("Null", "http://schemas.microsoft.com/winfx/2006/xaml");
                writer.WriteString("}");
                }
            else {
                var text = typeConverter.ConvertToInvariantString(value);
                if (text.StartsWith("{"))
                    text = "{}" + text;
                writer.WriteString(text);
                }
            }

        private Dictionary<String, Object> GetChildPropertiesAndContent(Object element, XmlWriter writer, Type type, ref Object contentPropertyValue) {
            var attribute = GetAttribute<ContentPropertyAttribute>(type);
            var dictionary = new Dictionary<String, Object>();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                if (IsPropertySerializable(property)) {
                    var obj = property.GetValue(element, null);
                    if (!IsDefaultValue(property, obj)) {
                        if (IsContentProperty(attribute, property)) {
                            contentPropertyValue = obj;
                            }
                        else {
                            var converter = TypeDescriptor.GetConverter(obj == null ? property.PropertyType : obj.GetType());
                            if (converter.CanConvertTo(typeof(String)) && converter.CanConvertFrom(typeof(String))) {
                                writer.WriteStartAttribute(property.Name);
                                WriteAttributeValue(converter, obj, writer);
                                writer.WriteEndAttribute();
                                }
                            else
                                dictionary[property.Name] = obj;
                            }
                        }
                    }
                }
            return dictionary;
            }
        }
    }
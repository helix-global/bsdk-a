using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [TypeConverter(typeof(Asn1ReadOnlyCollectionConverter))]
    #if USE_WINFORMS
    [Editor(typeof(NoEditor), typeof(UITypeEditor))]
    #endif
    public abstract class Asn1ReadOnlyCollection<T> : ReadOnlyCollection<T>, ICustomTypeDescriptor
        {
        private PropertyDescriptorCollection properties = null;

        protected Asn1ReadOnlyCollection(IEnumerable<T> source)
            : base(source.ToList())
            {
            }

        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        /**
         * <summary>Returns a collection of custom attributes for this instance of a component.</summary>
         * <returns>An <see cref="AttributeCollection"/> containing the attributes for this object.</returns>
         * */
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            return TypeDescriptor.GetAttributes(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetClassName:String
        /**
         * <summary>Returns the class name of this instance of a component.</summary>
         * <returns>The class name of the object, or null if the class does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetClassName()
            {
            return TypeDescriptor.GetClassName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetComponentName:String
        /**
         * <summary>Returns the name of this instance of a component.</summary>
         * <returns>The name of the object, or null if the object does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetComponentName()
            {
            return TypeDescriptor.GetComponentName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        /**
         * <summary>Returns a type converter for this instance of a component.</summary>
         * <returns>A <see cref="TypeConverter"/> that is the converter for this object, or null if there is no <see cref="TypeConverter"/> for this object.</returns>
         * */
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            return TypeDescriptor.GetConverter(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultEvent:EventDescriptor
        /**
         * <summary>Returns the default event for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.</returns>
         * */
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            return TypeDescriptor.GetDefaultEvent(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        /**
         * <summary>Returns the default property for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.</returns>
         * */
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            return TypeDescriptor.GetDefaultProperty(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        /**
         * <summary>Returns an editor of the specified type for this instance of a component.</summary>
         * <param name="editortype">A <see cref="Type"/> that represents the editor for this object. </param>
         * <returns>An <see cref="Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.</returns>
         * */
        Object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
            return TypeDescriptor.GetEditor(GetType(), editorBaseType);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents:EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            return TypeDescriptor.GetEvents(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component using the specified attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the filtered events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            return TypeDescriptor.GetEvents(GetType(), attributes);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        /**
         * <summary>Returns the properties for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
            EnsureCore();
            return properties;
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        /**
         * <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
            EnsureCore();
            return properties;
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor):Object
        /**
         * <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
         * <param name="descriptor">A <see cref="PropertyDescriptor"/> that represents the property whose owner is to be found. </param>
         * <returns>An <see cref="Object"/> that represents the owner of the specified property.</returns>
         * */
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return this;
            }
        #endregion

        private void EnsureCore()
            {
            if (properties == null)
                {
                properties = EnsureOverride();
                }
            }

        protected virtual PropertyDescriptorCollection EnsureOverride()
            {
            var r = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            var i = 0;
            foreach (var item in Items) {
                r.Add(new InternalDescriptor(
                    this,
                    item,
                    $"[{i}]"
                    ));
                i++;
                }
            return r;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return $"Count = {Count}";
            }

        protected class InternalDescriptor : PropertyDescriptor
            {
            private T Value { get; }
            private readonly Asn1ReadOnlyCollection<T> Owner;
            public InternalDescriptor(Asn1ReadOnlyCollection<T> owner, T value, String name)
                : base(name, new Attribute[0])
                {
                Value = value;
                Owner = owner;
                }

            #region M:CanResetValue(Object):Boolean
            /**
             * <summary>When overridden in a derived class, returns whether resetting an object changes its value.</summary>
             * <param name="component">The component to test for reset capability.</param>
             * <returns>true if resetting the component changes its value; otherwise, false.</returns>
             * */
            public override Boolean CanResetValue(Object component)
                {
                return false;
                }
            #endregion
            #region M:GetValue(Object):Object
            /**
             * <summary>When overridden in a derived class, gets the current value of the property on a component.</summary>
             * <param name="component">The component with the property for which to retrieve the value.</param>
             * <returns>The value of a property for a given component.</returns>
             * */
            public override Object GetValue(Object component)
                {
                return Value;
                }
            #endregion
            #region M:ResetValue(Object)
            /**
             * <summary>When overridden in a derived class, resets the value for this property of the component to the default value.</summary>
             * <param name="component">The component with the property value that is to be reset to the default value.</param>
             * */
            public override void ResetValue(Object component)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:SetValue(Object,Object)
            /**
             * <summary>When overridden in a derived class, sets the value of the component to a different value.</summary>
             * <param name="component">The component with the property value that is to be set.</param>
             * <param name="value">The new value.</param>
             * */
            public override void SetValue(Object component, Object value)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:ShouldSerializeValue(Object):Boolean
            /**
             * <summary>When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.</summary>
             * <param name="component">The component with the property to be examined for persistence.</param>
             * <returns>true if the property should be persisted; otherwise, false.</returns>
             * */
            public override Boolean ShouldSerializeValue(Object component)
                {
                return false;
                }
            #endregion

            public override Type ComponentType { get { return typeof(Asn1ReadOnlyCollection<T>); }}
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType  { get { return (Value != null) ? Value.GetType() : typeof(Object); }}
            public override String DisplayName { get { return Owner.GetDisplayName(Name, Value); }}

            /**
             * <summary>Returns a string that represents the current object.</summary>
             * <returns>A string that represents the current object.</returns>
             * <filterpriority>2</filterpriority>
             * */
            public override String ToString()
                {
                return Name;
                }
            }

        protected virtual String GetDisplayName(String name, T source)
            {
            return name;
            }

        protected static void WriteValue(JsonWriter writer, JsonSerializer serializer, String name, Object value) {
            if (value != null) {
                writer.WritePropertyName(name);
                if (value is IJsonSerializable)
                    {
                    ((IJsonSerializable)value).WriteJson(writer, serializer);
                    }
                else
                    {
                    writer.WriteValue(value);
                    }
                }
            }
        }
    }

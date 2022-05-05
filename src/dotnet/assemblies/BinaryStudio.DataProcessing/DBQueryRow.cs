using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace BinaryStudio.DataProcessing
    {
    public class DBQueryRow : ICustomTypeDescriptor
        {
        private readonly IList<PropertyDescriptor> Descriptors = new List<PropertyDescriptor>();
        internal readonly DBQuery Query;
        [Browsable(false)] public IList<Object> Values { get; }
        [Browsable(false)] public DataColumnCollection Columns { get; }
        [Browsable(false)] public Int64 ImplicitRowNumber { get; }

        internal DBQueryRow(DBQuery query, DataColumnCollection columns, Int64 id, IList<Object> values)
            {
            if (columns == null) { throw new ArgumentNullException(nameof(columns)); }
            Query = query;
            ImplicitRowNumber = id;
            Columns = columns;
            Values = values;
            var i = 0;
            foreach (DataColumn column in columns) {
                Descriptors.Add(new InternalPropertyDescriptor(
                    this,i, column.ColumnName,
                    new Attribute[0]));
                i++;
                }

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(GetType())) {
                Descriptors.Add(descriptor);
                }
            }

        /// <summary>Returns a collection of custom attributes for this instance of a component.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for this object.</returns>
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            return new AttributeCollection();
            }

        /// <summary>Returns the class name of this instance of a component.</summary>
        /// <returns>The class name of the object, or null if the class does not have a name.</returns>
        String ICustomTypeDescriptor.GetClassName()
            {
            return GetType().Name;
            }

        /// <summary>Returns the name of this instance of a component.</summary>
        /// <returns>The name of the object, or null if the object does not have a name.</returns>
        String ICustomTypeDescriptor.GetComponentName()
            {
            return GetType().Name;
            }

        /// <summary>Returns a type converter for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or null if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.</returns>
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            throw new NotImplementedException();
            }

        /// <summary>Returns the default event for this instance of a component.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.</returns>
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            throw new NotImplementedException();
            }

        /// <summary>Returns the default property for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.</returns>
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            throw new NotImplementedException();
            }

        /// <summary>Returns an editor of the specified type for this instance of a component.</summary>
        /// <returns>An <see cref="T:System.Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.</returns>
        /// <param name="editorBaseType">A <see cref="T:System.Type"/> that represents the editor for this object.</param>
        Object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
            throw new NotImplementedException();
            }

        /// <summary>Returns the events for this instance of a component.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection"/> that represents the events for this component instance.</returns>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            return ((ICustomTypeDescriptor)this).GetEvents(new Attribute[0]);
            }

        /// <summary>Returns the events for this instance of a component using the specified attribute array as a filter.</summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection"/> that represents the filtered events for this component instance.</returns>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            throw new NotImplementedException();
            }

        /// <summary>Returns the properties for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties for this component instance.</returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
            }

        /// <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.</returns>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
            return new PropertyDescriptorCollection(Descriptors.ToArray());
            }

        /// <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
        /// <returns>An <see cref="T:System.Object"/> that represents the owner of the specified property.</returns>
        /// <param name="descriptor">A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that represents the property whose owner is to be found.</param>
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return this;
            }

        private class InternalPropertyDescriptor : PropertyDescriptor
            {
            public Int32 Index { get; }
            public DBQueryRow Owner { get; }
            public InternalPropertyDescriptor(DBQueryRow owner, Int32 index, String name, Attribute[] attributes)
                : base(name, attributes)
                {
                Index = index;
                Owner = owner;
                }

            /// <summary>When overridden in a derived class, returns whether resetting an object changes its value.</summary>
            /// <returns>true if resetting the component changes its value; otherwise, false.</returns>
            /// <param name="component">The component to test for reset capability.</param>
            public override Boolean CanResetValue(Object component)
                {
                return false;
                }

            /// <summary>When overridden in a derived class, gets the current value of the property on a component.</summary>
            /// <returns>The value of a property for a given component.</returns>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            public override Object GetValue(Object component)
                {
                return Owner.Values[Index];
                }

            /// <summary>When overridden in a derived class, resets the value for this property of the component to the default value.</summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(Object component)
                {
                }

            /// <summary>When overridden in a derived class, sets the value of the component to a different value.</summary>
            /// <param name="component">The component with the property value that is to be set.</param>
            /// <param name="value">The new value. </param>
            public override void SetValue(Object component, Object value)
                {
                }

            /// <summary>When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.</summary>
            /// <returns>true if the property should be persisted; otherwise, false.</returns>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            public override Boolean ShouldSerializeValue(Object component)
                {
                return false;
                }

            public override Type ComponentType { get { return Owner.GetType(); }}
            public override Boolean IsReadOnly { get { return false; }}
            public override Type PropertyType { get { return  Owner.Columns[Index].DataType; }}
            }
        }
    }
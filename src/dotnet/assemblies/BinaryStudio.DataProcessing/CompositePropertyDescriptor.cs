using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BinaryStudio.DataProcessing
    {
    public class CompositePropertyDescriptor : PropertyDescriptor {
        public CompositeObject Source {get;private set; }
        public PropertyDescriptor PropertyDescriptor {get;private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        /// <param name="descriptors"></param>
        public CompositePropertyDescriptor(Object source, String name, Attribute[] attributes, IList<PropertyDescriptor> descriptors)
            : base(name, attributes) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            #if NET35
            Source = new CompositeObject(source, descriptors.Select(i => new CompositePropertyDescriptor(i, i.DisplayName.Substring(name.Length + 1))).OfType<PropertyDescriptor>(), name);
            #else
            Source = new CompositeObject(source, descriptors.Select(i => new CompositePropertyDescriptor(i, i.DisplayName.Substring(name.Length + 1))), name);
            #endif
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="descriptors"></param>
        public CompositePropertyDescriptor(Object source, String name, IList<PropertyDescriptor> descriptors)
            : this(source, name, new Attribute[0], descriptors) {
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        public CompositePropertyDescriptor(PropertyDescriptor source, String name)
            : base(name, source.Attributes.OfType<Attribute>().ToArray()) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            PropertyDescriptor = source;
            }

        #region M:CanResetValue(Object):Object
        public override Boolean CanResetValue(Object component) {
            return (PropertyDescriptor != null)
                ? PropertyDescriptor.CanResetValue(component)
                : false;
            }
        #endregion
        #region M:GetValue(Object):Object
        public override Object GetValue(Object component) {
            var source = component as ICustomTypeDescriptor; 
            return (PropertyDescriptor != null)
                ? PropertyDescriptor.GetValue((source != null) ? source.GetPropertyOwner(PropertyDescriptor) : component)
                : Source;
            }
        #endregion
        #region M:ResetValue(Object)
        public override void ResetValue(Object component) {
            if (PropertyDescriptor != null) {
                PropertyDescriptor.ResetValue(component);
                }
            }
        #endregion
        #region M:SetValue(Object,Object)
        public override void SetValue(Object component, Object value) {
            if (PropertyDescriptor != null) {
                PropertyDescriptor.SetValue(component, value);
                }
            }
        #endregion
        #region M:ShouldSerializeValue(Object):Boolean
        public override Boolean ShouldSerializeValue(Object component) {
            return (PropertyDescriptor != null)
                ? PropertyDescriptor.ShouldSerializeValue(component)
                : false;
            }
        #endregion

        #region P:ComponentType:Type
        public override Type ComponentType { get {
            return (PropertyDescriptor != null)
                    ? PropertyDescriptor.ComponentType
                    : typeof(CompositeObject);
            }}
        #endregion
        #region P:IsReadOnly:Boolean
        public override Boolean IsReadOnly { get {
            return (PropertyDescriptor != null)
                    ? PropertyDescriptor.IsReadOnly
                    : true;
            }}
        #endregion
        #region P:PropertyType:Type
        public override Type PropertyType { get {
            return (PropertyDescriptor != null)
                    ? PropertyDescriptor.PropertyType
                    : typeof(CompositeObject);
            }}
        #endregion
        }
    }
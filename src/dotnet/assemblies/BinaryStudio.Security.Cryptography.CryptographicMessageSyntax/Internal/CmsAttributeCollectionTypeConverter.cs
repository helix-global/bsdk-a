using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsAttributeCollectionTypeConverter : ObjectCollectionTypeConverter
        {
        /// <summary>Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or <see langword="null"/> if there are no properties.</returns>
        protected override IEnumerable<PropertyDescriptor> GetPropertiesInternal(ITypeDescriptorContext context, Object value, Attribute[] attributes) {
            if (value is ISet<CmsAttribute> items) {
                foreach (var item in items) {
                    var type = item.Type.FriendlyName;
                    yield return new InternalPropertyDescriptor(
                        type, item,
                        EmptyArray<Attribute>.Value);
                    }
                }
            }

        private class InternalPropertyDescriptor : PropertyDescriptor
            {
            private readonly Object value;
            public InternalPropertyDescriptor(String name, Object value, Attribute[] attributes)
                : base(name, attributes)
                {
                this.value = value;
                }

            public override Boolean CanResetValue(Object component)
                {
                return false;
                }

            public override Object GetValue(Object component)
                {
                return value;
                }

            public override void ResetValue(Object component)
                {
                throw new NotSupportedException();
                }

            public override void SetValue(Object component, Object value)
                {
                throw new NotSupportedException();
                }

            public override Boolean ShouldSerializeValue(Object component)
                {
                return false;
                }

            public override Type ComponentType { get; }
            public override Boolean IsReadOnly { get { return true; }}
            public override Type PropertyType { get { return value?.GetType(); }}
            }
        }
    }
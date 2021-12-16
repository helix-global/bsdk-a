using System;
using System.Collections.Generic;
using System.ComponentModel;
using BinaryStudio.PlatformUI;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    public class ECertificate : NotifyPropertyChangedDispatcherObject<X509Certificate>
        {
        public ECertificate(X509Certificate source)
            : base(source)
            {
            PageProperties = source.Source;
            PageStructure = source.Source;
            }

        public class PageGeneralPresenter : NotifyPropertyChangedDispatcherObject<Asn1Certificate> {
            public PageGeneralPresenter(Asn1Certificate source)
                : base(source)
                {
                }

            private Boolean FilterDescriptor(PropertyDescriptor descriptor) {
                switch (descriptor.Name) {
                    //case nameof(X509Certificate.Container): { return !String.IsNullOrWhiteSpace(Source.Container); }
                    }
                return true;
                }

            #region M:GetProperties(Attribute[]):IEnumerable<PropertyDescriptor>
            protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source, attributes)) {
                    if (FilterDescriptor(descriptor)) {
                        yield return descriptor;    
                        }
                    }
                }
            #endregion
            #region M:GetProperties:IEnumerable<PropertyDescriptor>
            protected override IEnumerable<PropertyDescriptor> GetProperties() {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source)) {
                    if (FilterDescriptor(descriptor)) {
                        yield return descriptor;    
                        }
                    }
                }
            #endregion

            protected override Object GetPropertyOwner(PropertyDescriptor descriptor)
                {
                return Source;
                }

            /// <summary>Returns a type converter for this instance of a component.</summary>
            /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or <see langword="null"/> if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.</returns>
            protected override TypeConverter GetConverter()
                {
                return TypeDescriptor.GetConverter(Source);
                }
            }

        public Object PageProperties { get; }
        public Object PageStructure { get; }
        }
    }
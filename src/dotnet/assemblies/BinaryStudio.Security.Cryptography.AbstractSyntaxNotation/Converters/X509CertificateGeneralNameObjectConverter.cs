using System;
using System.ComponentModel;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
    {
    internal class X509CertificateGeneralNameObjectConverter : TypeConverter
        {
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
            return true;
            }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, Object value, Attribute[] attributes)
            {
            return TypeDescriptor.GetProperties(value, attributes);
            }
        }
    }
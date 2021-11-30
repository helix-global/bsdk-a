using System;
using System.ComponentModel;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    internal class CmsMessageTypeConverter : ObjectTypeConverter
        {
        /// <summary>Returns whether this object supports properties, using the specified context.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns> <see langword="true"/> if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, <see langword="false"/>.</returns>
        public override Boolean GetPropertiesSupported(ITypeDescriptorContext context)
            {
            return true;
            }
        }
    }
using System;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CryptographicException : CryptographicException, IXmlSerializable
        {
        /**
         * <summary>This method is reserved and should not be used. When implementing the <see cref="IXmlSerializable"/> interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.</summary>
         * <returns>An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.</returns>
         */
        XmlSchema IXmlSerializable.GetSchema()
            {
            throw new NotImplementedException();
            }

        /**
         * <summary>Generates an object from its XML representation.</summary>
         * <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
         */
        void IXmlSerializable.ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }

        #region M:IXmlSerializable.WriteXml(XmlWriter)
        /**
         * <summary>Converts an object into its XML representation.</summary>
         * <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
         */
        void IXmlSerializable.WriteXml(XmlWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }

            }
        #endregion
        }
    }
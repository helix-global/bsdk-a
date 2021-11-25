using System;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CertificateRevocationList : X509Object, IJsonSerializable, IX509CertificateRevocationList
        {
        internal Asn1CertificateRevocationList UnderlyingObject { get; }
        public DateTime  EffectiveDate { get { return UnderlyingObject.EffectiveDate; }}
        public DateTime? NextUpdate    { get { return UnderlyingObject.NextUpdate;    }}
        public Int32 Version           { get { return UnderlyingObject.Version;       }}
        public X509RelativeDistinguishedNameSequence Issuer { get { return new X509RelativeDistinguishedNameSequence(UnderlyingObject.Issuer); }}
        public String FriendlyName { get { return UnderlyingObject.FriendlyName; }}

        public X509CertificateRevocationList(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Handle = CertCreateCRLContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, source, source.Length);
            UnderlyingObject = ConstructFromBinary(source);
            }

        public X509CertificateRevocationList(Asn1CertificateRevocationList source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UnderlyingObject = source;
            Handle = CertCreateCRLContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, source.Body, source.Body.Length);
            }

        public unsafe X509CertificateRevocationList(IntPtr handle)
            {
            if (handle == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(handle)); }
            Handle = handle;
            UnderlyingObject = ConstructFromBinary((CRL_CONTEXT*)handle);
            }

        #region M:ConstructFromBinary(ReadOnlyMemoryMappingStream):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList ConstructFromBinary(ReadOnlyMemoryMappingStream source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var o = Asn1Object.Load(source).FirstOrDefault();
            if (o == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
            return new Asn1CertificateRevocationList(o);
            }
        #endregion
        #region M:ConstructFromBinary(Byte[]):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList ConstructFromBinary(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return ConstructFromBinary(new ReadOnlyMemoryMappingStream(source));
            }
        #endregion
        #region M:ConstructFromBinary(CRL_CONTEXT*):Asn1CertificateRevocationList
        private static unsafe Asn1CertificateRevocationList ConstructFromBinary(CRL_CONTEXT* source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var size  = source->cbCrlEncoded;
            var bytes = source->pbCrlEncoded;
            var buffer = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                buffer[i] = bytes[i];
                }
            return ConstructFromBinary(buffer);
            }
        #endregion

        ///// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        ///// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        //XmlSchema IXmlSerializable.GetSchema()
        //    {
        //    throw new NotImplementedException();
        //    }

        ///// <summary>Generates an object from its XML representation.</summary>
        ///// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        //void IXmlSerializable.ReadXml(XmlReader reader)
        //    {
        //    throw new NotImplementedException();
        //    }

        ///// <summary>Converts an object into its XML representation.</summary>
        ///// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        //void IXmlSerializable.WriteXml(XmlWriter writer)
        //    {
        //    if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
        //    writer.WriteStartElement("CertificateRevocationList");
        //    writer.WriteAttributeString(nameof(Version), Version.ToString());
        //    writer.WriteAttributeString(nameof(EffectiveDate), EffectiveDate.ToString("O"));
        //    if (NextUpdate != null) {
        //        writer.WriteAttributeString(nameof(NextUpdate), NextUpdate.Value.ToString("O"));
        //        }
        //    var issuer = Issuer;
        //    if (issuer != null) {
        //        writer.WriteStartElement("CertificateRevocationList.Issuer");
        //        ((IXmlSerializable)issuer).WriteXml(writer);
        //        writer.WriteEndElement();
        //        }
        //    writer.WriteEndElement();
        //    writer.Flush();
        //    }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            UnderlyingObject.WriteJson(writer, serializer);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return FriendlyName;
            }

        public override X509ObjectType ObjectType { get { return X509ObjectType.Crl; }}
        public override IntPtr Handle { get; }

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CertCreateCRLContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);
        }
    }
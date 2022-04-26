using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using BinaryStudio.DataProcessing;
using BinaryStudio.IO;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
using BinaryStudio.DirectoryServices;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using Newtonsoft.Json;

#pragma warning disable 1591,1571

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /// <summary>
    /// <see cref="CmsMessage"/> class represents a CMS structure.
    /// For additional information, see <a href="https://tools.ietf.org/html/rfc5652">RFC 5652</a>.
    /// </summary>
    [TypeConverter(typeof(CmsMessageTypeConverter))]
    public sealed class CmsMessage : CmsObject, IFileService
        {
        private String thumbprint;

        /**
         * <summary>
         * This is the associated content. The type of content can be determined uniquely by <see cref="ContentType"/>.
         * </summary>
         */
        [Order(1)] public CmsContentInfo ContentInfo { get; }

        /**
         * <summary>
         * <see cref="ContentType"/> indicates the type of the associated content. It is
         * an object identifier; it is a unique string of integers assigned
         * by an authority that defines the content type.
         * </summary>
         * <remarks>
         * <table style="font-family: Consolas;width:100%;border-collapse:collapSe;border:none;mso-border-alt:solid windowtext .5pt;mso-yfti-tbllook:1184;mso-padding-alt:0cm 5.4pt 0cm 5.4pt">
         *   <tr>
         *      <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm;">type</td>
         *      <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm;">oid</td>
         *      <td style="border:solid windowtext 1.0pt;mso-border-alt:solid windowtext .5pt;padding:0cm;">class</td>
         *   </tr>
         *   <tr>
         *      <td>data</td>
         *      <td>1.2.840.113549.1.7.1</td>
         *      <td><see cref="CmsDataContentInfo"/></td>
         *   </tr>
         *   <tr>
         *      <td>signed-data</td>
         *      <td>1.2.840.113549.1.7.2</td>
         *      <td><see cref="CmsSignedDataContentInfo"/></td>
         *   </tr>
         *   <tr>
         *      <td>enveloped-data</td>
         *      <td>1.2.840.113549.1.7.3</td>
         *      <td><see cref="CmsEnvelopedDataContentInfo"/></td>
         *   </tr>
         *   <tr>
         *      <td>digested-data</td>
         *      <td>1.2.840.113549.1.7.5</td>
         *      <td><see cref="CmsDigestedDataContentInfo"/></td>
         *   </tr>
         *   <tr>
         *      <td>encrypted-data</td>
         *      <td>1.2.840.113549.1.7.6</td>
         *      <td><see cref="CmsEncryptedDataContentInfo"/></td>
         *   </tr>
         * </table>
         * </remarks>
         */
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))]
        [Order(-1)]
        public Oid ContentType { get; }

        public String Thumbprint { get {
            if (thumbprint == null) {
                using (var engine = SHA1.Create())
                using(var output = new MemoryStream()) {
                    UnderlyingObject.WriteTo(output);
                    output.Seek(0, SeekOrigin.Begin);
                    thumbprint = engine.ComputeHash(output).ToString("x");
                    }
                }
            return thumbprint;
            }}

        /**
         * <summary>
         * Constructs new instance of <see cref="CmsMessage"/> class by using an array of byte values as the content data.
         * </summary>
         * <param name="content">An array of byte values that represents the data from which to create the <see cref="CmsMessage"/> object.</param>
         * <x:block xmlns:x="http://xmldoc.schemas.helix.global" x:lang="ru-RU">
         *   <summary>
         *   Constructs new instance of <see cref="CmsMessage"/> class by using an array of byte values as the content data.
         *   </summary>
         *   <param name="content">Массив байт из которых будет сформировано содержимое объекта <see cref="CmsMessage"/>.</param>
         * </x:block>
         */
        public CmsMessage(Byte[] content)
            :this(Load(new ReadOnlyMemoryMappingStream(content)).FirstOrDefault())
            {
            }

        /**
         * <summary>
         * Constructs new instance of <see cref="CmsMessage"/> class by using an ASN1 object as the source data.
         * </summary>
         * <param name="source">An ASN1 object that represents the data from which to create the <see cref="CmsMessage"/> object.</param>
         */
        public CmsMessage(Asn1Object source)
            : base(source)
            {
            State |= ObjectState.Failed;
            State &= ~ObjectState.DisposeUnderlyingObject;
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if ((source.Class == Asn1ObjectClass.Universal) && (source is Asn1Sequence)) {
                using (new TraceScope()) {
                    if (source.Count <= 1) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    if (!(source[0] is Asn1ObjectIdentifier))       { throw new ArgumentOutOfRangeException(nameof(source)); }
                    if (!(source[1] is Asn1ContextSpecificObject))  { throw new ArgumentOutOfRangeException(nameof(source)); }
                    if (((Asn1ContextSpecificObject)source[1]).Type != 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    ContentType = new Oid(((Asn1ObjectIdentifier)source[0]).ToString());
                    switch (((Asn1ObjectIdentifier)source[0]).ToString())
                        {
                        case OID_Data:          { ContentInfo = new CmsDataContentInfo(source[1]);          } break;
                        case OID_SignedData:    { ContentInfo = new CmsSignedDataContentInfo(source[1]);    } break;
                        case OID_EnvelopedData: { ContentInfo = new CmsEnvelopedDataContentInfo(source[1]); } break;
                        case OID_SignEnvData:   { ContentInfo = new CmsSignEnvDataContentInfo(source[1]);   } break;
                        case OID_DigestedData:  { ContentInfo = new CmsDigestedDataContentInfo(source[1]);  } break;
                        case OID_EncryptedData: { ContentInfo = new CmsEncryptedDataContentInfo(source[1]); } break;
                        default : { throw new ArgumentOutOfRangeException(nameof(source)); }
                        }
                    }
                State &= ~ObjectState.Failed;
                State |= ObjectState.DisposeUnderlyingObject;
                }
            }

        private const String OID_Data                                                                     = "1.2.840.113549.1.7.1";
        private const String OID_SignedData                                                               = "1.2.840.113549.1.7.2";
        private const String OID_EnvelopedData                                                            = "1.2.840.113549.1.7.3";
        private const String OID_SignEnvData                                                              = "1.2.840.113549.1.7.4";
        private const String OID_DigestedData                                                             = "1.2.840.113549.1.7.5";
        private const String OID_EncryptedData                                                            = "1.2.840.113549.1.7.6";

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(ContentType), ContentType.Value);
            writer.WriteValue(serializer, nameof(ContentInfo), ContentInfo);
            writer.WriteEndObject();
            }

        /**
         * <summary>Gets the service object of the specified type.</summary>
         * <param name="service">An object that specifies the type of service object to get.</param>
         * <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Object GetService(Type service)
            {
            if (service == typeof(CmsSignedDataContentInfo)) { return ContentInfo as CmsSignedDataContentInfo; }
            return base.GetService(service);
            }

        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}

        String IFileService.FileName { get { return $"none.p7b"; }}
        String IFileService.FullName { get { return ((IFileService)this).FileName; }}

        Byte[] IFileService.ReadAllBytes() {
            using (var r = new MemoryStream()) {
                WriteTo(r);
                return r.ToArray();
                }
            }

        Stream IFileService.OpenRead()
            {
            return new MemoryStream(((IFileService)this).ReadAllBytes());
            }

        void IFileService.MoveTo(String target)
            {
            ((IFileService)this).MoveTo(target, false);
            }

        /// <summary>Move an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.MoveTo(String target, Boolean overwrite)
            {
            ((IFileService)this).CopyTo(target, overwrite);
            }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.CopyTo(String target, Boolean overwrite)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = ((IFileService)this).OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!String.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("CmsMessage");
            writer.WriteAttributeString("ContentType", ContentType.Value);
            if (ContentInfo != null) {
                writer.WriteStartElement("CmsMessage.ContentInfo");
                ContentInfo.WriteXml(writer);
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }
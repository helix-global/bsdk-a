using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using BinaryStudio.DataProcessing;
using BinaryStudio.IO;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
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
    public sealed class CmsMessage : CmsObject
        {
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
        }
    }
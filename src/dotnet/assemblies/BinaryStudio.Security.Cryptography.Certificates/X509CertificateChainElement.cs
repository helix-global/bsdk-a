using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using BinaryStudio.Diagnostics;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    /// <summary>
    /// <see cref="X509CertificateChainElement"/> class represents a structure of a single element in a simple certificate chain.
    /// </summary>
    [Serializable]
    public class X509CertificateChainElement :
            IExceptionSerializable,
            IX509CertificateChainElement,
            ISerializable
        {
        /// <summary>
        /// Certificate associated with current chain element.
        /// </summary>
        public IX509Certificate Certificate { get; }

        /// <summary>
        /// CRL associated with current chain element.
        /// </summary>
        public IX509CertificateRevocationList CertificateRevocationList { get; }

        /// <summary>Initializes a new instance of the <see cref="X509CertificateChainElement"/> class from specified source.</summary>
        /// <param name="source">Source of chain element.</param>
        /// <param name="index">Element index.</param>
        internal unsafe X509CertificateChainElement(CERT_CHAIN_ELEMENT* source, Int32 index) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            ErrorStatus = source->TrustStatus.ErrorStatus;
            InfoStatus  = source->TrustStatus.InfoStatus;
            ElementIndex = index;
            if (source->CertContext != null) {
                if (source->CertContext->CertEncodedSize > 0) {
                    Certificate = new X509Certificate(source->CertContext);
                    }
                }
            if (source->RevocationInfo != null) {
                if ((source->RevocationInfo->CrlInfo != null) &&
                    (source->RevocationInfo->CrlInfo->BaseCrlContext != null) &&
                    (source->RevocationInfo->CrlInfo->BaseCrlContext->CrlEncodedSize > 0)) {
                    CertificateRevocationList = new X509CertificateRevocationList(source->RevocationInfo->CrlInfo->BaseCrlContext);
                    }
                }
            }

        /// <summary>
        /// Element index.
        /// </summary>
        public Int32 ElementIndex { get; }

        /// <summary>
        /// Error status codes.
        /// </summary>
        public CertificateChainErrorStatus ErrorStatus { get; }

        /// <summary>
        /// Information status codes.
        /// </summary>
        public CertificateChainInfoStatus InfoStatus { get; }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            info.AddValue(nameof(ElementIndex), ElementIndex);
            info.AddValue(nameof(ErrorStatus),  (Int32)ErrorStatus);
            info.AddValue(nameof(InfoStatus),   (Int32)InfoStatus);
            info.AddValue(nameof(Certificate),  (Certificate != null) ? (Int64)Certificate.Handle : 0);
            info.AddValue(nameof(CertificateRevocationList),  (CertificateRevocationList != null) ? (Int64)CertificateRevocationList.Handle : 0);
            }

        /// <summary>The special constructor is used to deserialize values.</summary>
        /// <param name="info">The data needed to deserialize an object.</param>
        /// <param name="context">Describes the source of a given serialized stream, and provides an additional caller-defined context.</param>
        protected X509CertificateChainElement(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            ElementIndex = info.GetInt32(nameof(ElementIndex));
            ErrorStatus = (CertificateChainErrorStatus)info.GetInt32(nameof(ErrorStatus));
            InfoStatus  = (CertificateChainInfoStatus)info.GetInt32(nameof(InfoStatus));
            var cer = info.GetInt64(nameof(Certificate));
            var crl = info.GetInt64(nameof(CertificateRevocationList));
            if (cer != 0) { Certificate = new X509Certificate((IntPtr)cer); }
            if (crl != 0) { CertificateRevocationList = new X509CertificateRevocationList((IntPtr)crl); }
            }

        void IExceptionSerializable.WriteTo(TextWriter target)
            {
            using (var writer = new JsonTextWriter(target){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    }) {
                var serializer = new JsonSerializer();
                ((IExceptionSerializable)this).WriteTo(writer, serializer);
                writer.Flush();
                }
            }

        void IExceptionSerializable.WriteTo(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(ElementIndex), ElementIndex);
                writer.WriteValue(serializer, nameof(ErrorStatus),  ErrorStatus);
                writer.WriteValue(serializer, nameof(InfoStatus),   InfoStatus);
                var cer = Certificate;
                var crl = CertificateRevocationList;
                if (cer != null) {
                    writer.WriteValue(serializer, nameof(Certificate), $"SerialNumber:{{{cer.SerialNumber.ToLowerInvariant()}}},Subject:{{{cer.Subject}}},Issuer:{{{cer.Issuer}}}");
                    }
                if (crl != null) {
                    var r = new StringBuilder();
                    r.Append($"EffectiveDate:{{{crl.EffectiveDate:yyyy-MM-ddThh:mm:ss}}},");
                    if (crl.NextUpdate.HasValue)
                        {
                        r.Append($"NextUpdate:{{{crl.NextUpdate.Value:yyyy-MM-ddThh:mm:ss}}},");
                        }
                    r.Append($"Issuer:{{{crl.Issuer}}}");
                    writer.WriteValue(serializer, "CRL", r.ToString());
                    }
                }
            }
        }
    }
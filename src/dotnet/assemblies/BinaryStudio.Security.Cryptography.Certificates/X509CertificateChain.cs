using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    /// <summary>
    /// <see cref="X509CertificateChain"/> class represents a structure of a certificate chain.
    /// </summary>
    [Serializable]
    public class X509CertificateChain :
            IReadOnlyList<X509CertificateChainElement>,
            IExceptionSerializable,
            IX509CertificateChainStatus,
            ISerializable
        {
        private readonly IList<X509CertificateChainElement> source = new List<X509CertificateChainElement>();

        /// <summary>Initializes a new instance of the <see cref="X509CertificateChain"/> class from specified source.</summary>
        /// <param name="source">Source of chain data.</param>
        /// <param name="index">Chain index.</param>
        internal unsafe X509CertificateChain(CERT_SIMPLE_CHAIN* source, Int32 index) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            ChainIndex = index;
            ErrorStatus = source->TrustStatus.ErrorStatus;
            InfoStatus  = source->TrustStatus.InfoStatus;
            if ((source->ElementCount > 0) && (source->ElementArray != null)) {
                for (var i = 0; i < source->ElementCount; i++) {
                    this.source.Add(new X509CertificateChainElement(source->ElementArray[i], i));
                    }
                }
            }

        /// <summary>
        /// Error status codes.
        /// </summary>
        public CertificateChainErrorStatus ErrorStatus { get; }

        /// <summary>
        /// Information status codes.
        /// </summary>
        public CertificateChainInfoStatus InfoStatus { get; }

        /// <summary>
        /// Chain index.
        /// </summary>
        public Int32 ChainIndex { get; }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<X509CertificateChainElement> GetEnumerator()
            {
            return source.GetEnumerator();
            }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public Int32 Count { get { return source.Count; }}

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        public X509CertificateChainElement this[Int32 index]
            {
            get { return source[index]; }
            }

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            var c = Count;
            info.AddValue(nameof(ChainIndex),  ChainIndex);
            info.AddValue(nameof(ErrorStatus), (Int32)ErrorStatus);
            info.AddValue(nameof(InfoStatus),  (Int32)InfoStatus);
            info.AddValue(nameof(Count),       c);
            var j = 0;
            foreach (var i in source) {
                info.AddValue($"Item_{j}", i);
                j++;
                }
            }

        /// <summary>The special constructor is used to deserialize values.</summary>
        /// <param name="info">The data needed to deserialize an object.</param>
        /// <param name="context">Describes the source of a given serialized stream, and provides an additional caller-defined context.</param>
        protected X509CertificateChain(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            ChainIndex = info.GetInt32(nameof(ChainIndex));
            ErrorStatus = (CertificateChainErrorStatus)info.GetInt32(nameof(ErrorStatus));
            InfoStatus  = (CertificateChainInfoStatus)info.GetInt32(nameof(InfoStatus));
            var c = info.GetInt32(nameof(Count));
            for (var i = 0; i < c; i++) {
                source.Add((X509CertificateChainElement)info.GetValue(
                    $"Item_{i}",
                    typeof(X509CertificateChainElement)));
                }
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
                writer.WriteValue(serializer, nameof(ChainIndex),  ChainIndex);
                writer.WriteValue(serializer, nameof(ErrorStatus), ErrorStatus);
                writer.WriteValue(serializer, nameof(InfoStatus),  InfoStatus);
                var c = Count;
                if (c > 0) {
                    writer.WritePropertyName("[Self]");
                    using (writer.ArrayScope(serializer)) {
                        var j = 0;
                        foreach (var chainE in source) {
                            var certificate = chainE.Certificate;
                            writer.WriteValue($"Order:{{ {j}}}:SerialNumber:{{{certificate.SerialNumber.ToLowerInvariant()}}},Subject:{{{certificate.Subject}}},Issuer:{{{certificate.Issuer}}}");
                            if ((j == c - 1) && (ErrorStatus.HasFlag(CertificateChainErrorStatus.CERT_TRUST_IS_PARTIAL_CHAIN))) {
                                var a = certificate.Extensions.OfType<CertificateAuthorityKeyIdentifier>().FirstOrDefault();
                                var r = new StringBuilder();
                                r.Append($"Order:{{*{j + 1}}}:");
                                if (a != null) {
                                    r.Append((a.SerialNumber != null)
                                        ? $"SerialNumber:{{{a.SerialNumber.ToLowerInvariant()}}},"
                                        : $"SubjectKeyIdentifier:{{{a.KeyIdentifier.ToString("x")}}},");
                                    }
                                writer.WriteValue($"{r}Subject:{{{certificate.Issuer}}}");
                                }
                            j++;
                            }
                        }
                    }
                }
            }
        }
    }
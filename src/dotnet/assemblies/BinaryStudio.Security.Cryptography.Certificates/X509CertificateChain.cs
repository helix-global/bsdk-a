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
            IReadOnlyList<IX509CertificateChainElement>,
            IExceptionSerializable,
            ISerializable,
            IX509CertificateChain
        {
        private readonly IList<IX509CertificateChainElement> source = new List<IX509CertificateChainElement>();

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
        public IEnumerator<IX509CertificateChainElement> GetEnumerator()
            {
            return source.GetEnumerator();
            }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        void ICollection<IX509CertificateChainElement>.Add(IX509CertificateChainElement item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        void ICollection<IX509CertificateChainElement>.Clear()
            {
            throw new NotSupportedException();
            }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        Boolean ICollection<IX509CertificateChainElement>.Contains(IX509CertificateChainElement item)
            {
            return source.Contains(item);
            }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        void ICollection<IX509CertificateChainElement>.CopyTo(IX509CertificateChainElement[] array, Int32 arrayIndex)
            {
            source.CopyTo(array, arrayIndex);
            }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        Boolean ICollection<IX509CertificateChainElement>.Remove(IX509CertificateChainElement item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public Int32 Count { get { return source.Count; }}

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        Boolean ICollection<IX509CertificateChainElement>.IsReadOnly
            {
            get { return true; }
            }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        Int32 IList<IX509CertificateChainElement>.IndexOf(IX509CertificateChainElement item)
            {
            return source.IndexOf(item);
            }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        void IList<IX509CertificateChainElement>.Insert(Int32 index, IX509CertificateChainElement item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        void IList<IX509CertificateChainElement>.RemoveAt(Int32 index)
            {
            throw new NotSupportedException();
            }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        IX509CertificateChainElement IList<IX509CertificateChainElement>.this[Int32 index]
            {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
            }

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        public IX509CertificateChainElement this[Int32 index]
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
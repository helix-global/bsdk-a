using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BinaryStudio.Diagnostics;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CertificateChainContext :
            IReadOnlyList<X509CertificateChain>,
            IExceptionSerializable,
            IX509CertificateChainStatus
        {
        private readonly IList<X509CertificateChain> chains = new List<X509CertificateChain>();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<X509CertificateChain> GetEnumerator()
            {
            return chains.GetEnumerator();
            }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public Int32 Count { get { return chains.Count; }}

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        public X509CertificateChain this[Int32 index]
            {
            get { return chains[index]; }
            }

        /// <summary>
        /// Error status codes.
        /// </summary>
        public CertificateChainErrorStatus ErrorStatus { get; }

        /// <summary>
        /// Information status codes.
        /// </summary>
        public CertificateChainInfoStatus  InfoStatus  { get; }

        /// <summary>Initializes a new instance of the <see cref="X509CertificateChainContext"/> class from specified source.</summary>
        /// <param name="context">Source of chain context.</param>
        internal unsafe X509CertificateChainContext(ref CERT_CHAIN_CONTEXT context) {
            ErrorStatus = context.TrustStatus.ErrorStatus;
            InfoStatus  = context.TrustStatus.InfoStatus;
            if ((context.ChainCount > 0) && (context.ChainArray != null)) {
                for (var i = 0; i < context.ChainCount; i++) {
                    var chain = context.ChainArray[i];
                    if (chain != null)
                        {
                        chains.Add(new X509CertificateChain(chain, i));
                        }
                    }
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

        void IExceptionSerializable.WriteTo(JsonWriter writer, JsonSerializer serializer)
            {
            using (writer.ObjectScope(serializer)) {
                }
            }
        }
    }
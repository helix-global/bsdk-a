using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using BinaryStudio.Diagnostics;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CertificateChainContext :
            IReadOnlyList<IX509CertificateChain>,
            IExceptionSerializable,
            IX509CertificateChainContext
        {
        private unsafe CERT_CHAIN_CONTEXT* ChainContext = null;
        private readonly IList<IX509CertificateChain> chains = new List<IX509CertificateChain>();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IX509CertificateChain> GetEnumerator()
            {
            return chains.GetEnumerator();
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
        void ICollection<IX509CertificateChain>.Add(IX509CertificateChain item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        void ICollection<IX509CertificateChain>.Clear()
            {
            throw new NotSupportedException();
            }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        Boolean ICollection<IX509CertificateChain>.Contains(IX509CertificateChain item)
            {
            return chains.Contains(item);
            }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        void ICollection<IX509CertificateChain>.CopyTo(IX509CertificateChain[] array, Int32 arrayIndex)
            {
            chains.CopyTo(array, arrayIndex);
            }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        Boolean ICollection<IX509CertificateChain>.Remove(IX509CertificateChain item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection.</returns>
        public Int32 Count { get { return chains.Count; }}

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        Boolean ICollection<IX509CertificateChain>.IsReadOnly {
            get { return true; }
            }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        Int32 IList<IX509CertificateChain>.IndexOf(IX509CertificateChain item)
            {
            return chains.IndexOf(item);
            }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        void IList<IX509CertificateChain>.Insert(Int32 index, IX509CertificateChain item)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        void IList<IX509CertificateChain>.RemoveAt(Int32 index)
            {
            throw new NotSupportedException();
            }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
        IX509CertificateChain IList<IX509CertificateChain>.this[Int32 index]
            {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
            }

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        public IX509CertificateChain this[Int32 index]
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
        internal unsafe X509CertificateChainContext(ref CERT_CHAIN_CONTEXT context)
            {
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

        /// <summary>Initializes a new instance of the <see cref="X509CertificateChainContext"/> class from specified source.</summary>
        /// <param name="context">Source of chain context.</param>
        internal unsafe X509CertificateChainContext(CERT_CHAIN_CONTEXT* context) {
            ChainContext = context;
            ErrorStatus = context->TrustStatus.ErrorStatus;
            InfoStatus  = context->TrustStatus.InfoStatus;
            if ((context->ChainCount > 0) && (context->ChainArray != null)) {
                for (var i = 0; i < context->ChainCount; i++) {
                    var chain = context->ChainArray[i];
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

        #region M:Dispose(Boolean)
        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual unsafe void Dispose(Boolean disposing)
            {
            if (ChainContext != null) {
                //CertFreeCertificateChain(ChainContext);
                ChainContext = null;
                }
            }
        #endregion
        #region M:Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~X509CertificateChainContext()
            {
            Dispose(false);
            }
        #endregion

        [DllImport("crypt32.dll", SetLastError = true)][SuppressUnmanagedCodeSecurity][ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] private static extern unsafe void CertFreeCertificateChain(CERT_CHAIN_CONTEXT* ChainContext);
        }
    }
using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportCollection<K,T> : IEnumerable<KeyValuePair<K,T>>
        {
        private readonly IDictionary<K,T> values = new SortedDictionary<K,T>();
        private readonly IReportElementFactory<T> factory;

        public ReportCollection(IReportElementFactory<T> factory)
            {
            if (factory == null) { throw new ArgumentNullException(nameof(factory)); }
            this.factory = factory;
            }

        public Int32 Count { get
            {
            return values.Count;
            }}

        public T this[K index] { get {
            if (!values.TryGetValue(index, out var r))
                {
                values.Add(index, r = factory.CreateElement());
                }
            return r;
            }}

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<K,T>> GetEnumerator()
            {
            return values.GetEnumerator();
            }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }
        }
    }
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.ObjectModel
{
    /// <summary>Provides the base class for a generic read-only collection.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    internal class InternalReadOnlyCollection<T> : IReadOnlyCollection<T>, ICollection<T>, ICollection
    {
        private readonly ICollection<T> values;
        private Object syncroot;

        #region P:ICollection<T>.Count:Int32
        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> instance.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> instance.</returns>
        public Int32 Count { get {
            return values.Count;
            }}
        #endregion
        #region P:ICollection<T>.IsReadOnly:Boolean
        Boolean ICollection<T>.IsReadOnly { get {
            return true;
            }}
        #endregion
        #region P:ICollection.IsSynchronized:Boolean
        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.  In the default implementation of <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />, this property always returns false.</returns>
        Boolean ICollection.IsSynchronized { get {
            return false;
            }}
        #endregion
        #region P:ICollection.SyncRoot:Boolean
        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />, this property always returns the current instance.</returns>
        Object ICollection.SyncRoot { get {
            if (syncroot == null)
                {
                var collections = values as ICollection;
                if (collections == null)
                    {
                    Interlocked.CompareExchange<Object>(ref syncroot, new Object(), null);
                    }
                else
                    {
                    syncroot = collections.SyncRoot;
                    }
                }
            return syncroot;
            }}
        #endregion

        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> class that is a read-only wrapper around the specified list.</summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="list" /> is null.</exception>
        public InternalReadOnlyCollection(ICollection<T> list)
            {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }
            values = list;
            }

        #region M:Contains(T):Boolean
        /// <summary>Determines whether an element is in the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</summary>
        /// <returns>true if <paramref name="value" /> is found in the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />; otherwise, false.</returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />. The value can be null for reference types.</param>
        public Boolean Contains(T value)
            {
            return values.Contains(value);
            }
        #endregion
        #region M:CopyTo(T[],Int32)
        /// <summary>Copies the entire <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(T[] array, Int32 index)
            {
            values.CopyTo(array, index);
            }
        #endregion
        #region M:GetEnumerator:IEnumerator<T>
        /// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> for the <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" />.</returns>
        public IEnumerator<T> GetEnumerator()
            {
            return values.GetEnumerator();
            }
        #endregion
        #region M:IsCompatibleObject(Object):Boolean
        private static Boolean IsCompatibleObject(Object value) {
            if (value is T)    { return true;  }
            if (value != null) { return false; }
            return default(T) == null;
            }
        #endregion
        #region M:ICollection<T>.Add(T)
        void ICollection<T>.Add(T value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection<T>.Clear
        void ICollection<T>.Clear()
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection<T>.Remove(T):Boolean
        Boolean ICollection<T>.Remove(T value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection.CopyTo(Array,Int32)
        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="array" /> is multidimensional.-or-<paramref name="array" /> does not have zero-based indexing.-or-The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.-or-The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        void ICollection.CopyTo(Array array, Int32 index)
            {
            var items = array as T[];
            if (items != null) {
                values.CopyTo(items, index);
                }
            else
                {
                var src = values as ICollection;
                if (src != null) {
                    src.CopyTo(array, index);
                    }
                else
                    {
                    var objects = array as Object[];
                    if (objects == null) { throw new NotSupportedException(); }
                    foreach (var value in values)
                        {
                        objects[index++] = value;
                        }
                    }
                }
            }
        #endregion
        #region M:IEnumerable.GetEnumerator:IEnumerator
        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            {
            return values.GetEnumerator();
            }
        #endregion
        }
    }

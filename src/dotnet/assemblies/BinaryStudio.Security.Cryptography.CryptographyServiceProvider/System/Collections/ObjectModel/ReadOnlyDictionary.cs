
using System.Diagnostics.CodeAnalysis;
#if NET40 || NET35
using System.Collections.Generic;
using System.Threading;

namespace System.Collections.ObjectModel
    {
    [Serializable]
    #if CODE_ANALYSIS
    [SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "<Pending>")]
    #endif
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>
    {
        private Object m_syncRoot;
        private KeyCollection m_keys;
        private ValueCollection m_values;

        private void EnsureKeys() {
            if (m_keys == null) { m_keys = new KeyCollection(Dictionary.Keys); }
            }

        private void EnsureValues() {
            if (m_values == null) { m_values = new ValueCollection(Dictionary.Values); }
            }

        #region P:Dictionary:IDictionary<TKey,TValue>
        private IDictionary<TKey,TValue> Dictionary { get; }
        #endregion
        #region P:IReadOnlyDictionary<TKey,TValue>.Count:Int32
        public Int32 Count { get {
            return Dictionary.Count;
            }}
        #endregion
        #region P:IReadOnlyDictionary<TKey,TValue>.this[TKey]:TValue
        public TValue this[TKey key] { get {
            return Dictionary[key];
            }}
        #endregion
        #region P:Keys:KeyCollection
        public ICollection<TKey> Keys { get {
            EnsureKeys();
            return m_keys;
            }}
        #endregion
        #region P:ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly:Boolean
        Boolean ICollection<KeyValuePair<TKey,TValue>>.IsReadOnly { get { return true; }}
        #endregion
        #region P:IDictionary<TKey,TValue>.this[TKey]:TValue
        TValue IDictionary<TKey,TValue>.this[TKey key] {
            get { return Dictionary[key]; }
            set { throw new NotSupportedException(); }
            }
        #endregion
        #region P:IDictionary<TKey,TValue>.Keys:ICollection<TKey>
        ICollection<TKey> IDictionary<TKey,TValue>.Keys { get {
            return Keys;
            }}
        #endregion
        #region P:IDictionary<TKey,TValue>.Values:ICollection<TValue>
        ICollection<TValue> IDictionary<TKey,TValue>.Values { get {
            return Values;
            }}
        #endregion
        #region P:IReadOnlyDictionary<TKey,TValue>.Keys:IEnumerable<TKey>
        IEnumerable<TKey> IReadOnlyDictionary<TKey,TValue>.Keys { get {
            return Keys;
            }}
        #endregion
        #region P:IReadOnlyDictionary<TKey,TValue>.Values:IEnumerable<TValue>
        IEnumerable<TValue> IReadOnlyDictionary<TKey,TValue>.Values { get {
            return Values;
            }}
        #endregion
        #region P:ICollection.IsSynchronized:Boolean
        Boolean ICollection.IsSynchronized { get {
            return false;
            }}
        #endregion
        #region P:ICollection.SyncRoot:Boolean
        Object ICollection.SyncRoot
        {
            get
            {
                if (m_syncRoot == null)
                {
                    var mDictionary = Dictionary as ICollection;
                    if (mDictionary == null)
                    {
                        Interlocked.CompareExchange<Object>(ref m_syncRoot, new Object(), null);
                    }
                    else
                    {
                        m_syncRoot = mDictionary.SyncRoot;
                    }
                }
                return m_syncRoot;
            }
        }
        #endregion
        #region P:IDictionary.IsFixedSize:Boolean
        Boolean IDictionary.IsFixedSize { get {
            return true;
            }}
        #endregion
        #region P:IDictionary.IsReadOnly:Boolean
        Boolean IDictionary.IsReadOnly { get {
            return true;
            }}
        #endregion
        #region P:IDictionary.this[Object]:Object
        Object IDictionary.this[Object key] {
            get
                {
                if (!IsCompatibleKey(key)) { return null; }
                return this[(TKey)key];
                }
            set { throw new NotSupportedException(); }
            }
        #endregion
        #region P:IDictionary.Keys:ICollection
        ICollection IDictionary.Keys { get {
            EnsureKeys();
            return m_keys;
            }}
        #endregion
        #region P:IDictionary.Values:ICollection
        ICollection IDictionary.Values { get {
            EnsureValues();
            return m_values;
            }}
        #endregion
        #region P:Values:ValueCollection
        public ICollection<TValue> Values { get {
            EnsureValues();
            return m_values;
            }}
        #endregion

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
            {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            Dictionary = dictionary;
            }

        #region M:ContainsKey(TKey):Boolean
        public Boolean ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }
        #endregion
        #region M:GetEnumerator:IEnumerator<KeyValuePair<TKey,TValue>>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion
        #region M:IsCompatibleKey(Object):Boolean
        private static Boolean IsCompatibleKey(Object key)
            {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            return key is TKey;
            }
        #endregion
        #region M:ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey,TValue>)
        void ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey, TValue> item)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection<KeyValuePair<TKey,TValue>>.Clear
        void ICollection<KeyValuePair<TKey,TValue>>.Clear()
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection<KeyValuePair<TKey,TValue>>.Contains(KeyValuePair<TKey,TValue>):Boolean
        Boolean ICollection<KeyValuePair<TKey,TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            {
            return Dictionary.Contains(item);
            }
        #endregion
        #region M:ICollection<KeyValuePair<TKey,TValue>>.CopyTo(KeyValuePair<TKey,TValue>[],Int32)
        void ICollection<KeyValuePair<TKey,TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }
        #endregion
        #region M:ICollection<KeyValuePair<TKey,TValue>>.Remove(KeyValuePair<TKey,TValue>):Boolean
        Boolean ICollection<KeyValuePair<TKey,TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IDictionary<TKey,TValue>.Add(TKey,TValue)
        void IDictionary<TKey,TValue>.Add(TKey key, TValue value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IDictionary<TKey,TValue>.Remove(TKey):Boolean
        Boolean IDictionary<TKey,TValue>.Remove(TKey key)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:ICollection.CopyTo(Array,Int32)
        void ICollection.CopyTo(Array array, Int32 index)
            {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }
            if (array.Rank != 1) { throw new ArgumentOutOfRangeException(nameof(array)); }
            if (array.GetLowerBound(0) != 0) { throw new ArgumentOutOfRangeException(nameof(array)); }
            if (index < 0 || index > array.Length) { throw new ArgumentOutOfRangeException(nameof(index)); }
            if (array.Length - index < Count) { throw new ArgumentOutOfRangeException(nameof(index)); }
            var keyValuePairArray = array as KeyValuePair<TKey, TValue>[];
            if (keyValuePairArray != null)
                {
                Dictionary.CopyTo(keyValuePairArray, index);
                return;
                }
            var dictionaryEntry = array as DictionaryEntry[];
            if (dictionaryEntry == null) {
                var keyValuePair = array as Object[];
                if (keyValuePair == null) { throw new ArgumentOutOfRangeException(nameof(array)); }
                foreach (var mDictionary in Dictionary) {
                    var num = index;
                    index = num + 1;
                    keyValuePair[num] = new KeyValuePair<TKey, TValue>(mDictionary.Key, mDictionary.Value);
                    }
                }
            else
                {
                foreach (var mDictionary1 in Dictionary)
                    {
                    var num1 = index;
                    index = num1 + 1;
                    dictionaryEntry[num1] = new DictionaryEntry((Object)mDictionary1.Key, (Object)mDictionary1.Value);
                    }
                }
            }
        #endregion
        #region M:IDictionary.Add(Object,Object)
        void IDictionary.Add(Object key, Object value)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IDictionary.Clear
        void IDictionary.Clear()
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IDictionary.Contains(Object):Boolean
        Boolean IDictionary.Contains(Object key)
        {
            if (!IsCompatibleKey(key))
            {
                return false;
            }
            return ContainsKey((TKey)key);
        }
        #endregion
        #region M:IDictionary.GetEnumerator:IDictionaryEnumerator
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            var mDictionary = Dictionary as IDictionary;
            if (mDictionary != null)
            {
                return mDictionary.GetEnumerator();
            }
            return new DictionaryEnumerator(Dictionary);
        }
        #endregion
        #region M:IDictionary.Remove(Object)
        void IDictionary.Remove(Object key)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IEnumerable.GetEnumerator:IEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion
        #region M:TryGetValue(TKey,out TValue):Boolean
        public Boolean TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }
        #endregion

        private struct DictionaryEnumerator : IDictionaryEnumerator
            {
            private readonly IDictionary<TKey, TValue> m_dictionary;
            private IEnumerator<KeyValuePair<TKey, TValue>> m_enumerator;

            public Object Current
                {
                get
                    {
                    return Entry;
                    }
                }

            public DictionaryEntry Entry
                {
                get
                    {
                    var current = m_enumerator.Current;
                    Object key = current.Key;
                    current = m_enumerator.Current;
                    return new DictionaryEntry(key, (Object)current.Value);
                    }
                }

            public Object Key
                {
                get
                    {
                    return m_enumerator.Current.Key;
                    }
                }

            public Object Value
                {
                get
                    {
                    return m_enumerator.Current.Value;
                    }
                }

            public DictionaryEnumerator(IDictionary<TKey, TValue> dictionary)
                {
                m_dictionary = dictionary;
                m_enumerator = m_dictionary.GetEnumerator();
                }

            public Boolean MoveNext()
                {
                return m_enumerator.MoveNext();
                }

            public void Reset()
                {
                m_enumerator.Reset();
                }
            }

        private sealed class KeyCollection : InternalReadOnlyCollection<TKey>
            {
            public KeyCollection(ICollection<TKey> list)
                : base(list)
                {
                }
            }

        private sealed class ValueCollection : InternalReadOnlyCollection<TValue>
            {
            public ValueCollection(ICollection<TValue> list)
                : base(list)
                {
                }
            }
        }
    }
#endif
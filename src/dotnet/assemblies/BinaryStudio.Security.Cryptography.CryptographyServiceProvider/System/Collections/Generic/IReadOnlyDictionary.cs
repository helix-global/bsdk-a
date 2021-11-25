#if NET40 || NET35
namespace System.Collections.Generic
    {
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
        {
        TValue this[TKey key] { get; }
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }
        Boolean ContainsKey(TKey key);
        Boolean TryGetValue(TKey key, out TValue value);
        }
    }
#endif
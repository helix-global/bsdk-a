#if NET40
namespace System.Collections.Generic
    {
    /// <summary>Represents a strongly-typed, read-only collection of elements.</summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
        {
	    /// <summary>Gets the number of elements in the collection.</summary>
	    /// <returns>The number of elements in the collection.</returns>
        Int32 Count { get; }
        }
    }
#endif
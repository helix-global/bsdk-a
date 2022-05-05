using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinaryStudio.PlatformComponents
    {
    internal class EmptyArray<T>
        {
        public static readonly T[] Value = new T[0];
        }

    internal class EmptyList<T>
        {
        public static readonly IList<T> Value = new ReadOnlyCollection<T>(EmptyArray<T>.Value);
        }
    }
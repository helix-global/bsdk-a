using System;
using System.Collections.Generic;

namespace BinaryStudio.DataProcessing
    {
    public static class Extensions
        {
        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/> if its not null.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="value">The object to add to the <see cref="ICollection{T}"/>.</param>
        public static void AddIfNotNull<T>(this ICollection<T> source, T value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (value != null) {
                source.Add(value);
                }
            }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="condition">If <see langword="true"/> then <paramref name="value"/> will be added into collection.</param>
        /// <param name="value">The object to add to the <see cref="ICollection{T}"/>.</param>
        public static void Add<T>(this IList<T> source, Boolean condition, T value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (condition) {
                source.Add(value);
                }
            }
        }
    }
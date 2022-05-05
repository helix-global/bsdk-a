using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace BinaryStudio.DataProcessing
    {
    public class DBQuery : IQueryable, IList
        {
        private Int32 CachedCount = -1;
        private readonly String AliasSuffix;
        private const Int32 FrameSize = 100;
        private readonly IDictionary<Int32,DBQueryRow> Cache = new Dictionary<Int32, DBQueryRow>();

        private class QueryFrame
            {
            public Int32 Offset;
            public IList<DBQueryRow> Values;
            public Int32 Size { get { return Values.Count; }}
            public Boolean Contains(Int32 index)
                {
                return (index >= Offset) && (index < Offset + Values.Count);
                }
            }

        public DbCommand Source { get; }
        public DBQuery(DbCommand source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Connection == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
            Columns = (new DataTable()).Columns;
            Source = source;
            AliasSuffix = $"alias_{Guid.NewGuid():N}";
            ElementType = typeof(Object);
            Expression = Expression.Constant(this);
            using (var command = Source.Connection.CreateCommand()) {
                command.CommandTimeout = Source.CommandTimeout;
                command.CommandText = $@"
                    SELECT
                        TOP 1 *
                    FROM ({Source.CommandText}) a_{AliasSuffix}";
                using (var reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        var fieldcount = reader.FieldCount;
                        for (var i = 0; i < fieldcount; i++) {
                            Columns.Add(new DataColumn(
                                reader.GetName(i),
                                reader.GetFieldType(i) ?? typeof(Object)));
                            }
                        }
                    }
                }
            }

        public IEnumerator GetEnumerator() {
            for (var i = 0; i < FrameSize; i++) {
                yield return this[i];
                }
            }

        public DataColumnCollection Columns { get; }
        public Expression Expression { get; }
        public Type ElementType { get; }
        public IQueryProvider Provider
            {
            get { throw new NotImplementedException(); }
            }

        public void CopyTo(Array array, Int32 index)
            {
            throw new NotImplementedException();
            }

        public Int32 Count { get {
            if (CachedCount == -1) {
                using (var command = Source.Connection.CreateCommand()) {
                    command.CommandTimeout = Source.CommandTimeout;
                    command.CommandText = $@"
                        SELECT COUNT(*)
                        FROM ({Source.CommandText}) a_{AliasSuffix}";
                    CachedCount = (Int32)command.ExecuteScalar();
                    }
                }
            return CachedCount;
            }}


        /// <summary>Adds an item to the <see cref="T:System.Collections.IList"/>.</summary>
        /// <returns>The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,</returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size.</exception>
        /// <filterpriority>2</filterpriority>
        Int32 IList.Add(Object value)
            {
            throw new NotSupportedException();
            }

        /// <summary>Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.</summary>
        /// <returns>true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.</returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>.</param>
        /// <filterpriority>2</filterpriority>
        public Boolean Contains(Object value) {
            if (value == null) { return false; }
            if (value is DBQueryRow row) {
                if (ReferenceEquals(row.Query, this)) {
                    return true;
                    }
                }
            return false;
            }

        /// <summary>Removes all items from the <see cref="T:System.Collections.IList"/>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.</exception>
        /// <filterpriority>2</filterpriority>
        void IList.Clear()
            {
            throw new NotSupportedException();
            }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.</summary>
        /// <returns>The index of <paramref name="value"/> if found in the list; otherwise, -1.</returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <filterpriority>2</filterpriority>
        public Int32 IndexOf(Object value) {
            if (value == null) { return -1; }
            if (value is DBQueryRow row) {
                if (ReferenceEquals(row.Query, this)) {
                    return (Int32)row.ImplicitRowNumber;
                    }
                }
            return -1;
            }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size.</exception>
        /// <exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <filterpriority>2</filterpriority>
        void IList.Insert(Int32 index, Object value)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.</summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"/>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size.</exception>
        /// <filterpriority>2</filterpriority>
        void IList.Remove(Object value)
            {
            throw new NotSupportedException();
            }

        /// <summary>Removes the <see cref="T:System.Collections.IList"/> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size.</exception>
        /// <filterpriority>2</filterpriority>
        void IList.RemoveAt(Int32 index)
            {
            throw new NotSupportedException();
            }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only.</exception>
        /// <filterpriority>2</filterpriority>
        public Object this[Int32 index]
            {
            get
                {
                if (Cache.TryGetValue(index, out var r)) { return r; }
                var frame = LoadFrame(Math.Max(0,index - FrameSize), FrameSize*3);
                var i = frame.Offset;
                foreach (var value in frame.Values) {
                    Cache[i] = value;
                    i++;
                    }
                return Cache[index];
                }
            set { throw new NotSupportedException(); }
            }

        private QueryFrame LoadFrame(Int32 offset, Int32 count) {
            lock(this) {
                if (offset < 0) { return null; }
                var target = new QueryFrame{
                    Offset = offset,
                    Values = new List<DBQueryRow>()
                    };
                Debug.Print($"LoadFrame:{target.Offset}");
                var firstcolumn = Columns[0];
                var fieldcount  = Columns.Count;
                using (var command = Source.Connection.CreateCommand()) {
                    command.CommandTimeout = Source.CommandTimeout;
                    command.CommandText = $@"
                        SELECT * FROM(SELECT
                             ROW_NUMBER() OVER(ORDER by [{firstcolumn.ColumnName}])[implicit_row_index]
                            ,*
                        FROM ({Source.CommandText}) [a_{AliasSuffix}]) [b_{AliasSuffix}]
                        WHERE ([implicit_row_index] >= {target.Offset + 1})
                          AND ([implicit_row_index] <  {target.Offset + 1 + count})";
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var values = new Object[Columns.Count];
                            for (var i = 1; i < fieldcount; i++) {
                                values[i-1] = reader[i];
                                }
                            target.Values.Add(new DBQueryRow(this, Columns,(Int64)reader[0],values));
                            }
                        }
                    }
                return target;
                }
            }

        Boolean IList.IsReadOnly  { get { return true; }}
        Boolean IList.IsFixedSize { get { return true; }}
        Boolean ICollection.IsSynchronized { get { return true; }}
        Object ICollection.SyncRoot { get { return this; }}
        }
}
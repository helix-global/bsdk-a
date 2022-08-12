using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BinaryStudio.DataProcessing
    {
    public class CSVDataReader : IDataReader
        {
        private TextReader Reader;
        private readonly CultureInfo Culture;
        private readonly String Delimiter;
        private readonly List<DataColumn> Columns;
        private IList<Object> DataRow;
        private readonly CSVDataReaderFlags Flags;
        private readonly Int32 Offset;
        private Int32 CurrentIndex = -1;
        private Int32 State = 0;
        private readonly Int32 DelimiterLength;
        private String NewLine;
        private const Int32 FieldNamesFetched = 1;

        public CSVDataReader(TextReader reader,String delimiter, String qualifier, CultureInfo culture, Encoding encoding,CSVDataReaderFlags flags, Int32 offset, String newline,params DataColumn[] columns)
            {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (offset < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }
            Delimiter = delimiter ?? ";";
            DelimiterLength = Delimiter.Length;
            Reader = reader;
            Culture = culture ?? CultureInfo.CurrentCulture;
            #if NET35
            Columns = new List<DataColumn>(
                ((flags & CSVDataReaderFlags.FirstRowContainsFieldNames) != CSVDataReaderFlags.FirstRowContainsFieldNames)
                    ? columns
                    : new DataColumn[0]
                );
            #else
            Columns = new List<DataColumn>(
                (!flags.HasFlag(CSVDataReaderFlags.FirstRowContainsFieldNames)
                    ? columns
                    : new DataColumn[0]
                ));
            #endif
            Flags  = flags;
            Offset = offset;
            FieldCount = -1;
            NewLine = newline ?? "\n";
            }

        #region M:GetName(Int32):String
        /// <summary>Gets the name for the field to find.</summary>
        /// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public String GetName(Int32 i)
            {
            EnsureColumns();
            return Columns[i].ColumnName;
            }
        #endregion
        #region M:GetDataTypeName(Int32):Int32
        /// <summary>Gets the data type information for the specified field.</summary>
        /// <returns>The data type information for the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public String GetDataTypeName(Int32 i)
            {
            return GetFieldType(i).Name;
            }
        #endregion
        #region M:GetFieldType(Int32):Type
        /// <summary>Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</summary>
        /// <returns>The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Type GetFieldType(Int32 i)
            {
            EnsureColumns();
            return Columns[i].DataType;
            }
        #endregion
        #region M:GetValue(Int32):Object
        /// <summary>Return the value of the specified field.</summary>
        /// <returns>The <see cref="T:System.Object" /> which will contain the field value upon return.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Object GetValue(Int32 i)
            {
            return DataRow[i];
            }
        #endregion
        #region M:GetValues(Object[]):Int32
        /// <summary>Populates an array of objects with the column values of the current record.</summary>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        /// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into. </param>
        public Int32 GetValues(Object[] values)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:GetOrdinal(String):Int32
        /// <summary>Return the index of the named field.</summary>
        /// <returns>The index of the named field.</returns>
        /// <param name="name">The name of the field to find. </param>
        public Int32 GetOrdinal(String name) {
            EnsureColumns();
            for (var i = 0; i < Columns.Count; i++) {
                if (String.Equals(Columns[i].ColumnName, name, StringComparison.OrdinalIgnoreCase)) {
                    return i;
                    }
                }
            return -1;
            }
        #endregion
        #region M:GetBoolean(Int32):Boolean
        /// <summary>Gets the value of the specified column as a Boolean.</summary>
        /// <returns>The value of the column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Boolean GetBoolean(Int32 i)
            {
            return (Boolean)ToObject(GetValue(i), typeof(Boolean));
            }
        #endregion
        #region M:GetByte(Int32):Byte
        /// <summary>Gets the 8-bit unsigned integer value of the specified column.</summary>
        /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Byte GetByte(Int32 i)
            {
            return (Byte)ToObject(GetValue(i), typeof(Byte));
            }
        #endregion
        #region M:GetBytes(Int32,Int64,Byte[],Int32,Int32):Int64
        /// <summary>Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.</summary>
        /// <returns>The actual number of bytes read.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation. </param>
        /// <param name="buffer">The buffer into which to read the stream of bytes. </param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation. </param>
        /// <param name="length">The number of bytes to read. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Int64 GetBytes(Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:GetChar(Int32):Char
        /// <summary>Gets the character value of the specified column.</summary>
        /// <returns>The character value of the specified column.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Char GetChar(Int32 i)
            {
            return (Char)ToObject(GetValue(i), typeof(Char));
            }
        #endregion
        #region M:GetChars(Int32,Int32,Char[],Int32,Int32):Int64
        /// <summary>Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.</summary>
        /// <returns>The actual number of characters read.</returns>
        /// <param name="i">The zero-based column ordinal. </param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation. </param>
        /// <param name="buffer">The buffer into which to read the stream of bytes. </param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation. </param>
        /// <param name="length">The number of bytes to read. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Int64 GetChars(Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:GetGuid(Int32):Guid
        /// <summary>Returns the GUID value of the specified field.</summary>
        /// <returns>The GUID value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Guid GetGuid(Int32 i)
            {
            return (Guid)ToObject(GetValue(i), typeof(Guid));
            }
        #endregion
        #region M:GetInt16(Int32):Int16
        /// <summary>Gets the 16-bit signed integer value of the specified field.</summary>
        /// <returns>The 16-bit signed integer value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Int16 GetInt16(Int32 i)
            {
            return (Int16)ToObject(GetValue(i), typeof(Int16));
            }
        #endregion
        #region M:GetInt32(Int32):Int32
        /// <summary>Gets the 32-bit signed integer value of the specified field.</summary>
        /// <returns>The 32-bit signed integer value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Int32 GetInt32(Int32 i)
            {
            return (Int32)ToObject(GetValue(i), typeof(Int32));
            }
        #endregion
        #region M:GetInt64(Int32):Int64
        /// <summary>Gets the 64-bit signed integer value of the specified field.</summary>
        /// <returns>The 64-bit signed integer value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Int64 GetInt64(Int32 i)
            {
            return (Int64)ToObject(GetValue(i), typeof(Int64));
            }
        #endregion
        #region M:GetFloat(Int32):Single
        /// <summary>Gets the single-precision floating point number of the specified field.</summary>
        /// <returns>The single-precision floating point number of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Single GetFloat(Int32 i)
            {
            return (Single)ToObject(GetValue(i), typeof(Single));
            }
        #endregion
        #region M:GetDouble(Int32):Double
        /// <summary>Gets the double-precision floating point number of the specified field.</summary>
        /// <returns>The double-precision floating point number of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Double GetDouble(Int32 i)
            {
            return (Double)ToObject(GetValue(i), typeof(Double));
            }
        #endregion
        #region M:GetString(Int32):String
        /// <summary>Gets the string value of the specified field.</summary>
        /// <returns>The string value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public String GetString(Int32 i)
            {
            return (String)ToObject(GetValue(i), typeof(String));
            }
        #endregion
        #region M:GetDecimal(Int32):Decimal
        /// <summary>Gets the fixed-position numeric value of the specified field.</summary>
        /// <returns>The fixed-position numeric value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Decimal GetDecimal(Int32 i)
            {
            return (Decimal)ToObject(GetValue(i), typeof(Decimal));
            }
        #endregion
        #region M:GetDateTime(Int32):DateTime
        /// <summary>Gets the date and time data value of the specified field.</summary>
        /// <returns>The date and time data value of the specified field.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public DateTime GetDateTime(Int32 i)
            {
            return (DateTime)ToObject(GetValue(i), typeof(DateTime));
            }
        #endregion
        #region M:GetData(Int32):IDataReader
        /// <summary>Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.</summary>
        /// <returns>The <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public IDataReader GetData(Int32 i)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IsDBNull(Int32):Boolean
        /// <summary>Return whether the specified field is set to null.</summary>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        /// <param name="i">The index of the field to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Boolean IsDBNull(Int32 i)
            {
            throw new NotImplementedException();
            }
        #endregion

        /// <summary>Gets the number of columns in the current row.</summary>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public Int32 FieldCount { get;private set; }

        #region M:this[Int32]:Object
        /// <summary>Gets the column located at the specified index.</summary>
        /// <returns>The column located at the specified index as an <see cref="T:System.Object" />.</returns>
        /// <param name="i">The zero-based index of the column to get. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. </exception>
        public Object this[Int32 i]
            {
            get { return GetValue(i); }
            }
        #endregion
        #region M:this[String]:Object
        /// <summary>Gets the column with the specified name.</summary>
        /// <returns>The column with the specified name as an <see cref="T:System.Object" />.</returns>
        /// <param name="name">The name of the column to find. </param>
        /// <exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found. </exception>
        public Object this[String name]
            {
            get { return this[GetOrdinal(name)]; }
            }
        #endregion
        #region M:Close
        /// <summary>Closes the <see cref="T:System.Data.IDataReader" /> Object.</summary>
        public void Close() {
            if (!IsClosed) {
                IsClosed = true;
                if (Reader != null) {
                    Reader.Close();
                    }
                }
            }
        #endregion
        #region M:GetSchemaTable:DataTable
        /// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.</summary>
        /// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.IDataReader" /> is closed. </exception>
        public DataTable GetSchemaTable()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:NextResult:Boolean
        /// <summary>Advances the data reader to the next result, when reading the results of batch SQL statements.</summary>
        /// <returns>true if there are more rows; otherwise, false.</returns>
        public Boolean NextResult()
            {
            return false;
            }
        #endregion
        #region M:Read:Boolean
        /// <summary>Advances the <see cref="T:System.Data.IDataReader" /> to the next record.</summary>
        /// <returns>true if there are more rows; otherwise, false.</returns>
        public Boolean Read() {
            if (Reader != null) {
                if (Reader.Peek() == -1) { return false; }
                #if NET35
                if ((Flags  & CSVDataReaderFlags.HasMultiLineValue) != CSVDataReaderFlags.HasMultiLineValue)
                #else
                if (!Flags.HasFlag(CSVDataReaderFlags.HasMultiLineValue))
                #endif
                    {
                    String line;
                    while ((line = ReadLine(Reader,NewLine)) != null) {
                        line = line.Trim();
                        if (line.StartsWith("#", false, Culture)) { continue; }
                        CurrentIndex++;
                        if (CurrentIndex < Offset) { continue; }
                        var values = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                        if (Columns.Count == 0) {
                            #if NET35
                            if ((Flags & CSVDataReaderFlags.FirstRowContainsFieldNames) == CSVDataReaderFlags.FirstRowContainsFieldNames) {
                            #else
                            if (Flags.HasFlag(CSVDataReaderFlags.FirstRowContainsFieldNames)) {
                            #endif
                                Columns.AddRange(values.Select(i => new DataColumn(i, typeof(String))).ToArray());
                                FieldCount = Columns.Count;
                                continue;
                                }
                            DataRow = new Object[values.Length];
                            for (var i = 0; i < values.Length; i++) {
                                Columns.Add(new DataColumn(String.Format(Culture, "Field{0}", i), typeof(String)));
                                DataRow[i] = ToObject(values[i], typeof(String));
                                }
                            RecordsAffected += 1;
                            return true;
                            }
                        if (DataRow == null) {
                            DataRow = new Object[values.Length];
                            }
                        for (var i = 0; i < values.Length; i++) {
                            if (i < Columns.Count) {
                                DataRow[i] = ToObject(values[i], Columns[i].DataType);
                                }
                            else
                                {
                                DataRow[i] = values[i];
                                }
                            }
                        if (values.Length != FieldCount) { return false; }
                        RecordsAffected += 1;
                        return true;
                        }
                    }
                else
                    {
                    var builder = new StringBuilder();
                    #if NET35
                    if ((Flags & CSVDataReaderFlags.FirstRowContainsFieldNames)==CSVDataReaderFlags.FirstRowContainsFieldNames) { throw new InvalidOperationException(); }
                    #else
                    if (!Flags.HasFlag(CSVDataReaderFlags.FirstRowContainsFieldNames)) { throw new InvalidOperationException(); }
                    #endif
                    if (DelimiterLength == 0) { throw new InvalidOperationException(); }
                    #if NET35
                    if (((Flags & CSVDataReaderFlags.FirstRowContainsFieldNames)==CSVDataReaderFlags.FirstRowContainsFieldNames) &&
                        ((State & FieldNamesFetched) != FieldNamesFetched))
                    #else
                    if (Flags.HasFlag(CSVDataReaderFlags.FirstRowContainsFieldNames) &&
                        ((State & FieldNamesFetched) != FieldNamesFetched))
                    #endif
                        {
                        String line;
                        while ((line = ReadLine(Reader,NewLine)) != null) {
                            line = line.Trim();
                            if (line.StartsWith("#", false, Culture)) { continue; }
                            CurrentIndex++;
                            if (CurrentIndex < Offset) { continue; }
                            State |= FieldNamesFetched;
                            var values = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                            Columns.AddRange(values.Select(i => new DataColumn(i, typeof(String))).ToArray());
                            FieldCount = Columns.Count;
                            break;
                            }
                        }
                    if (FieldCount <= 0) { throw new InvalidOperationException(); }
                    var fieldindex = 0;
                    DataRow = new Object[FieldCount];
                    while (fieldindex < FieldCount) {
                        var c = 0;
                        while ((c = Reader.Read()) != -1) {
                            if (c == Delimiter[0]) {
                                var fragment = new StringBuilder();
                                fragment.Append((char)c);
                                for (var i = 1; i < DelimiterLength; i++) {
                                    c = Reader.Read();
                                    if (c == -1) { break; }
                                    fragment.Append((char)c);
                                    }
                                if (Delimiter != fragment.ToString()) {
                                    builder.Append(fragment);
                                    }
                                else
                                    {
                                    DataRow[fieldindex] = ToObject(builder.ToString(), typeof(String));
                                    #if NET35
                                    builder = new StringBuilder();
                                    #else
                                    builder.Clear();
                                    #endif
                                    fieldindex++;
                                    continue;
                                    }
                                }
                            else
                                {
                                if (fieldindex == FieldCount - 1) {
                                    if (c == '\r') {
                                        if (Reader.Peek() == '\n') {
                                            Reader.Read();
                                            }
                                        DataRow[fieldindex] = ToObject(builder.ToString(), typeof(String));
                                        break;
                                        }
                                    }
                                builder.Append((char)c);
                                }
                            }
                        if (c == -1) {
                            DataRow[fieldindex] = builder.ToString();
                            return (fieldindex == FieldCount - 1);
                            }
                        fieldindex++;
                        }
                    return true;
                    }
                }
            return false;
            }
        #endregion

        /// <summary>Gets a value indicating the depth of nesting for the current row.</summary>
        /// <returns>The level of nesting.</returns>
        public Int32 Depth { get; }

        #region P:IsClosed:Boolean
        /// <summary>Gets a value indicating whether the data reader is closed.</summary>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public Boolean IsClosed { get;private set; }
        #endregion
        #region P:RecordsAffected:Int32
        /// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
        /// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
        public Int32 RecordsAffected { get;private set; }
        #endregion

        private Boolean IsDisposed { get; set; }

        private void EnsureColumns()
            {

            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (!IsDisposed) {
                Close();
                IsDisposed = true;
                if (Reader != null) {
                    Reader.Dispose();
                    Reader = null;
                    }
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
        ~CSVDataReader() {
            Dispose(false);
            }
        #endregion
        #region M:ToObject(Object,Type):Object
        private static Object ToObject(Object source, Type type) {
            if (source == null) { return DBNull.Value; }
            if (source.GetType() == type) { return source; }
            if (source is IConvertible) { return ((IConvertible)source).ToType(type, null); }
            return source;
            }
        #endregion

        private static String ReadLine(TextReader reader, String newline)
            {
            if (newline == null) { throw new ArgumentNullException(nameof(newline)); }
            if (newline.Length == 0) { throw new ArgumentException(nameof(newline)); }
            var r = new StringBuilder();
            for (;;)
                {
                var c = reader.Read();
                if (c == -1) { break; }
                if (c != newline[0])
                    {
                    r.Append((char)c);
                    }
                else
                    {
                    var builder = new StringBuilder();
                    builder.Append((char)c);
                    for (var i = 1; i < newline.Length; i++) {
                        c = reader.Read();
                        if (c == -1) { break; }
                        builder.Append((char)c);
                        }
                    if (newline == builder.ToString()) {
                        break;
                        }
                    r.Append(builder);
                    }
                }
            return r.ToString();
            }
        }
    }

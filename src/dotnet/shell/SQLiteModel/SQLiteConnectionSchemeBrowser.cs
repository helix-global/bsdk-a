using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using BinaryStudio.DataProcessing;
using BinaryStudio.PlatformUI;

namespace shell
    {
    internal class SQLiteConnectionSchemeBrowser : NotifyPropertyChangedDispatcherObject<SQLiteConnection>, IEnumerable<Object>
        {
        public IList<SQLiteTableDescriptor>  TableDescriptors { get; }
        private readonly IList<SQLiteColumnDescriptor> ColumnDescriptors;

        private class SchemaItem
            {
            public String CollectionName;
            public Int32? NumberOfRestrictions;
            public Int32? NumberOfIdentifierParts;
            public override String ToString()
                {
                return CollectionName;
                }
            }

        private Boolean FocusedProperty;
        private SQLiteTableDescriptor SelectedTableDescriptorProperty;

        #region P:Focused:Boolean
        public Boolean Focused
            {
            get { return FocusedProperty; }
            set { SetValue(ref FocusedProperty, value, nameof(Focused)); }
            }
        #endregion
        #region P:SelectedTableDescriptor:SQLiteTableDescriptor
        public SQLiteTableDescriptor SelectedTableDescriptor
            {
            get { return SelectedTableDescriptorProperty; }
            set
                {
                if (SetValue(ref SelectedTableDescriptorProperty, value, nameof(SelectedTableDescriptorProperty))) {
                    OnPropertyChanged(nameof(SelectedTable));
                    }
                }
            }
        #endregion

        #region M:ToInt32(Object):Int32?
        private static Int32? ToInt32(Object value)
            {
            if ((value == null) || (value is DBNull)) { return null; }
            if (value is Int32 r) { return r; }
            return Int32.Parse(value.ToString());
            }
        #endregion
        #region M:ToInt64(Object):Int64?
        private static Int64? ToInt64(Object value)
            {
            if ((value == null) || (value is DBNull)) { return null; }
            if (value is Int64 r) { return r; }
            return Int64.Parse(value.ToString());
            }
        #endregion
        #region M:ToString(Object):String
        private static String ToString(Object value)
            {
            if ((value == null) || (value is DBNull)) { return null; }
            return value.ToString();
            }
        #endregion
        #region M:ToGuid(Object):Guid?
        private static Guid? ToGuid(Object value)
            {
            if ((value == null) || (value is DBNull)) { return null; }
            if (value is Guid r) { return r; }
            return Guid.Parse(value.ToString());
            }
        #endregion

        public SQLiteConnectionSchemeBrowser(SQLiteConnection content)
            : base(content)
            {
            TableDescriptors = new List<SQLiteTableDescriptor>();
            ColumnDescriptors = new List<SQLiteColumnDescriptor>();
            var r = new Dictionary<String,DataTable>();
            foreach (DataRow row in content.GetSchema().Rows) {
                var schema = (String)row[0];
                var table  = content.GetSchema(schema);
                r.Add(schema,table);
                switch (schema) {
                    case "Tables":
                        {
                        foreach (DataRow i in table.Rows) {
                            TableDescriptors.Add(new SQLiteTableDescriptor{
                                TableCatalog = ToString(i["TABLE_CATALOG"]),
                                TableSchema = ToString(i["TABLE_SCHEMA"]),
                                TableName = ToString(i["TABLE_NAME"]),
                                TableType = ToString(i["TABLE_TYPE"]),
                                TableDefinition = ToString(i["TABLE_DEFINITION"]),
                                TableId = (Int64)i["TABLE_ID"],
                                TableRootPage = (Int32)i["TABLE_ROOTPAGE"]
                                });
                            }
                        }
                        break;
                    case "Columns":
                        {
                        foreach (DataRow i in table.Rows) {
                            ColumnDescriptors.Add(new SQLiteColumnDescriptor{
                                TableCatalog = ToString(i["TABLE_CATALOG"]),
                                TableSchema = ToString(i["TABLE_SCHEMA"]),
                                TableName = ToString(i["TABLE_NAME"]),
                                ColumnName = ToString(i["COLUMN_NAME"]),
                                ColumnGuid = ToGuid(i["COLUMN_GUID"]),
                                ColumnPropId = ToInt64(i["COLUMN_PROPID"]),
                                OrdinalPosition = (Int32)i["ORDINAL_POSITION"],
                                ColumnHasDefault = (Boolean)i["COLUMN_HASDEFAULT"],
                                ColumnDefault = ToString(i["COLUMN_DEFAULT"]),
                                ColumnFlags = ToInt64(i["COLUMN_FLAGS"]),
                                Nullable = (Boolean)i["IS_NULLABLE"],
                                DataType = ToString(i["DATA_TYPE"]),
                                TypeGuid = ToGuid(i["TYPE_GUID"]),
                                CharacterMaximumLength = (Int32)i["CHARACTER_MAXIMUM_LENGTH"],
                                CharacterOctetLength = ToInt32(i["CHARACTER_OCTET_LENGTH"]),
                                NumericPrecision = ToInt32(i["NUMERIC_PRECISION"]),
                                NumericScale = ToInt32(i["NUMERIC_SCALE"]),
                                DateTimePrecision = ToInt64(i["DATETIME_PRECISION"]),
                                CharacterSetCatalog = ToString(i["CHARACTER_SET_CATALOG"]),
                                CharacterSetSchema = ToString(i["CHARACTER_SET_SCHEMA"]),
                                CharcterSetName = ToString(i["CHARACTER_SET_NAME"]),
                                CollationCatalog = ToString(i["COLLATION_CATALOG"]),
                                CollationSchema = ToString(i["COLLATION_SCHEMA"]),
                                CollationName = ToString(i["COLLATION_NAME"]),
                                DomainCatalog = ToString(i["DOMAIN_CATALOG"]),
                                DimainName = ToString(i["DOMAIN_NAME"]),
                                Description = ToString(i["DESCRIPTION"]),
                                PrimaryKey = (Boolean)i["PRIMARY_KEY"],
                                EDMType = ToString(i["EDM_TYPE"]),
                                AutoIncrement = (Boolean)i["AUTOINCREMENT"],
                                Unique = (Boolean)i["UNIQUE"]
                                });
                            }
                        }
                        break;
                    }
                }
            foreach (var table in TableDescriptors) {
                table.Columns.AddRange(
                    ColumnDescriptors.Where(i =>
                        String.Equals(i.TableCatalog, table.TableCatalog) &&
                        String.Equals(i.TableName, table.TableName)));
                }
            var tableS = TableDescriptors.FirstOrDefault();
            if (tableS != null) {
                tableS.IsSelected = true;
                }
            }

        public IEnumerator<Object> GetEnumerator()
            {
            yield return new SQLiteTableCollection(this);
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }

        public IQueryable SelectedTable { get {
            return (SelectedTableDescriptor != null)
                ? new SQLiteCommand($"SELECT * FROM {SelectedTableDescriptor.TableName}",Source).AsQueryable()
                : null;
                //return new SQLiteCommand(@"");
                //var connection = new SqlConnection("data source=localhost;initial catalog=icao;integrated security=True;MultipleActiveResultSets=True");
                ////var connection = new SqlConnection(@"Data Source=DEV-SR-SQL02\SQL2008SRV;Initial Catalog=CensorDb_ulyanov;Persist Security Info=True;User ID=sa;Password=escort");
                //connection.Open();
                //return
                //    new SqlCommand(@"SELECT * FROM [dbo].[T_EthalonStorageTable]", connection){
                //        CommandTimeout = 600,
                //        CommandType = CommandType.Text
                //        }.AsQueryable();
                //return (new DBQueryProvider(new SqlQueryFactory())).CreateQuery(
                //    Expression.Constant(new SqlCommand(@"SELECT * FROM [dbo].[T1]", connection){
                //     CommandType = CommandType.Text
                //     }));
            }
            }
        }
    }
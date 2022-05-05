using System;

namespace shell
    {
    internal class SQLiteColumnDescriptor
        {
        public String TableCatalog { get;set; }
        public String TableSchema { get;set; }
        public String TableName { get;set; }
        public String ColumnName { get;set; }
        public Guid? ColumnGuid { get;set; }
        public Int64? ColumnPropId { get;set; }
        public Int32 OrdinalPosition { get;set; }
        public Boolean ColumnHasDefault { get;set; }
        public String ColumnDefault { get;set; }
        public Int64? ColumnFlags { get;set; }
        public Boolean Nullable { get;set; }
        public String DataType { get;set; }
        public Guid? TypeGuid { get;set; }
        public Int32 CharacterMaximumLength { get;set; }
        public Int32? CharacterOctetLength { get;set; }
        public Int32? NumericPrecision { get;set; }
        public Int32? NumericScale { get;set; }
        public Int64? DateTimePrecision { get;set; }
        public String CharacterSetCatalog { get;set; }
        public String CharacterSetSchema { get;set; }
        public String CharcterSetName { get;set; }
        public String CollationCatalog { get;set; }
        public String CollationSchema { get;set; }
        public String CollationName { get;set; }
        public String DomainCatalog { get;set; }
        public String DimainName { get;set; }
        public String Description { get;set; }
        public Boolean PrimaryKey { get;set; }
        public String EDMType { get;set; }
        public Boolean AutoIncrement { get;set; }
        public Boolean Unique { get;set; }

        public override String ToString()
            {
            return ColumnName;
            }
        }
    }
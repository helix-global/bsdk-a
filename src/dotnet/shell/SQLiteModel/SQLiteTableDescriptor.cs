using System;
using System.Collections.Generic;

namespace shell
    {
    internal class SQLiteTableDescriptor
        {
        public String TableCatalog { get;set; }
        public String TableSchema { get;set; }
        public String TableName { get;set; }
        public String TableType { get;set; }
        public String TableDefinition { get;set; }
        public Int64 TableId { get;set; }
        public Int64 TableRootPage { get;set; }

        public List<SQLiteColumnDescriptor> Columns { get; }

        public SQLiteTableDescriptor()
            {
            Columns = new List<SQLiteColumnDescriptor>();
            }

        public override String ToString()
            {
            return TableName;
            }
        }
    }
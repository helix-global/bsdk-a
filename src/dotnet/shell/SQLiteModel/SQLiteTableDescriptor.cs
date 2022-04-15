using System;
using System.Collections.Generic;
using BinaryStudio.PlatformUI;

namespace shell
    {
    internal class SQLiteTableDescriptor : NotifyPropertyChangedDispatcherObject
        {
        private Boolean IsSelectedProperty;
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override String ToString()
            {
            return TableName;
            }

        public Boolean IsSelected
            {
            get { return IsSelectedProperty; }
            set { SetValue(ref IsSelectedProperty,value,nameof(IsSelected)); }
            }
        }
    }
using System;
using System.Collections;
using System.Collections.Generic;
using BinaryStudio.PlatformUI;

namespace shell
    {
    internal class SQLiteTableCollection : NotifyPropertyChangedDispatcherObject<SQLiteConnectionSchemeBrowser>, IEnumerable<Object>
        {
        public SQLiteTableCollection(SQLiteConnectionSchemeBrowser source)
            : base(source)
            {
            IsExpanded = true;
            }

        public Boolean IsExpanded { get;set; }

        public IEnumerator<Object> GetEnumerator()
            {
            return Source.TableDescriptors.GetEnumerator();
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return GetEnumerator();
            }
        }
    }
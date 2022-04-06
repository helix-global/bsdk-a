using System.Data.SQLite;
using BinaryStudio.PlatformUI;
using BinaryStudio.PlatformUI.Shell;

namespace shell
    {
    internal class ESQLiteConnection : NotifyPropertyChangedDispatcherObject<SQLiteConnection>
        {
        public ESQLiteConnection(SQLiteConnection source)
            : base(source)
            {
            SchemeBrowser = new SQLiteConnectionSchemeBrowser(source);
            }

        public SQLiteConnectionSchemeBrowser SchemeBrowser { get; }
        }
    }
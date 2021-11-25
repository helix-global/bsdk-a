//#define SQLSERVER
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace BinaryStudio.Diagnostics
    {
    public class TraceSession
        {
        #if SQLSERVER
        internal SqlConnection Connection { get; }
        #elif SQLITE
        internal SQLiteConnection Connection { get; }
        #endif
        public Int32 Identifier { get; }
        public TraceSession()
            {
            #if SQLSERVER
            Connection = new SqlConnection($"Data Source=zsql;Initial Catalog=SQL;Persist Security Info=True;User ID=sa;Password=Qwerty11;MULTIPLEACTIVERESULTSETS=true;");
            Connection.Open();
            using (var c = Connection.CreateCommand()) {
                c.CommandText = @"INSERT INTO TraceSession(ProcessName) OUTPUT Inserted.Id VALUES (@ProcessName)";
                c.Parameters.AddWithValue("@ProcessName", Path.GetFileName(Assembly.GetEntryAssembly().FullName));
                Identifier = (Int32)c.ExecuteScalar();
                }
            #elif SQLITE
            var target = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ".sqlite3");
            if (!Directory.Exists(target)) {
                var folder = Directory.CreateDirectory(target);
                folder.Attributes |= FileAttributes.Hidden;
                }
            var filename = Path.Combine(target, $"trace-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.db");
            Connection = new SQLiteConnection($"DataSource={filename}");
            Connection.Open();
            using (var c = Connection.CreateCommand()) {
                c.CommandText = @"
                CREATE TABLE [TraceInfo]
                    (
                    [Id]         [INTEGER] PRIMARY KEY AUTOINCREMENT,
                    [ThreadId]   [INTEGER] NULL,
                    [LongName]   [text] NULL,
                    [ShortName]  [text] NULL,
                    [EntryTime]  [bigint] NOT NULL,
                    [Size]       [bigint] NULL,
                    [LeavePoint] [INTEGER] NULL,
                    [Level]      [INTEGER] NULL,
                    [ParentId]   [INTEGER] NULL,
                    [Session]    [INTEGER] NULL
                    )";
                c.ExecuteNonQuery();
                }
            Identifier = 0;
            #endif
            }
        }
    }
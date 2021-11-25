//#define SQLSERVER
#define FEATURE_CLR
#define NO_TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BinaryStudio.Diagnostics
    {
    public class TraceScope : IDisposable
        {
        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();
        private static TraceSession Session;
        #if FEATURE_CLR
        private IntPtr Scope;
        #else
        private Int32 Identity { get; }
        #endif
        private static CultureInfo culture = CultureInfo.InvariantCulture;
        [ThreadStatic] private static Int32 Level;
        [ThreadStatic] private static Stack<Int32> Stack;
        private static Object i = new Object();
        private static Int64 I = 1;

        private class TraceSource : ITraceSource
            {
            ~TraceSource()
                {
                Console.WriteLine("TraceSource.Finalize");
                }

            public TraceSource()
                {
                GC.SuppressFinalize(this);
                }

            public unsafe void Trace(TraceSourceType type, UInt32 process, UInt32 thread, [MarshalAs(UnmanagedType.BStr)] String source, Int64 scope, Int64* r, ITraceParameterCollection args) {
                Console.WriteLine($">{type}:{process:X8}:{thread:X8}:{source}:{scope:X16}");
                if (type == TraceSourceType.Enter) {
                    if (r != null) {
                        lock (i) {
                            *r = I++;
                            Console.WriteLine($"<{type}:{process:X8}:{thread:X8}:{source}:{scope:X16}:{*r:X16}");
                            }
                        }
                    }
                if (args != null) {
                    Console.WriteLine("  ARGS:");
                    var c = args.Count;
                    for (var i = 0; i < c; i++) {
                        Console.WriteLine("    [{0:D2}]:Size={1},Value={2}", i,
                            args[i].Size,
                            String.Join(String.Empty,args[i].Value.Select(j => j.ToString("X2"))));
                        }
                    }
                }
            }

        static TraceScope()
            {
            SetDllDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), String.Format("{0}", (IntPtr.Size == 4) ? "x86" : "x64")));
            //SetupTraceSource(new TraceSource(), IntPtr.Zero);
            //Session = new TraceSession();
            }

        private static void EnsureStack() {
            if (Stack == null) {
                Stack = new Stack<Int32>();
                }
            }
        private static void EnsureConnection() {
            //lock (o) {
            //    if (db == null) {
            //        db = new SqlConnection($"Data Source=zsql;Initial Catalog=SQL;Persist Security Info=True;User ID=sa;Password=Qwerty11;MULTIPLEACTIVERESULTSETS=true;");
            //        db.Open();
            //        //using (var c = new SQLiteCommand(db)) {
            //        //    c.CommandText = @"
            //        //        CREATE TABLE TraceInfo(
            //        //            ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
            //        //            THREAD INTEGER NOT NULL,
            //        //            KEY TEXT NULL,
            //        //            REF INTEGER NULL
            //        //            );";
            //        //    c.ExecuteNonQuery();
            //        //    }
            //        }
            //    }
            }

        private static Object GetParentId() {
            if (Stack.Count == 0) { return 0; }
            return Stack.Peek();
            }

        #region TraceScope(String)
        public TraceScope(String identity) {
            #if FEATURE_CLR
            #if !NO_TRACE
            Scope = TraceScope_Create(identity, null);
            #endif
            #else
            EnsureConnection();
            EnsureStack();
            using (new WriteLockScope(o)) {
                using (var c = Session.Connection.CreateCommand()) {
                    Level++;
                    #if SQLSERVER
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[EntryTime],[Level],[Session],[ParentId]) OUTPUT Inserted.Id VALUES (@ThreadId,@LongName,@EntryTime,@Level,@Session,@ParentId)";
                    #else
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[EntryTime],[Level],[Session],[ParentId]) VALUES (@ThreadId,@LongName,@EntryTime,@Level,@Session,@ParentId)";
                    #endif
                    c.Parameters.AddWithValue("@ThreadId", Thread.CurrentThread.ManagedThreadId);
                    c.Parameters.AddWithValue("@LongName", identity);
                    c.Parameters.AddWithValue("@EntryTime", DateTime.Now.Ticks);
                    c.Parameters.AddWithValue("@Level", Level);
                    c.Parameters.AddWithValue("@Session", Session.Identifier);
                    c.Parameters.AddWithValue("@ParentId", GetParentId());
                    #if SQLSERVER
                    Identity = (Int32)c.ExecuteScalar();
                    #else
                    c.ExecuteNonQuery();
                    Identity = (Int32)Session.Connection.LastInsertRowId;
                    #endif
                    Stack.Push(Identity);
                    }
                }
            #endif
            }
        #endregion
        #region TraceScope(String,Int64)
        public TraceScope(String identity, Int64 size) {
            #if FEATURE_CLR
            #if !NO_TRACE
            Scope = TraceScope_Create(identity, null, size);
            #endif
            #else
            EnsureConnection();
            EnsureStack();
            using (new WriteLockScope(o)) {
                using (var c = Session.Connection.CreateCommand()) {
                    Level++;
                    #if SQLSERVER
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[EntryTime],[Size],[Level],[Session],[ParentId]) OUTPUT Inserted.Id VALUES (@ThreadId,@LongName,@EntryTime,@Size,@Level,@Session,@ParentId)";
                    #else
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[EntryTime],[Size],[Level],[Session],[ParentId]) VALUES (@ThreadId,@LongName,@EntryTime,@Size,@Level,@Session,@ParentId)";
                    #endif
                    c.Parameters.AddWithValue("@ThreadId", Thread.CurrentThread.ManagedThreadId);
                    c.Parameters.AddWithValue("@LongName", identity);
                    c.Parameters.AddWithValue("@EntryTime", DateTime.Now.Ticks);
                    c.Parameters.AddWithValue("@Size", size);
                    c.Parameters.AddWithValue("@Level", Level);
                    c.Parameters.AddWithValue("@Session", Session.Identifier);
                    c.Parameters.AddWithValue("@ParentId", GetParentId());
                    #if SQLSERVER
                    Identity = (Int32)c.ExecuteScalar();
                    #else
                    c.ExecuteNonQuery();
                    Identity = (Int32)Session.Connection.LastInsertRowId;
                    #endif
                    Stack.Push(Identity);
                    }
                }
            #endif
            }
        #endregion
        #region TraceScope(StackFrame)
        public TraceScope(StackFrame identity) {
            #if FEATURE_CLR
            #if !NO_TRACE
            Scope = TraceScope_Create(ToString(identity.GetMethod()), identity.GetMethod().Name);
            #endif
            #else
            EnsureConnection();
            EnsureStack();
            using (new WriteLockScope(o)) {
                using (var c = Session.Connection.CreateCommand()) {
                    Level++;
                    #if SQLSERVER
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[ShortName],[EntryTime],[Level],[Session],[ParentId]) OUTPUT Inserted.Id VALUES (@ThreadId,@LongName,@ShortName,@EntryTime,@Level,@Session,@ParentId)";
                    #else
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[ShortName],[EntryTime],[Level],[Session],[ParentId]) VALUES (@ThreadId,@LongName,@ShortName,@EntryTime,@Level,@Session,@ParentId)";
                    #endif
                    c.Parameters.AddWithValue("@ThreadId", Thread.CurrentThread.ManagedThreadId);
                    c.Parameters.AddWithValue("@LongName", ToString(identity.GetMethod()));
                    c.Parameters.AddWithValue("@ShortName", identity.GetMethod().Name);
                    c.Parameters.AddWithValue("@EntryTime", DateTime.Now.Ticks);
                    c.Parameters.AddWithValue("@Level", Level);
                    c.Parameters.AddWithValue("@Session", Session.Identifier);
                    c.Parameters.AddWithValue("@ParentId", GetParentId());
                    #if SQLSERVER
                    Identity = (Int32)c.ExecuteScalar();
                    #else
                    c.ExecuteNonQuery();
                    Identity = (Int32)Session.Connection.LastInsertRowId;
                    #endif
                    Stack.Push(Identity);
                    }
                }
            #endif
            }
        #endregion
        #region TraceScope(StackFrame,Int64)
        public TraceScope(StackFrame identity, Int64 size) {
            #if FEATURE_CLR
            #if !NO_TRACE
            Scope = TraceScope_Create(ToString(identity.GetMethod()), identity.GetMethod().Name, size);
            #endif
            #else
            EnsureConnection();
            EnsureStack();
            using (new WriteLockScope(o)) {
                using (var c = Session.Connection.CreateCommand()) {
                    Level++;
                    #if SQLSERVER
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[ShortName],[EntryTime],[Size],[Level],[Session],[ParentId]) OUTPUT Inserted.Id VALUES (@ThreadId,@LongName,@ShortName,@EntryTime,@Size,@Level,@Session,@ParentId)";
                    #else
                    c.CommandText = @"INSERT INTO TraceInfo([ThreadId],[LongName],[ShortName],[EntryTime],[Size],[Level],[Session],[ParentId]) VALUES (@ThreadId,@LongName,@ShortName,@EntryTime,@Size,@Level,@Session,@ParentId)";
                    #endif
                    c.Parameters.AddWithValue("@ThreadId", Thread.CurrentThread.ManagedThreadId);
                    c.Parameters.AddWithValue("@LongName", ToString(identity.GetMethod()));
                    c.Parameters.AddWithValue("@ShortName", identity.GetMethod().Name);
                    c.Parameters.AddWithValue("@EntryTime", DateTime.Now.Ticks);
                    c.Parameters.AddWithValue("@Size", size);
                    c.Parameters.AddWithValue("@Level", Level);
                    c.Parameters.AddWithValue("@Session", Session.Identifier);
                    c.Parameters.AddWithValue("@ParentId", GetParentId());
                    #if SQLSERVER
                    Identity = (Int32)c.ExecuteScalar();
                    #else
                    c.ExecuteNonQuery();
                    Identity = (Int32)Session.Connection.LastInsertRowId;
                    #endif
                    Stack.Push(Identity);
                    }
                }
            #endif
            }
        #endregion

        public TraceScope()
            :this(new StackTrace(true).GetFrame(1))
            {
            }

        public TraceScope(Int64 size)
            :this(new StackTrace(true).GetFrame(1), size)
            {
            }

        #region M:ToString(MethodBase):String
        protected static String ToString(MethodBase mi)
            {
            var r = new StringBuilder();
            if (mi.DeclaringType != null) {
                r.Append(mi.DeclaringType.Name);
                r.Append(".");
                }
            r.Append(mi.Name);
            var args = mi.GetParameters();
            if (args.Length > 0) {
                r.Append("(");
                #if NET35
                r.Append(String.Join(",", args.Select(i => ToString(i.ParameterType)).ToArray()));
                #else
                r.Append(String.Join(",", args.Select(i => ToString(i.ParameterType))));
                #endif
                r.Append(")");
                }
            return r.ToString();
            }
        #endregion
        #region M:ToString(Type):String
        protected static String ToString(Type type) {
            var r = new StringBuilder();
            #if !NET40 && !NET35
            if (type.IsConstructedGenericType) {
                var j = type.Name.IndexOf("`");
                r.Append(type.Name.Substring(0, j));
                r.Append('<');
                r.Append(String.Join(",", type.GenericTypeArguments.Select(ToString)));
                r.Append('>');
                }
            #else
            if (!type.IsGenericTypeDefinition && type.IsGenericType) {
                var j = type.Name.IndexOf("`");
                r.Append(type.Name.Substring(0, j));
                r.Append('<');
                #if NET35
                r.Append(String.Join(",", type.GetGenericArguments().Select(ToString).ToArray()));
                #else
                r.Append(String.Join(",", type.GetGenericArguments().Select(ToString)));
                #endif
                r.Append('>');
                }
            #endif
            else
                {
                r.Append(type.Name);
                }
            return r.ToString();
            }
        #endregion

        private class ReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public ReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterReadLock();
                }

            public void Dispose()
                {
                o.ExitReadLock();
                o = null;
                }
            }

        private class WriteLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public WriteLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterWriteLock();
                }

            public void Dispose()
                {
                o.ExitWriteLock();
                o = null;
                }
            }

        #region M:ToDouble(Object):Double?
        private static Double? ToDouble(Object value) {
            if (value == null) { return null; }
            if (value is DBNull) { return null; }
            if (value is Double) { return (Double)value; }
            if (value is IConvertible c) { return c.ToDouble(null); }
            return Double.TryParse(value.ToString(), out var r)
                ? r
                : new Double?();
            }
        #endregion
        #region M:ToInt64(Object):Int64?
        private static Int64? ToInt64(Object value) {
            if (value == null) { return null; }
            if (value is DBNull) { return null; }
            if (value is Int64) { return (Int64)value; }
            if (value is IConvertible c) { return c.ToInt64(null); }
            return Int64.TryParse(value.ToString(), out var r)
                ? r
                : new Int64?();
            }
        #endregion
        #region M:ToInt32(Object):Int32?
        private static Int32? ToInt32(Object value) {
            if (value == null) { return null; }
            if (value is DBNull) { return null; }
            if (value is Int32) { return (Int32)value; }
            if (value is IConvertible c) { return c.ToInt32(null); }
            return Int32.TryParse(value.ToString(), out var r)
                ? r
                : new Int32?();
            }
        #endregion
        #region M:ToString(Object):String
        private static String ToString(Object value)
            {
            return ((value == null) || (value is DBNull))
                ? null
                : value.ToString();
            }
        #endregion

        private class TraceSummaryInfo
            {
            public String EntryPoint { get;set; }
            public TimeSpan Duration { get;set; }
            public Int32 Count { get;set; }
            public String Velocity { get;set; }
            public Int64? Size { get;set; }
            }

        private class TraceInfo
            {
            public Int32 Id { get;set; }
            public String LongName { get;set; }
            public String ShortName { get;set; }
            public TimeSpan Duration { get;set; }
            public Int32? ParentId { get;set; }
            }

        private static String GetShortName(String value, IDictionary<String,String> shortnames)
            {
            if (shortnames.TryGetValue(value, out var r)) {
                if (!String.IsNullOrWhiteSpace(r)) {
                    return r;
                    }
                }
            var values = value.Split(new Char[]{'.' }, StringSplitOptions.RemoveEmptyEntries);
            return (values.Length > 0)
                ? values[values.Length - 1]
                : value;
            }

        #region M:WriteTo(TextWriter,Dictionary<String,TraceSummaryInfo>,Dictionary<Int64,TraceInfo>)
        private static void WriteTo(TextWriter writer, Dictionary<String, TraceSummaryInfo> summary, Dictionary<Int64, TraceInfo> table, Int32 level, Double K, Int64 timespan, params Int32[] parents) {
            var values = table.Where(i => parents.Any(j => j == i.Value.ParentId)).Select(i => i.Value).ToArray();
            var shortnames = values.Select(i => Tuple.Create(i.LongName, i.ShortName)).Distinct().ToDictionary(i => i.Item1, i=> i.Item2);
            var groups = values.GroupBy(i=>i.LongName).Select(i=>
                Tuple.Create(i,
                    i.Sum(j => j.Duration.Ticks),
                    i.Count())).ToArray();
            if (timespan < 0) {
                timespan = values.Sum(i => i.Duration.Ticks);
                }
            foreach (var j in groups.OrderByDescending(i => i.Item2)) {
                var builder = new StringBuilder();
                builder.Append(new String(' ', level*2));
                var ticks = j.Item2;
                var count = j.Item3;
                //var P = (((Double)ticks)/duration)*K;
                var P = (((Double)ticks)/timespan)*K;
                builder.AppendFormat(culture, "[{0:00.00}%]:", P*100.0);
                builder.AppendFormat(culture, "[{0}]:", GetShortName(j.Item1.Key, shortnames));
                builder.AppendFormat(culture, "[{0:F2} ms]:", (new TimeSpan(ticks)).TotalMilliseconds);
                builder.AppendFormat(culture, "[{0} hit]:", count);
                builder.AppendFormat(culture, "[{0}]:", j.Item1.Key);
                writer.WriteLine(builder);
                WriteTo(writer, summary, table, level + 1, P, j.Item2, j.Item1.Select(i => i.Id).ToArray());
                //builder.AppendFormat("[{0} ms]:", i.Duration.TotalMilliseconds);
                }
            }
        #endregion
        #region M:WriteTo(TextWriter)
        public static void WriteTo(TextWriter writer)
            {
            var summary = new Dictionary<String, TraceSummaryInfo>();
            #if FEATURE_CLR
            #if !NO_TRACE
            using (var connection = new SQLiteConnection($"DataSource={TraceScope_GetDataSourceFileName()}")) {
                connection.Open();
                using (var c = connection.CreateCommand()) {
                    c.CommandText = @"
                        WITH T AS
                          (
                          SELECT
                            [a].[LongName] [LongName]
                            ,SUM(([b].[EntryTime]-[a].[EntryTime])) [Duration]
                            ,COUNT([a].[Id]) [Count]
                            ,SUM([a].[Size]) [Size]
                          FROM [TraceInfo] [a]
                            INNER JOIN [TraceInfo] [b] ON ([b].[LeavePoint]=[a].[Id])
                          GROUP BY [a].[LongName]
                          )
                        SELECT
                          ROW_NUMBER() OVER (ORDER BY [a].[LongName]) [OrderId],*
                        FROM T [a]
                        ";
                    writer.WriteLine("------------------------");
                    using (var reader = c.ExecuteReader()) {
                        while (reader.Read())
                            {
                            var span = ToInt64(reader[2]).GetValueOrDefault();
                            var i = new TraceSummaryInfo {
                                EntryPoint = reader[1].ToString(),
                                Duration = new TimeSpan(span),
                                Size = ToInt64(reader[4])
                                };
                            summary[i.EntryPoint] = i;
                            var r = new StringBuilder();
                            r.Append($"{reader[0]:D3}:[{i.EntryPoint}]:[{i.Duration}]:[{reader[3]}]");
                            if (i.Size != null) {
                                var velocity = i.Size.Value/(Double)i.Duration.TotalSeconds;
                                if (velocity > 1024) {
                                    velocity /= 1024;
                                    i.Velocity = $"{velocity:F2}KB/s";
                                    }
                                else
                                    {
                                    i.Velocity = $"{velocity:F2}B/s";
                                    }
                                r.Append($":[{i.Velocity}]");
                                }
                            writer.WriteLine(r.ToString());
                            }
                        }
                    }
                var table = new Dictionary<Int64, TraceInfo>();
                using (var c = connection.CreateCommand()) {
                    c.CommandText = @"
                          SELECT
                             [a].[Id]
                            ,[a].[LongName] [LongName]
                            ,([b].[EntryTime]-[a].[EntryTime]) [Duration]
                            ,[a].[ParentId]
                            ,[a].[ShortName]
                          FROM [TraceInfo] [a]
                            INNER JOIN [TraceInfo] [b] ON ([b].[LeavePoint]=[a].[Id])
                          ORDER BY [a].[Id]
                        ";
                    using (var reader = c.ExecuteReader()) {
                        while (reader.Read()) {
                            var i = new TraceInfo {
                                Id = ToInt32(reader[0]).GetValueOrDefault(),
                                LongName = ToString(reader[1]),
                                Duration = new TimeSpan((Int64)reader[2]),
                                ParentId = ToInt32(reader[3]),
                                ShortName = ToString(reader[4])
                                };
                            table[i.Id] = i;
                            }
                        }
                    }
                if (table.Count > 0) {
                    writer.WriteLine("------------------------");
                    WriteTo(writer, summary, table, 0, 1.0, -1, 0);
                    }
                }
            #endif
            #else
            EnsureConnection();
            using (var c = Session.Connection.CreateCommand()) {
                c.CommandText = @"
                    WITH T AS
                      (
                      SELECT
                        [a].[LongName] [LongName]
                        ,SUM(([b].[EntryTime]-[a].[EntryTime])) [Duration]
                        ,COUNT([a].[Id]) [Count]
                        ,SUM([a].[Size]) [Size]
                      FROM [TraceInfo] [a]
                        INNER JOIN [TraceInfo] [b] ON ([b].[LeavePoint]=[a].[Id])
                      WHERE [a].[Session] = @Session
                      GROUP BY [a].[Session],[a].[LongName]
                      )
                    SELECT
                      ROW_NUMBER() OVER (ORDER BY [a].[LongName]) [OrderId],*
                    FROM T [a]
                    ";
                c.Parameters.AddWithValue("@Session", Session.Identifier);
                writer.WriteLine("------------------------");
                using (var reader = c.ExecuteReader()) {
                    while (reader.Read())
                        {
                        var i = new TraceSummaryInfo {
                            EntryPoint = (String)reader[1],
                            Duration = new TimeSpan((Int64)reader[2]),
                            Size = ToInt64(reader[4])
                            };
                        summary[i.EntryPoint] = i;
                        var r = new StringBuilder();
                        r.Append($"{reader[0]:D3}:[{i.EntryPoint}]:[{i.Duration}]:[{reader[3]}]");
                        if (i.Size != null) {
                            var velocity = i.Size.Value/(Double)i.Duration.TotalSeconds;
                            if (velocity > 1024) {
                                velocity /= 1024;
                                i.Velocity = $"{velocity:F2}KB/s";
                                }
                            else
                                {
                                i.Velocity = $"{velocity:F2}B/s";
                                }
                            r.Append($":[{i.Velocity}]");
                            }
                        writer.WriteLine(r.ToString());
                        }
                    }
                }
            var table = new Dictionary<Int64, TraceInfo>();
            using (var c = Session.Connection.CreateCommand()) {
                c.CommandText = @"
                      SELECT
                         [a].[Id]
                        ,[a].[LongName] [LongName]
                        ,([b].[EntryTime]-[a].[EntryTime]) [Duration]
                        ,[a].[Level]
                        ,[a].[ParentId]
                        ,[a].[ShortName]
                      FROM [TraceInfo] [a]
                        INNER JOIN [TraceInfo] [b] ON ([b].[LeavePoint]=[a].[Id])
                      WHERE [a].[Session] = @Session
                      ORDER BY [a].[Session] DESC,[a].[Id]
                    ";
                c.Parameters.AddWithValue("@Session", Session.Identifier);
                using (var reader = c.ExecuteReader()) {
                    while (reader.Read()) {
                        var i = new TraceInfo {
                            Id = ToInt32(reader[0]).GetValueOrDefault(),
                            LongName = ToString(reader[1]),
                            Duration = new TimeSpan((Int64)reader[2]),
                            //Level = ToInt32(reader[3]).GetValueOrDefault(),
                            ParentId = ToInt32(reader[4]),
                            ShortName = ToString(reader[5])
                            };
                        table[i.Id] = i;
                        }
                    }
                }
            if (table.Count > 0) {
                writer.WriteLine("------------------------");
                WriteTo(writer, summary, table, 0, 1.0, -1, 0);
                }
            #endif
            }
        #endregion
        #region M:Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            #if FEATURE_CLR
            #if !NO_TRACE
            TraceScope_Close(Scope);
            #endif
            #else
            using (new WriteLockScope(o)) {
                using (var c = Session.Connection.CreateCommand()) {
                    c.CommandText = @"INSERT INTO TraceInfo([EntryTime],[LeavePoint]) VALUES (@EntryTime,@LeavePoint)";
                    c.Parameters.AddWithValue("@EntryTime", DateTime.Now.Ticks);
                    c.Parameters.AddWithValue("@LeavePoint", Identity);
                    c.ExecuteNonQuery();
                    Level--;
                    Stack.Pop();
                    }
                }
            #endif
            }
        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Boolean SetDllDirectory(String pathname);

        #if !NO_TRACE
        [DllImport("intrcpx.dll", EntryPoint = "SetupTraceSource")] private static extern Int32 SetupTraceSource(ITraceSource scope, IntPtr r);
        [DllImport("clrx.dll", EntryPoint = "TraceScope_Create")] private static extern Int32 TraceScope_Create([MarshalAs(UnmanagedType.LPStr)] String longname, [MarshalAs(UnmanagedType.LPStr)] String shortname, ref Int64 size, out IntPtr scope);
        [DllImport("clrx.dll", EntryPoint = "TraceScope_Create")] private static extern Int32 TraceScope_Create([MarshalAs(UnmanagedType.LPStr)] String longname, [MarshalAs(UnmanagedType.LPStr)] String shortname, IntPtr size, out IntPtr scope);
        [DllImport("clrx.dll", EntryPoint = "TraceScope_Close")]  private static extern Int32 TraceScope_Close(IntPtr scope);
        [DllImport("clrx.dll", EntryPoint = "TraceScope_GetDataSourceFileName")]  private static extern Int32 TraceScope_GetDataSourceFileName(out IntPtr filename);

        private static IntPtr TraceScope_Create(String longname, String shortname, Int64 size)
            {
            SetDllDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), String.Format("{0}", (IntPtr.Size == 4) ? "x86" : "x64")));
            TraceScope_Create(longname, shortname, ref size, out var scope);
            return scope;
            }

        private static IntPtr TraceScope_Create(String longname, String shortname)
            {
            SetDllDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), String.Format("{0}", (IntPtr.Size == 4) ? "x86" : "x64")));
            TraceScope_Create(longname, shortname, IntPtr.Zero, out var scope);
            return scope;
            }

        private static String TraceScope_GetDataSourceFileName()
            {
            TraceScope_GetDataSourceFileName(out var filename);
            try
                {
                return Marshal.PtrToStringAnsi(filename);
                }
            finally
                {
                Marshal.FreeHGlobal(filename);
                }
            }
        #endif
        }
    }

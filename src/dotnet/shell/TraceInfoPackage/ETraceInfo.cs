using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using BinaryStudio.IO;
using BinaryStudio.PlatformUI;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace shell
    {
    internal class ETraceInfo : NotifyPropertyChangedDispatcherObject<SQLiteConnection>
        {
        private Boolean LoadedProperty;
        private Boolean FocusedProperty;
        public IList<ETraceInfoEntry> Entries { get; }
        public ETraceInfo(SQLiteConnection connection)
            :base(connection)
            {
            Entries = new ObservableCollection<ETraceInfoEntry>();
            var entries = new Dictionary<Int64, ETraceInfoEntry>();
            using (var command = connection.CreateCommand()) {
                command.CommandText = @"
                    SELECT
                      [a].[Id]
                     ,[a].[LongName] [LongName]
                     ,([b].[EntryTime]-[a].[EntryTime]) [Duration]
                     ,[a].[ParentId]
                     ,[a].[ShortName]
                     ,[a].[Parameters] [InputParameters]
                     ,[b].[Parameters] [OutputParameters]
                    FROM [TraceInfo] [a]
                      LEFT JOIN [TraceInfo] [b] ON ([b].[LeavePoint]=[a].[Id])
                    WHERE ([a].[ThreadId] IS NOT NULL)
                    ORDER BY [a].[Id]
                    ";
                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        var Id = (Int64)reader["Id"];
                        var ParentId = (Int64)reader["ParentId"];
                        var ShortName = (String)reader["ShortName"];
                        var InputParameters  = (Byte[])reader["InputParameters" ];
                        var OutputParameters = (Byte[])reader["OutputParameters"];
                        var e = new ETraceInfoEntry {
                            Id = Id,
                            ParentId = ParentId,
                            InputParameters = InputParameters,
                            OutputParameters = OutputParameters,
                            ShortName = ShortName,
                            Duration = TimeSpan.FromTicks((Int64)reader["Duration"])
                            };
                        entries[Id] = e;
                        if (ParentId == 0)
                            {
                            Entries.Add(e);
                            }
                        }
                    }
                }

            foreach (var e in entries) {
                if (e.Value.ParentId != 0) {
                    entries[e.Value.ParentId].Entries.Add(e.Value);
                    }
                }

            ThreadPool.QueueUserWorkItem((state)=>{
                foreach (var e in Entries) {
                    e.DecodedInputParameters  = Asn1Object.Load(new ReadOnlyMemoryMappingStream(e.InputParameters )).FirstOrDefault();
                    e.DecodedOutputParameters = Asn1Object.Load(new ReadOnlyMemoryMappingStream(e.OutputParameters)).FirstOrDefault();
                    }
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(()=>{
                    Loaded = true;
                    }));
                });
            }

        #region P:Focused:Boolean
        public Boolean Focused
            {
            get { return FocusedProperty; }
            set { SetValue(ref FocusedProperty, value, nameof(Focused)); }
            }
        #endregion
        #region P:Loaded:Boolean
        public Boolean Loaded
            {
            get { return LoadedProperty; }
            set { SetValue(ref LoadedProperty, value, nameof(Loaded)); }
            }
        #endregion
        }
    }
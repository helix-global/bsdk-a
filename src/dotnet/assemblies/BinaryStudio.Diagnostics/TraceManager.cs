using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BinaryStudio.Diagnostics
    {
    public class TraceManager
        {
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        #region P:Instance:TraceManager
        private static TraceManager instance;
        public static TraceManager Instance { get {
            return instance ?? (instance = new TraceManager());
            }}
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

        private class UpgradeableReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public UpgradeableReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterUpgradeableReadLock();
                }

            public void Dispose()
                {
                o.ExitUpgradeableReadLock();
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

        //#region M:Trace(String):ITraceContext
        //public ITraceContext Trace(String key)
        //    {
        //    var r = key.Split('.');
        //    return (r.Length > 1)
        //        ? new TraceContext(this, r[r.Length-1], key)
        //        : new TraceContext(this, key, key);
        //    }
        //#endregion
        //#region M:Trace(String,String):ITraceContext
        //public ITraceContext Trace(String shortname, String key)
        //    {
        //    return new TraceContext(this, shortname, key);
        //    }
        //#endregion
        //#region M:Trace(String,Int64):ITraceContext
        //public ITraceContext Trace(String shortname, String key, Int64 datasize)
        //    {
        //    return new TraceContext(this, shortname, key, datasize);
        //    }
        //#endregion
        //#region M:Trace(TraceContextIdentity):ITraceContext
        //public ITraceContext Trace(TraceContextIdentity key)
        //    {
        //    return new TraceContext(this, key);
        //    }
        //#endregion
        //#region M:Trace(TraceContextIdentity):ITraceContext
        //public ITraceContext Trace(TraceContextIdentity key,Int64 datasize)
        //    {
        //    return new TraceContext(this, key, datasize);
        //    }
        //#endregion

        protected static IDisposable ReadLock(ReaderWriterLockSlim o)            { return new ReadLockScope(o);            }
        protected static IDisposable WriteLock(ReaderWriterLockSlim o)           { return new WriteLockScope(o);           }
        protected static IDisposable UpgradeableReadLock(ReaderWriterLockSlim o) { return new UpgradeableReadLockScope(o); }

        //public ITraceContext Trace()
        //    {
        //    return new TraceContext(this, new TraceContextStackFrameIdentity(new StackTrace(true).GetFrame(1)));
        //    }

        //public ITraceContext Trace(Int64 datasize)
        //    {
        //    return new TraceContext(this, new TraceContextStackFrameIdentity(new StackTrace(true).GetFrame(1)), datasize);
        //    }

        private TraceManager()
            {
            }

        #region M:UpdateSummary(TraceContext)
        private void UpdateSummary(TraceContext scope) {
            using (UpgradeableReadLock(summarylock)) {
                TraceScopeSummaryInfo r;
                if (!summary.TryGetValue(scope.Identity, out r)) {
                    using (WriteLock(summarylock)) {
                        summary.Add(scope.Identity, new TraceScopeSummaryInfo(scope.Identity)
                            {
                            AverageDuration = scope.Duration,
                            MaxDuration = scope.Duration,
                            MinDuration = scope.Duration,
                            AverageVelocity = scope.Velocity,
                            Count = 1
                            });
                        return;
                        }
                    }
                using (WriteLock(r.@lock)) {
                    r.AverageDuration = Average(r.AverageDuration, scope.Duration);
                    r.MinDuration = Min(r.MinDuration, scope.Duration);
                    r.MaxDuration = Max(r.MaxDuration, scope.Duration);
                    r.AverageVelocity = Average(r.AverageVelocity, scope.Velocity);
                    r.Count++;
                    }
                }
            }
        #endregion
        #region M:PushContext(TraceContext)
        internal void PushContext(TraceContext context) {
            Tuple<Stack<TraceContext>,ReaderWriterLockSlim> r;
            using (UpgradeableReadLock(scopelock)) {
                var key = Thread.CurrentThread.ManagedThreadId;
                if (!contexts.TryGetValue(key, out r)) {
                    using (WriteLock(scopelock)) {
                        contexts.Add(key, r = Tuple.Create(new Stack<TraceContext>(), new ReaderWriterLockSlim()));
                        }
                    }
                }
            using (WriteLock(r.Item2)) {
                r.Item1.Push(context);
                }
            }
        #endregion
        #region M:PopContext
        internal void PopContext() {
            using (UpgradeableReadLock(scopelock)) {
                Tuple<Stack<TraceContext>,ReaderWriterLockSlim> r;
                var key = Thread.CurrentThread.ManagedThreadId;
                if (contexts.TryGetValue(key, out r)) {
                    using (WriteLock(r.Item2)) {
                        if (r.Item1.Count > 0) {
                            var o = r.Item1.Pop();
                            UpdateSummary(o);
                            if (r.Item1.Count == 0) {
                                using (WriteLock(scopelock)) {
                                    contexts.Remove(key);
                                    AddH(o);
                                    }
                                }
                            else
                                {
                                var p = r.Item1.Peek();
                                p.Add(o);
                                }
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:Average(Double,Double):Double
        private static Double Average(Double x, Double y)
            {
            return ((x + y)/2.0);
            }
        #endregion
        #region M:Average(TimeSpan,TimeSpan):TimeSpan
        private static TimeSpan Average(TimeSpan x, TimeSpan y)
            {
            return new TimeSpan((x.Ticks + y.Ticks)/2);
            }
        #endregion
        #region M:Min(TimeSpan,TimeSpan):TimeSpan
        private static TimeSpan Min(TimeSpan x, TimeSpan y)
            {
            return new TimeSpan(Math.Min(x.Ticks, y.Ticks));
            }
        #endregion
        #region M:Max(TimeSpan,TimeSpan):TimeSpan
        private static TimeSpan Max(TimeSpan x, TimeSpan y)
            {
            return new TimeSpan(Math.Max(x.Ticks, y.Ticks));
            }
        #endregion
        #region M:AddH(TraceContext)
        private void AddH(TraceContext context)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (!context.IsCompleted) { throw new ArgumentOutOfRangeException(nameof(context)); }
            if (!h.TryGetValue(context.Identity, out TraceContext r))
                {
                h.Add(context.Identity, context);
                }
            else
                {
                r.UnionWith(context);
                }
            }
        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly ReaderWriterLockSlim scopelock = new ReaderWriterLockSlim();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IDictionary<Int32, Tuple<Stack<TraceContext>,ReaderWriterLockSlim>> contexts = new Dictionary<Int32, Tuple<Stack<TraceContext>, ReaderWriterLockSlim>>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IDictionary<TraceContextIdentity,TraceContext> h = new Dictionary<TraceContextIdentity, TraceContext>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly ReaderWriterLockSlim summarylock = new ReaderWriterLockSlim();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IDictionary<TraceContextIdentity,TraceScopeSummaryInfo> summary = new Dictionary<TraceContextIdentity, TraceScopeSummaryInfo>();

        internal ICollection<TraceContext> H { get { return h.Values; }}
        internal ICollection<TraceScopeSummaryInfo> P { get { return summary.Values; }}

        private void Write(TextWriter writer, TraceContext context, Int32 indent, Double K)
            {
            K = context.ParentRelativePercent*K;
            writer.WriteLine(String.Format(culture, "{0}:[{1:F2}%]:[{2}]:[{3:F0} ms]:[{4}]:[{5}]", new String(' ', indent), K*100.0,
                context.Identity.ShortName, context.Duration.TotalMilliseconds, context.Identity, context.Hit));
            foreach (var i in context.Children.OrderByDescending(i => i.ParentRelativePercent)) {
                Write(writer, i, indent + 2, K);
                }
            }

        public void Write(TextWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteLine("------------------------");
            var i = 0;
            foreach (var j in P.OrderBy(j => j.Identity.ToString()))
                {
                var r = new StringBuilder();
                r.Append($"{i:D3}:[{j.Identity}]:[{j.AverageDuration}]:[{j.Count}]");
                if (!String.IsNullOrEmpty(j.VelocityString)) {
                    r.Append($":[{j.VelocityString}]");
                    }
                writer.WriteLine(r.ToString());
                i++;
                }
            writer.WriteLine("------------------------");
            foreach (var j in H.OrderBy(j => j.Identity.ToString()))
                {
                Write(writer, j, 0, 1.0);
                i++;
                }
            writer.WriteLine("------------------------");
            }
        }
    }
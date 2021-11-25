using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BinaryStudio.Diagnostics
    {
    internal class TraceContext : IDisposable, IEquatable<TraceContext>, ITraceContext
        {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Stopwatch watch;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly TraceManager manager;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly IDictionary<TraceContextIdentity, TraceContext> children = new Dictionary<TraceContextIdentity, TraceContext>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public Boolean IsCompleted { get;private set; }
        private readonly Int32 hashcode;
        public Int64 DataSize { get;set; }
        public ICollection<TraceContext> Children { get { return children.Values; }}
        public TraceContext Parent { get;private set; }

        private TraceContext(TraceManager manager)
            {
            if (manager == null) { throw new ArgumentNullException(nameof(manager)); }
            this.manager = manager;
            }

        internal TraceContext(TraceManager manager, StackFrame frame)
            :this(manager)
            {
            if (frame == null) { throw new ArgumentNullException(nameof(frame)); }
            Identity = new TraceContextStackFrameIdentity(frame);
            hashcode = Identity.GetHashCode();
            watch = new Stopwatch();
            watch.Start();
            manager.PushContext(this);
            }

        internal TraceContext(TraceManager manager, String shortname, String key)
            :this(manager)
            {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            Identity = new TraceContextStringIdentity(shortname, key);
            hashcode = Identity.GetHashCode();
            watch = new Stopwatch();
            watch.Start();
            manager.PushContext(this);
            }

        internal TraceContext(TraceManager manager, String shortname, String key, Int64 datasize)
            :this(manager, shortname, key)
            {
            DataSize = datasize;
            }

        internal TraceContext(TraceManager manager, TraceContextIdentity key)
            :this(manager)
            {
            Identity = key;
            hashcode = Identity.GetHashCode();
            watch = new Stopwatch();
            watch.Start();
            manager.PushContext(this);
            }

        internal TraceContext(TraceManager manager, TraceContextIdentity key, Int64 datasize)
            :this(manager, key)
            {
            DataSize = datasize;
            }

        #region M:GetHashCode:Int32
        /**
         * <summary>Serves as the default hash function.</summary>
         * <returns>A hash code for the current object.</returns>
         * */
        public override Int32 GetHashCode()
            {
            return hashcode;
            }
        #endregion
        #region M:Equals(TraceContext):Boolean
        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
         * */
        public Boolean Equals(TraceContext other)
            {
            if (other == null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Identity.Equals(other.Identity);
            }
        #endregion
        #region M:Equals(Object):Boolean
        /**
         * <summary>Determines whether the specified object is equal to the current object.</summary>
         * <param name="other">The object to compare with the current object.</param>
         * <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
         * */
        public override Boolean Equals(Object other)
            {
            if (other == null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            var r = other as TraceContext;
            if (r == null) { return false; }
            return Equals(r);
            }
        #endregion

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            watch.Stop();
            Duration = watch.Elapsed;
            Velocity = DataSize/Duration.TotalMilliseconds;
            IsCompleted = true;
            Hit = 1;
            manager.PopContext();
            }

        public TimeSpan Duration { get; private set; }
        public Double Velocity { get; private set; }
        public Double ParentRelativePercent { get {
            if (Parent == null) { return 1.0; }
            return (Parent.IsCompleted)
                ? Duration.TotalMilliseconds/Parent.Duration.TotalMilliseconds
                : 0.0;
            }}
        public TraceContextIdentity Identity { get; }
        public Int32 Hit { get;private set; }

        #region M:ToString:String
        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * */
        public override String ToString()
            {
            return $"{Identity}";
            }
        #endregion

        internal void UnionWith(TraceContext context)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (!context.IsCompleted) { throw new ArgumentOutOfRangeException(nameof(context)); }
            if (!context.Identity.Equals(Identity)) { throw new ArgumentOutOfRangeException(nameof(context)); }
            Hit += context.Hit;
            Duration += context.Duration;
            Velocity = (Velocity + context.Velocity)/2.0;
            foreach (var i in context.Children) {
                Add(i);
                }
            }

        public void Add(TraceContext context)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (!context.IsCompleted) { throw new ArgumentOutOfRangeException(nameof(context)); }
            if (!children.TryGetValue(context.Identity, out TraceContext r))
                {
                children.Add(context.Identity, context);
                context.Parent = this;
                }
            else
                {
                r.UnionWith(context);
                }
            }
        }
    }
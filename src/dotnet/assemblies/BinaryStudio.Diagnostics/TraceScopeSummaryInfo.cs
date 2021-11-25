using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace BinaryStudio.Diagnostics
    {
    internal class TraceScopeSummaryInfo
        {
        private static readonly CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
        public TimeSpan AverageDuration { get;set; }
        public TimeSpan MaxDuration { get;set; }
        public TimeSpan MinDuration { get;set; }
        public Double AverageVelocity { get;set; }
        public TraceContextIdentity Identity { get; }

        public Int32 Count { get;set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal ReaderWriterLockSlim @lock = new ReaderWriterLockSlim();

        public TraceScopeSummaryInfo(TraceContextIdentity identity)
            {
            Identity = identity;
            }

        internal String VelocityString { get {
            if (AverageVelocity == 0.0) { return String.Empty; }
            var r = AverageVelocity*0.9765625;
            if (r > 1048576) { return String.Format(culture, "{0:F2} GB/s", r / 1048576); }
            if (r > 1024) { return String.Format(culture, "{0:F2} MB/s", r / 1024); }
            return String.Format(culture, "{0:F2} KB/s", r);
            }}

        public override String ToString()
            {
            if (AverageVelocity > 0)
                {
                return $"{AverageDuration}:{VelocityString}";
                }
            return AverageDuration.ToString();
            }
        }
    }
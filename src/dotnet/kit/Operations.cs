using System;
using System.Collections.Generic;
using System.Threading;

namespace Kit
    {
    public static class Operations
        {
        private static IDisposable ReadLock(ReaderWriterLockSlim o)            { return new ReadLockScope(o);            }
        private static IDisposable WriteLock(ReaderWriterLockSlim o)           { return new WriteLockScope(o);           }
        private static IDisposable UpgradeableReadLock(ReaderWriterLockSlim o) { return new UpgradeableReadLockScope(o); }

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

        private static readonly Queue<Message> queue = new Queue<Message>();
        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();

        #region M:TryDequeue([Out]Message):Boolean
        public static Boolean TryDequeue(out Message message)
            {
            message = null;
            using (UpgradeableReadLock(o)) {
                if (queue.Count > 0) {
                    using (WriteLock(o)) {
                        message = queue.Dequeue();
                        return true;
                        }
                    }
                }
            return false;
            }
        #endregion
        #region M:Enqueue(Message)
        public static void Enqueue(Message message) {
            if (message != null) {
                using (WriteLock(o)) {
                    queue.Enqueue(message);
                    }
                }
            }
        #endregion
        }
    }
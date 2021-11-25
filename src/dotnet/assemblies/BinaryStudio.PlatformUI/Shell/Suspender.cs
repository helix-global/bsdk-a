using System;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI
    {
    public sealed class Suspender : ISuspendable
        {
        private class SuspendScope : DisposableObject
            {
            private readonly ISuspendable _suspendable;

            public SuspendScope(ISuspendable suspender)
                {
                _suspendable = suspender;
                _suspendable.Suspend();
                }

            protected override void DisposeManagedResources()
                {
                _suspendable.Resume();
                }
            }

        private Int32 _suspendCount;

        private readonly Action _resumeAction;

        public Boolean IsSuspended
            {
            get
                {
                return _suspendCount > 0;
                }
            }

        public Int32 SuspendCount
            {
            get
                {
                return _suspendCount;
                }
            }

        public Suspender(Action resumeAction = null)
            {
            _resumeAction = resumeAction;
            }

        public IDisposable Suspend()
            {
            return new SuspendScope(this);
            }

        void ISuspendable.Suspend()
            {
            _suspendCount++;
            }

        void ISuspendable.Resume()
            {
            var num = _suspendCount - 1;
            _suspendCount = num;
            if (num == 0 && _resumeAction != null)
                {
                _resumeAction();
                }
            }
        }
    }
using System;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal abstract class InvokableBase
        {
        public Exception Exception { get; private set; }

        protected abstract void InvokeMethod();

        public Int32 Invoke()
            {
            VerifyAccess();
            try
                {
                InvokeMethod();
                }
            catch (Exception ex)
                {
                Exception = ex;
                }
            return 0;
            }

        private void VerifyAccess()
            {
            if (!ThreadHelper.CheckAccess())
                throw new InvalidOperationException(Resources.Services_InvokedOnWrongThread);
            }
        }
    }
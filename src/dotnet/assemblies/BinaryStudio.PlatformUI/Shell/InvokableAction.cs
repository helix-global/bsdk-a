using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal class InvokableAction : InvokableBase
        {
        private Action a;
        private VsExecutionContextTrackerHelper.CapturedContext context;

        public InvokableAction(Action a, Boolean captureContext = false)
            {
            this.a = a;
            if (!captureContext)
                return;
            context = VsExecutionContextTrackerHelper.CaptureCurrentContext();
            }

        protected override void InvokeMethod()
            {
            if (context != null)
                context.ExecuteUnderContext(a);
            else
                a();
            }
        }
    }
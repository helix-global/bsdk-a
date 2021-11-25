using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal static class VsExecutionContextTrackerHelper
        {
        private static IVsExecutionContextTracker _instance;

        public static IVsExecutionContextTracker Instance
            {
            get
                {
                //if (VsExecutionContextTrackerHelper._instance == null)
                //  VsExecutionContextTrackerHelper._instance = Package.GetGlobalService(typeof (SVsExecutionContextTracker)) as IVsExecutionContextTracker;
                //return VsExecutionContextTrackerHelper._instance;
                return null;
                }
            }

        public static UInt32 GetCurrentContext()
            {
            if (Instance != null)
                {
                // ISSUE: reference to a compiler-generated method
                return Instance.GetCurrentContext();
                }
            return 0;
            }

        public static CapturedContext CaptureCurrentContext()
            {
            return new CapturedContext();
            }

        public class CapturedContext : IDisposable
            {
            private readonly UInt32 capturedContext;

            internal CapturedContext()
                {
                capturedContext = 0U;
                if (Instance == null)
                    return;
                // ISSUE: reference to a compiler-generated method
                capturedContext = Instance.GetCurrentContext();
                }

            public void ExecuteUnderContext(Action t)
                {
                if (Instance != null)
                    {
                    // ISSUE: reference to a compiler-generated method
                    Instance.PushContext(capturedContext);
                    }
                t();
                if (Instance == null)
                    return;
                // ISSUE: reference to a compiler-generated method
                Instance.PopContext(capturedContext);
                }

            public void Dispose()
                {
                if (Instance == null)
                    return;
                // ISSUE: reference to a compiler-generated method
                Instance.ReleaseContext(capturedContext);
                }
            }
        }
    }
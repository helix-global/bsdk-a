using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal class GenericThreadHelper : ThreadHelper
        {
        protected override IDisposable GetInvocationWrapper()
            {
            return null;
            }
        }
    }
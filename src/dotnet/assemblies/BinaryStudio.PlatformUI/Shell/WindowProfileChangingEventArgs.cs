using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class WindowProfileChangingEventArgs : EventArgs
        {
        public WindowProfile OldWindowProfile { get; }

        public WindowProfile NewWindowProfile { get; }

        public WindowProfileChangingEventArgs(WindowProfile oldProfile, WindowProfile newProfile)
            {
            OldWindowProfile = oldProfile;
            NewWindowProfile = newProfile;
            }
        }
    }
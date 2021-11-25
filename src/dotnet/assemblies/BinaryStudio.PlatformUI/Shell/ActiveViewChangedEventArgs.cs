using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class ActiveViewChangedEventArgs : EventArgs
        {
        public ActivationType ActivationType { get; }

        public ActiveViewChangedEventArgs(ActivationType type)
            {
            ActivationType = type;
            }
        }
    }
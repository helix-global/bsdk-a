using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class DockEventArgs : EventArgs
        {
        public DockAction Action { get; }
        public ViewElement ChangingElement { get; }
        public Boolean IsActionSuccessful { get; internal set; }

        public DockEventArgs(DockAction action, ViewElement changingElement, Boolean isActionSuccessful = true)
            {
            Action = action;
            ChangingElement = changingElement;
            IsActionSuccessful = isActionSuccessful;
            }
        }
    }
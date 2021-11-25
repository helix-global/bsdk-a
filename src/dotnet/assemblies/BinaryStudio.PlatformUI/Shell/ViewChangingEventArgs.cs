using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class ViewChangingEventArgs : EventArgs
        {
        public View OldView { get; }

        public View NewView { get; }

        public ViewChangingEventArgs(View oldView, View newView)
            {
            OldView = oldView;
            NewView = newView;
            }
        }
    }
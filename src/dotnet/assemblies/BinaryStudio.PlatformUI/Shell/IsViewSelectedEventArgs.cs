using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class IsViewSelectedEventArgs : EventArgs
        {
        public View View { get; set; }

        public IsViewSelectedEventArgs(View view)
            {
            View = view;
            }
        }
    }
using System;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class RowIndexChangedEventArgs : EventArgs
        {
        public View View
            {
            get;
            }

        public RowIndexChangedEventArgs(View view)
            {
            View = view;
            }
        }
    }
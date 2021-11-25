using System;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class IsPreviewViewChangedEventArgs : EventArgs
        {
        public View View { get; }

        public Boolean OldValue { get; }

        public Boolean NewValue { get; }

        public Boolean Replaced { get; }

        public IsPreviewViewChangedEventArgs(View view, Boolean oldValue, Boolean newValue, Boolean replaced)
            {
            View = view;
            OldValue = oldValue;
            NewValue = newValue;
            Replaced = replaced;
            }
        }
    }
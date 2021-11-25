using System;
using System.Collections.Generic;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class SelectedItemHiddenEventArgs : EventArgs
        {
        public class ViewIndexChange {
            public View View { get; }
            public Int32 NewIndex { get; }

            public ViewIndexChange(View view, Int32 newIndex) {
                View = view;
                NewIndex = newIndex;
                }
            }

        public List<ViewIndexChange> ViewsToMove {get; }

        public SelectedItemHiddenEventArgs(List<ViewIndexChange> viewsToMove) {
            ViewsToMove = viewsToMove;
            }
        }
    }
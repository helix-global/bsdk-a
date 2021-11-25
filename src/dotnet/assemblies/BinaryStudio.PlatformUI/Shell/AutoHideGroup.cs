using System;
using System.Collections.Generic;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class AutoHideGroup : ViewGroup
        {
        public AutoHideGroup() {

            }

        public AutoHideGroup(IEnumerable<View> children) {
            if (children != null) {
                foreach (var child in children) {
                    Children.Add(child);
                    }
                }
            }

        public ViewGroup OriginalGroup {
            get
                {
                if (Children.Count == 0)
                    return null;
                var child = (View)Children[0];
                var bookmark = DockOperations.FindBookmark(child, child.WindowProfile);
                if (bookmark == null)
                    return null;
                return bookmark.Parent;
                }
            }

        public override Boolean IsChildAllowed(ViewElement e) {
            return e is View;
            }

        public static AutoHideGroup Create() {
            return ViewElementFactory.Current.CreateAutoHideGroup();
            }

        public override Boolean IsChildOnScreen(Int32 childIndex) {
            if (IsOnScreen)
                return Children[childIndex].IsSelected;
            return false;
            }

        protected override void TryCollapseCore() {
            if (Children.Count != 0)
                return;
            Detach();
            }
        }
    }
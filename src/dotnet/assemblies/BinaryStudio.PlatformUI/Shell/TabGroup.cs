using System;
using System.Collections.Generic;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class TabGroup : NestedGroup
        {
        public TabGroup() {}
        public TabGroup(IEnumerable<ViewElement> children) {
            if (children != null) {
                foreach (var child in children) {
                    Children.Add(child);
                    }
                }
            }

        #region M:IsChildAllowed(ViewElement):Boolean
        public override Boolean IsChildAllowed(ViewElement e) {
            if (!(e is View)) {
                return e is ViewBookmark;
                }
            return true;
            }
        #endregion
        #region M:IsTabbed(ViewElement):Boolean
        public static Boolean IsTabbed(ViewElement element) {
            return element.FindAncestor<TabGroup>() != null;
            }
        #endregion
        #region M:Create:TabGroup
        public static TabGroup Create() {
            return ViewElementFactory.Current.CreateTabGroup();
            }
        #endregion
        #region M:TryCollapseCore
        protected override void TryCollapseCore() {
            if (Children.Count == 0) { Detach(); }
            else
                {
                if (Children.Count != 1 || Parent == null) { return; }
                Parent.AbsorbElementWithOnlyOneChild(this);
                }
            }
        #endregion
        }
    }
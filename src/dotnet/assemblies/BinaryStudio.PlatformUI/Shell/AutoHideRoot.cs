using System;
using System.Linq;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell {
    [XamlSerializable]
    public class AutoHideRoot : ViewGroup {
        #region P:AutoHideRoot.IsAutoHideCenter:Boolean
        public static readonly DependencyProperty IsAutoHideCenterProperty = DependencyProperty.RegisterAttached("IsAutoHideCenter", typeof(Boolean), typeof(AutoHideRoot), new PropertyMetadata(default(Boolean)));
        public static void SetIsAutoHideCenter(DependencyObject source, Boolean value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(IsAutoHideCenterProperty, value);
            }

        public static Boolean GetIsAutoHideCenter(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (Boolean)source.GetValue(IsAutoHideCenterProperty);
            }
        #endregion

        #region M:IsChildAllowed(ViewElement):Boolean
        public override Boolean IsChildAllowed(ViewElement e) {
            if (!(e is AutoHideChannel))
                return e is DockRoot;
            return true;
            }
        #endregion

        public static AutoHideRoot Create() {
            return ViewElementFactory.Current.CreateAutoHideRoot();
            }

        public static Boolean HasVisibleChannels(ViewElement element) {
            var viewElement = element;
            var num = 1;
            var autoHideRoot = (AutoHideRoot)viewElement.Find(e => e is AutoHideRoot, num != 0);
            if (autoHideRoot == null)
                return false;
            return autoHideRoot.VisibleChildren.Any(e => e is AutoHideChannel);
            }

        #region M:UpdateHasDocumentGroupContainer
        protected override void UpdateHasDocumentGroupContainer() {
            base.UpdateHasDocumentGroupContainer();
            UpdateDockRestrictions();
            }
        #endregion
        #region M:UpdateIsDockingEnabled
        protected override void UpdateIsDockingEnabled() {
            base.UpdateIsDockingEnabled();
            UpdateDockRestrictions();
            }
        #endregion
        #region M:UpdateDockRestrictions
        private void UpdateDockRestrictions() {
            DockRestriction = !IsDockingEnabled
                ? DockRestrictionType.AlwaysFloating
                : DockRestrictionType.DocumentGroup | DockRestrictionType.Document | DockRestrictionType.OutsideView;
            }
        #endregion
        #region M:TryCollapseCore
        protected override void TryCollapseCore() {
            if (VisibleChildren.Count != 0) { return; }
            Detach();
            }
        #endregion

        public AutoHideRoot()
            {
            }

        public AutoHideRoot(DockRoot e)
            {
            Add(e);
            }
        }
    }
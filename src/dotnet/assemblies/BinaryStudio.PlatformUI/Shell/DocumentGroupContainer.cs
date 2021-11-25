using System;
using System.ComponentModel;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class DocumentGroupContainer : DockGroup
        {
        private FloatSite _floatSite;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WindowProfileSerializationVariants SerializationVariants {
            get
                {
                return WindowProfileSerializationVariants.Default | WindowProfileSerializationVariants.Restricted;
                }
            }

        private FloatSite FloatSite {
            get
                {
                return _floatSite;
                }
            set
                {
                if (_floatSite == value)
                    return;
                if (_floatSite != null)
                    _floatSite.OnScreenViewCardinalityChanged -= OnFloatSiteOnScreenViewCardinalityChanged;
                _floatSite = value;
                if (_floatSite != null)
                    _floatSite.OnScreenViewCardinalityChanged += OnFloatSiteOnScreenViewCardinalityChanged;
                UpdateVisibility();
                }
            }

        public DocumentGroupContainer() {
            UpdateVisibility();
            AutoHideRoot.SetIsAutoHideCenter(this, true);
            }

        public DocumentGroupContainer(DocumentGroup child) {
            if (child != null) {
                Children.Add(child);
                }
            }

        public override Boolean IsChildAllowed(ViewElement e) {
            return e is DocumentGroup;
            }

        protected internal override void OnChildVisibilityChangedCore() {
            RebuildVisibleChildren();
            }

        protected override void OnOnScreenViewCardinalityChanged(DependencyPropertyChangedEventArgs args) {
            base.OnOnScreenViewCardinalityChanged(args);
            UpdateVisibility();
            }

        protected override void TryCollapseCore() {
            if (Parent == null)
                return;
            var rootElement = FindRootElement() as FloatSite;
            if (rootElement == null)
                return;
            var floatSite = rootElement;
            Predicate<ViewElement> predicate1 = ve =>
            {
                if (!(ve is View))
                    return ve is ViewBookmark;
                return true;
            };
            var num = 0;

            if (floatSite.Find(predicate1, num != 0) != null)
                return;
            Detach();
            }

        private void OnFloatSiteOnScreenViewCardinalityChanged(Object sender, EventArgs e) {
            UpdateVisibility();
            }

        protected internal override void OnAncestorChanged() {
            FloatSite = this.FindAncestor<FloatSite>();
            base.OnAncestorChanged();
            }

        private void UpdateVisibility() {
            IsVisible = ComputeVisibility();
            }

        private Boolean ComputeVisibility() {
            return FloatSite == null || FloatSite.OnScreenViewCardinality != OnScreenViewCardinality.Zero || OnScreenViewCardinality != OnScreenViewCardinality.Zero;
            }

        public static DocumentGroupContainer Create() {
            return ViewElementFactory.Current.CreateDocumentGroupContainer();
            }
        }
    }
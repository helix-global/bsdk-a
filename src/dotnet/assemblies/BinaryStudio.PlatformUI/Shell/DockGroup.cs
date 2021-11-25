using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class DockGroup : ViewGroup
        {
        public DockGroup()
            {
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        public DockGroup(params ViewElement[] children) {
            if (children != null) {
                foreach (var child in children) {
                    Children.Add(child);
                    }
                }
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="children"></param>
        public DockGroup(Orientation orientation, params ViewElement[] children)
            :this(children)
            {
            Orientation = orientation;
            }

        #region P:Orientation:Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DockGroup));
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }
        #endregion

        public override ICustomXmlSerializer CreateSerializer() {
            return new DockGroupCustomSerializer(this);
            }

        public override Boolean IsChildAllowed(ViewElement e) {
            if (!(e is DockGroup) && !(e is View) && !(e is ViewBookmark))
                return e is TabGroup;
            return true;
            }

        protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs e) {
            base.OnChildrenChanged(e);
            if (e.NewItems != null) {
                SnapChildrenSizesToSelf(e.NewItems);
                }
            else {
                if (e.Action != NotifyCollectionChangedAction.Reset)
                    return;
                SnapChildrenSizesToSelf(Children);
                }
            }

        protected override void OnDockedHeightChanged() {
            base.OnDockedHeightChanged();
            if (Orientation != Orientation.Horizontal)
                return;
            SnapChildrenSizesToSelf(Children);
            }

        protected override void OnDockedWidthChanged() {
            base.OnDockedWidthChanged();
            if (Orientation != Orientation.Vertical)
                return;
            SnapChildrenSizesToSelf(Children);
            }

        private void SnapChildrenSizesToSelf(IEnumerable children) {
            if (Orientation == Orientation.Horizontal) {
                if (DockedHeight.IsFill)
                    return;
                foreach (ViewElement child in children)
                    child.DockedHeight = DockedHeight;
                }
            else {
                if (DockedWidth.IsFill)
                    return;
                foreach (ViewElement child in children)
                    child.DockedWidth = DockedWidth;
                }
            }

        protected override void UpdateOnScreenViewCardinality() {
            if (VisibleChildren.Count > 1)
                OnScreenViewCardinality = OnScreenViewCardinality.Many;
            else
                base.UpdateOnScreenViewCardinality();
            }

        protected override void TryCollapseCore() {
            switch (Children.Count) {
                case 0:
                    Detach();
                    break;
                case 1:
                    if (Parent == null)
                        break;
                    Parent.AbsorbElementWithOnlyOneChild(this);
                    break;
                default:
                    var parent = Parent as DockGroup;
                    if (parent == null || parent.Orientation != Orientation)
                        break;
                    parent.AbsorbElementWithMultipleChildren(this);
                    break;
                }
            }

        private void AbsorbElementWithMultipleChildren(DockGroup childGroup) {
            Validate.IsNotNull(childGroup, "childGroup");
            var index = Children.IndexOf(childGroup);
            var viewElementList = new List<ViewElement>(childGroup.Children);
            using (childGroup.PreventCollapse()) {
                foreach (var viewElement in viewElementList) {
                    Children.Insert(index, viewElement);
                    ++index;
                    }
                Children.Remove(childGroup);
                }
            foreach (var viewElement in viewElementList)
                viewElement.TryCollapse();
            }

        public static DockGroup Create() {
            return ViewElementFactory.Current.CreateDockGroup();
            }
        }
    }
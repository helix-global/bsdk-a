using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell {
    [XamlSerializable]
    public abstract class NestedGroup : ViewGroup
        {
        #region P:ResolveSelectionMode:ResolveSelectionMode
        public static readonly DependencyProperty ResolveSelectionModeProperty = DependencyProperty.Register("ResolveSelectionMode", typeof(ResolveSelectionMode), typeof(GroupControl), new FrameworkPropertyMetadata(ResolveSelectionMode.NearestElement));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ResolveSelectionMode ResolveSelectionMode
            {
            get { return (ResolveSelectionMode)GetValue(ResolveSelectionModeProperty); }
            set { SetValue(ResolveSelectionModeProperty, value); }
            }
        #endregion

        protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs e) {
            base.OnChildrenChanged(e);
            if (e.NewItems != null)
                {
                SnapChildrenSizesToSelf(e.NewItems);
                }
            else
                {
                if (e.Action != NotifyCollectionChangedAction.Reset)
                    return;
                SnapChildrenSizesToSelf(Children);
                }
            }

        protected override void UpdateSelectionCore(Int32 oldIndex) {
            if (oldIndex < 0 || oldIndex >= Children.Count)
                throw new ArgumentOutOfRangeException(nameof(oldIndex), oldIndex, "oldIndex must be a valid index in the Children collection");
            if (ResolveSelectionMode == ResolveSelectionMode.NearestElement)
                {
                for (var index = oldIndex + 1; index < Children.Count; ++index)
                    {
                    if (Children[index].IsVisible)
                        {
                        SelectedElement = Children[index];
                        return;
                        }
                    }
                for (var index = oldIndex - 1; index >= 0; --index)
                    {
                    if (Children[index].IsVisible)
                        {
                        SelectedElement = Children[index];
                        return;
                        }
                    }
                SelectedElement = null;
                }
            else
                SelectedElement = SelectionOrder.FirstOrDefault(element => element.IsVisible);
            }

        protected override void ValidateDockedWidth(SplitterLength width) {
            if (width.IsFill)
                throw new ArgumentException("NestedGroup does not accept Fill values for DockedWidth.");
            }

        protected override void ValidateDockedHeight(SplitterLength height) {
            if (height.IsFill)
                throw new ArgumentException("NestedGroup does not accept Fill values for DockedHeight.");
            }

        protected override void OnDockedWidthChanged() {
            base.OnDockedWidthChanged();
            SnapChildrenSizesToSelf(Children);
            }

        protected override void OnDockedHeightChanged() {
            base.OnDockedHeightChanged();
            SnapChildrenSizesToSelf(Children);
            }

        private void SnapChildrenSizesToSelf(IEnumerable children) {
            foreach (ViewElement child in children)
                {
                child.DockedWidth = DockedWidth;
                child.DockedHeight = DockedHeight;
                }
            }

        public override Boolean IsChildOnScreen(Int32 childIndex) {
            if (IsOnScreen && Children[childIndex].IsVisible)
                return Children[childIndex].IsSelected;
            return false;
            }

        protected override void CheckSelectedElementSet(ViewElement suggestedItemToSelect) {
            Validate.IsNotNull(suggestedItemToSelect, "suggestedItemToSelect");
            if (SelectedElement != null || !suggestedItemToSelect.IsVisible)
                return;
            SelectedElement = suggestedItemToSelect;
            }
        }
    }
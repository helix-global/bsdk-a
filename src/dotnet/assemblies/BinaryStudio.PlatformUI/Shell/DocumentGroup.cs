using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable(FactoryMethodName = "Create")]
    public class DocumentGroup : NestedGroup
        {
        private readonly Suspender _childReorderSuspender = new Suspender(null);
        public static readonly DependencyProperty IsPreviewViewProperty = DependencyProperty.RegisterAttached("IsPreviewView", typeof(Boolean), typeof(DocumentGroup), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsPreviewViewChanged));
        public static readonly DependencyProperty IsMultiSelectedProperty = DependencyProperty.RegisterAttached("IsMultiSelected", typeof(Boolean), typeof(DocumentGroup), new PropertyMetadata(Boxes.BooleanFalse, OnIsMultiSelectedChanged, IsMultiSelectedCoerceValue));
        private static readonly DependencyPropertyKey HasPinnedViewsKey = DependencyProperty.RegisterReadOnly("HasPinnedViews", typeof(Boolean), typeof(DocumentGroup), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty HasPinnedViewsProperty = HasPinnedViewsKey.DependencyProperty;
        private readonly ObservableCollection<ViewElement> pinnedViews;
        private readonly ImmutableObservableCollection<ViewElement> readOnlyPinnedViews;
        private DispatcherOperation _asyncChildReorderOperation;
        private Boolean _replacingPreviewView;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WindowProfileSerializationVariants SerializationVariants {
            get
                {
                return WindowProfileSerializationVariants.Default | WindowProfileSerializationVariants.Restricted;
                }
            }

        #region P:PreviewView:View
        public static readonly DependencyProperty PreviewViewProperty = DependencyProperty.Register("PreviewView", typeof(View), typeof(DocumentGroup), new FrameworkPropertyMetadata(null, OnPreviewViewChanged, CoercePreviewView), IsValidPreviewView);
        public View PreviewView {
            get { return (View)GetValue(PreviewViewProperty); }
            set { SetValue(PreviewViewProperty, value); }
            }

        private static void OnPreviewViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((DocumentGroup)sender).OnPreviewViewChanged(e.OldValue as View, e.NewValue as View);
            }

        private void OnPreviewViewChanged(View oldView, View newView) {
            using (SuspendChildValidation()) {
                try
                    {
                    PreviewViewChanging.RaiseEvent(this, new ViewChangingEventArgs(oldView, newView));
                    var flag = false;
                    _replacingPreviewView = oldView != null && newView != null;
                    if (oldView != null) {
                        SetIsPreviewView(oldView, false);
                        oldView.Hidden -= OnPreviewViewHidden;
                        if (newView != null) {
                            flag = true;
                            }
                        else
                            {
                            var oldIndex = Children.IndexOf(oldView);
                            if (oldIndex >= 0) {
                                var newIndex = ViewManager.Instance.Preferences.DocumentDockPreference != DockPreference.DockAtBeginning ? (oldView.IsPinned ? PinnedViews.Count : Children.Count - 1) : (oldView.IsPinned ? 0 : PinnedViews.Count);
                                if (oldIndex != newIndex) {
                                    var num = (Int32)MoveChild(oldIndex, newIndex);
                                    }
                                }
                            }
                        }
                    if (newView != null) {
                        SetIsPreviewView(newView, true);
                        newView.Hidden += OnPreviewViewHidden;
                        var num = (Int32)MoveChild(newView, Children.Count - 1);
                        if (oldView != null)
                            newView.IsSelected = oldView.IsSelected;
                        }
                    if (flag) { oldView.Hide(); }
                    PreviewViewChanged.RaiseEvent(this);
                    }
                finally
                    {
                    _replacingPreviewView = false;
                    }
                }
            }

        private static Object CoercePreviewView(DependencyObject sender, Object value) {
            return ((DocumentGroup)sender).CoercePreviewView((View)value);
            }

        private Object CoercePreviewView(View view) {
            if (view == null) { return null; }
            return Children.Contains(view)
                ? view
                : View.InvalidView;
            }

        private static Boolean IsValidPreviewView(Object value) {
            return !Equals(value, View.InvalidView);
            }
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ViewElement> MultiSelectedElements {
            get
                {
                foreach (var child in Children) {
                    if (child != null && GetIsMultiSelected(child))
                        yield return child;
                    }
                }
            }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean HasPinnedViews {
            get
                {
                return (Boolean)GetValue(HasPinnedViewsProperty);
                }
            private set
                {
                SetValue(HasPinnedViewsKey, Boxes.Box(value));
                }
            }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IObservableCollection<ViewElement> PinnedViews {
            get
                {
                return readOnlyPinnedViews;
                }
            }

        private Int32 PreviewViewIndex {
            get
                {
                if (PreviewView == null)
                    return -1;
                return Children.Count - 1;
                }
            }

        public event EventHandler PreviewViewChanged;

        public event EventHandler<ViewChangingEventArgs> PreviewViewChanging;

        public static event EventHandler<IsPreviewViewChangedEventArgs> IsPreviewViewChanged;

        static DocumentGroup() {
            ResolveSelectionModeProperty.OverrideMetadata(typeof(DocumentGroup), new FrameworkPropertyMetadata(ResolveSelectionMode.MostRecentSelection));
            }

        public DocumentGroup() {
            pinnedViews = new ObservableCollection<ViewElement>();
            readOnlyPinnedViews = new ImmutableObservableCollection<ViewElement>(pinnedViews);
            pinnedViews.CollectionChanged += OnPinnedViewsCollectionChanged;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        public DocumentGroup(IEnumerable<View> children)
            :this()
            {
            if (children != null) {
                foreach (var child in children) {
                    Children.Add(child);
                    }
                }
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="children"></param>
        public DocumentGroup(params View[] children)
            :this()
            {
            if (children != null) {
                foreach (var child in children) {
                    Children.Add(child);
                    }
                }
            }

        private static void OnIsPreviewViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var view = (View)sender;
            var ancestor = view.FindAncestor<DocumentGroup>();
            var args = new IsPreviewViewChangedEventArgs(view, (Boolean)e.OldValue, (Boolean)e.NewValue, ancestor._replacingPreviewView);
            // ISSUE: reference to a compiler-generated field
            IsPreviewViewChanged.RaiseEvent(null, args);
            }

        public static Boolean GetIsPreviewView(View view) {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return (Boolean)view.GetValue(IsPreviewViewProperty);
            }

        private static void SetIsPreviewView(View view, Boolean isPreview) {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            view.SetValue(IsPreviewViewProperty, Boxes.Box(isPreview));
            }

        public static Boolean GetIsMultiSelected(ViewElement element) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Boolean)element.GetValue(IsMultiSelectedProperty);
            }

        public static void SetIsMultiSelected(ViewElement element, Boolean value) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(IsMultiSelectedProperty, Boxes.Box(value));
            }

        private static void OnIsMultiSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (ViewElement)sender;
            ViewElement parent = source.Parent;
            var num = (Boolean)e.NewValue ? 1 : 0;
            if (GetIsMultiSelected(source) || !source.IsSelected || !NativeMethods.IsKeyPressed(17)) { return; }
            var documentGroup = parent as DocumentGroup;
            if (documentGroup == null)
                return;
            foreach (var element2 in documentGroup.SelectionOrder) {
                if (GetIsMultiSelected(element2)) {
                    element2.IsSelected = true;
                    break;
                    }
                }
            }

        private static Object IsMultiSelectedCoerceValue(DependencyObject sender, Object value) {
            if ((Boolean)value) {
                var source = (ViewElement)sender;
                var parent = source.Parent as DocumentGroup;
                if (parent != null)
                    return Boxes.Box(parent.IsMultiSelectedCoerceValue(source, (Boolean)value));
                }
            return Boxes.BooleanFalse;
            }

        protected virtual Boolean IsMultiSelectedCoerceValue(ViewElement element, Boolean isMultiSelected) {
            return false;
            }

        public override Boolean IsChildAllowed(ViewElement e) {
            if (!(e is View))
                return e is ViewBookmark;
            return true;
            }

        public override Boolean CanHostPinnedViews() {
            return true;
            }

        public void MovePinnedView(ViewElement element, Int32 newPosition) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (newPosition < 0 || newPosition >= PinnedViews.Count)
                throw new ArgumentOutOfRangeException(nameof(newPosition), "Invalid position");
            if (!PinnedViews.Contains(element))
                throw new ArgumentException("element is not contained in pinned list.");
            pinnedViews.Move(PinnedViews.IndexOf(element), newPosition);
            }

        public static Boolean IsTabbedDocument(ViewElement element) {
            return element.FindAncestor<DocumentGroup, ViewElement>(e => (ViewElement)e.Parent) != null;
            }

        public static DocumentGroup Create() {
            return ViewElementFactory.Current.CreateDocumentGroup();
            }

        protected internal override void OnChildPinnedStatusChanged(ViewElement element) {
            var view = element as View;
            if (view == null)
                return;
            if (view.IsPinned) {
                pinnedViews.Add(view);
                if (!Equals(PreviewView, view))
                    return;
                PreviewView = null;
                }
            else
                pinnedViews.Remove(view);
            }

        private void OnPreviewViewHidden(Object sender, EventArgs e) {
            if (!Equals(PreviewView, sender))
                return;
            PreviewView = null;
            }

        protected internal override void OnChildrenChanged(NotifyCollectionChangedEventArgs e) {
            base.OnChildrenChanged(e);
            using (SuspendChildReordering()) {
                using (SuspendChildValidation()) {
                    var arrayList1 = new ArrayList();
                    var arrayList2 = new ArrayList();
                    switch (e.Action) {
                        case NotifyCollectionChangedAction.Add:
                            arrayList1.AddRange(e.NewItems);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            if (PreviewView != null && e.OldItems.Contains(PreviewView))
                                PreviewView = null;
                            arrayList2.AddRange(e.OldItems);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            arrayList1.AddRange(e.NewItems);
                            goto case NotifyCollectionChangedAction.Remove;
                        case NotifyCollectionChangedAction.Move:
                            if (PreviewView != null && !Equals(Children[PreviewViewIndex], PreviewView)) {
                                ScheduleAsyncChildReorder();
                                }
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            PreviewView = null;
                            pinnedViews.Clear();
                            if (e.NewItems != null) {
                                arrayList1.AddRange(e.NewItems);
                                }
                            break;
                        }
                    for (var index1 = 0; index1 < arrayList1.Count; ++index1) {
                        var view = arrayList1[index1] as View;
                        if (view != null) {
                            var index2 = e.NewStartingIndex + index1;
                            if (IsPastPreviewIndex(index2))
                                ScheduleAsyncChildReorder();
                            if (view.IsPinned) {
                                if (index2 > pinnedViews.Count)
                                    pinnedViews.Add(view);
                                else
                                    pinnedViews.Insert(index2, view);
                                }
                            else if (IsPinnedIndex(index2))
                                ScheduleAsyncChildReorder();
                            }
                        }
                    foreach (ViewElement viewElement in arrayList2) {
                        var view = viewElement as View;
                        if (view != null && view.IsPinned) {
                            if (!ViewManager.Instance.Preferences.MaintainPinStatus)
                                view.IsPinned = false;
                            pinnedViews.Remove(view);
                            }
                        }
                    }
                }
            }

        private Boolean IsPastPreviewIndex(Int32 index) {
            if (PreviewView == null || index <= 0)
                return false;
            return index > Children.IndexOf(PreviewView);
            }

        public override Boolean CanMoveTab(Int32 oldIndex, Int32 newIndex) {
            return IsValidIndex(oldIndex) && IsValidIndex(newIndex) && (!HasPinnedViews && PreviewView == null || oldIndex == newIndex || newIndex != PreviewViewIndex && IsPinnedIndex(oldIndex) == IsPinnedIndex(newIndex));
            }

        private Boolean IsPinnedIndex(Int32 index) {
            if (index >= 0)
                return index < pinnedViews.Count;
            return false;
            }

        private Boolean IsValidIndex(Int32 index) {
            if (index >= 0)
                return index < Children.Count;
            return false;
            }

        private void OnPinnedViewsCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            using (SuspendChildValidation()) {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        HandleAddedPinnedViews(e.NewItems, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        HandleRemovedPinnedViews(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        HandleRemovedPinnedViews(e.OldItems);
                        if (e.NewItems != null) {
                            HandleAddedPinnedViews(e.NewItems, e.NewStartingIndex);
                            }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        HandleMovedPinnedViews(e.NewItems, e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    }
                HasPinnedViews = PinnedViews.Count > 0;
                }
            }

        private void HandleAddedPinnedViews(IList newViews, Int32 newIndex) {
            var num1 = 0;
            if (newIndex > 0) {
                var num2 = Children.IndexOf(pinnedViews[newIndex]);
                var num3 = Children.IndexOf(pinnedViews[newIndex - 1]);
                num1 = num2 >= num3 ? num3 + 1 : num3;
                }
            var enumerator = newViews.GetEnumerator();
            try {
                do
                    ;
                while (enumerator.MoveNext() && MoveChild((ViewElement)enumerator.Current, num1++) != MoveResult.Scheduled);
                }
            finally {
                var disposable = enumerator as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                }
            }

        private void HandleMovedPinnedViews(IList movedViews, Int32 oldIndex, Int32 newIndex) {
            var count = movedViews.Count;
            while (count > 0 && MoveChild(oldIndex++, newIndex++) != MoveResult.Scheduled)
                --count;
            }

        private void HandleRemovedPinnedViews(IList oldViews) {
            if (pinnedViews.Count == 0)
                return;
            var count = pinnedViews.Count;
            foreach (ViewElement oldView in oldViews) {
                var oldIndex = Children.IndexOf(oldView);
                if (oldIndex != -1 && MoveChild(oldIndex, count++) == MoveResult.Scheduled)
                    break;
                }
            }

        private MoveResult MoveChild(ViewElement child, Int32 newIndex) {
            return MoveChild(Children.IndexOf(child), newIndex);
            }

        private MoveResult MoveChild(Int32 oldIndex, Int32 newIndex) {
            Validate.IsWithinRange(oldIndex, 0, Children.Count - 1, "oldIndex");
            Validate.IsWithinRange(newIndex, 0, Children.Count - 1, "newIndex");
            if (oldIndex == newIndex)
                return MoveResult.NotNeeded;
            if (_childReorderSuspender.IsSuspended) {
                ScheduleAsyncChildReorder();
                return MoveResult.Scheduled;
                }
            try {
                Children.Move(oldIndex, newIndex);
                return MoveResult.Moved;
                }
            catch (InvalidOperationException) {
                ScheduleAsyncChildReorder();
                return MoveResult.Scheduled;
                }
            }

        public IDisposable SuspendChildReordering() {
            return _childReorderSuspender.Suspend();
            }

        private void ScheduleAsyncChildReorder() {
            if (_asyncChildReorderOperation != null) {
                return;
                }
            var suspendValidationScope = SuspendChildValidation();
            try {
                _asyncChildReorderOperation = Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
            //IDisposable suspendValidationScope;
            using (suspendValidationScope) {
                        _asyncChildReorderOperation = null;
                        ReorderChildren();
                        }
                    }));
                }
            catch (Exception) {
                suspendValidationScope.Dispose();
                throw;
                }
            }

        private void ReorderChildren() {
            using (SuspendChildValidation()) {
                var source = new List<IndexBundle>();
                var flag = false;
                for (var index = 0; index < Children.Count; ++index) {
                    var child = Children[index] as View;
                    if (child != null) {
                        if (child.IsPinned) {
                            if (flag)
                                source.Add(new IndexBundle(index, pinnedViews.IndexOf(child)));
                            }
                        else {
                            if (index < PinnedViews.Count)
                                flag = true;
                            if (Equals(child, PreviewView) && index < Children.Count - 1) {
                                var num = (Int32)MoveChild(index, Children.Count - 1);
                                }
                            }
                        }
                    }
                var array = source.OrderBy(bundle => bundle.PinnedIndex).ToArray();
                for (var index1 = 0; index1 < array.Length - 1; ++index1) {
                    for (var index2 = index1 + 1; index2 < array.Length; ++index2) {
                        if (array[index1].ChildIndex > array[index2].ChildIndex)
                            ++array[index2].ChildIndex;
                        }
                    }
                var num1 = PinnedViews.Count - source.Count;
                foreach (var oldIndex in array.Select(bundle => bundle.ChildIndex)) {
                    var num2 = (Int32)MoveChild(oldIndex, num1++);
                    }
                }
            }

        protected override void TryCollapseCore() {
            if (Parent == null)
                return;
            var flag = false;
            var rootElement = FindRootElement() as FloatSite;
            if (rootElement != null)
                flag = rootElement.Find<View>(v =>
                {
                    if (v.IsVisible)
                        return !IsTabbedDocument(v);
                    return false;
                }, false) == null;
            if (Children.Count == 0 && new List<DocumentGroup>(Parent.FindAll<DocumentGroup>()).Count > 1 | flag) {
                Detach();
                }
            else
                {
                if (!flag || !IsVisible || (Children.Count != 1 || !Equals(Parent.Parent, rootElement))) { return; }
                using (rootElement.PreventCollapse()) {
                    var child = Children[0];
                    child.Detach();
                    rootElement.Child = child;
                    }
                }
            }

        private IDisposable SuspendChildValidation() {
            return null;
            }

        private enum MoveResult
            {
            NotNeeded,
            Moved,
            Scheduled,
            }

        private struct IndexBundle
            {
            public Int32 ChildIndex;
            public readonly Int32 PinnedIndex;

            public IndexBundle(Int32 childIndex, Int32 pinnedIndex) {
                ChildIndex = childIndex;
                PinnedIndex = pinnedIndex;
                }
            }
        }
    }
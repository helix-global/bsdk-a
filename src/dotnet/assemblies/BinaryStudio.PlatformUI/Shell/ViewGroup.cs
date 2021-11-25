using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Serialization;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell
    {
    [ContentProperty("Children")]
    [DefaultProperty("Children")]
    [XamlSerializable]
    public abstract class ViewGroup : ViewElement, ISupportInitialize, IEnumerable<ViewElement>
        {
        private ObservableCollection<ViewElement> visibleChildren;
        private ImmutableObservableCollection<ViewElement> readOnlyVisibleChildren;
        private Boolean childVisiblityChangedWhileNotCollapsible;
        private Int32 _deferRebuildVisibleChildrenCount;
        private Boolean _rebuildVisibleChildrenRequestedWhileDeferred;
        private Suspender _selectionUpdateSuspender;
        private ViewElement _previouslySelectedElementWhileSuspended;

        public IObservableCollection<ViewElement> Children { get; }

        protected IList<ViewElement> SelectionOrder { get; }

        #region P:SelectedElement:ViewElement
        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register("SelectedElement", typeof(ViewElement), typeof(ViewGroup), new PropertyMetadata(default(ViewElement), OnSelectedElementChanged));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewElement SelectedElement {
            get { return (ViewElement)GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
            }

        private static void OnSelectedElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (ViewGroup)sender;
            var newValue = e.NewValue as ViewElement;
            var oldValue = e.OldValue as ViewElement;
            if (newValue != null && !source.Children.Contains(newValue))
                throw new InvalidOperationException("SelectedElement must be a child of the group");
            source.UpdateSelectionOrder();
            if (oldValue != null) { oldValue.IsSelected = false; }
            if (newValue != null) { newValue.IsSelected = true; }
            source.OnSelectedElementChanged();
            source.SelectedElementChanged.RaiseEvent(source);
            }
        #endregion
        #region P:IsActive:Boolean
        private static readonly DependencyPropertyKey IsActivePropertyKey = DependencyProperty.RegisterReadOnly("IsActive", typeof(Boolean), typeof(ViewGroup), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;
        public Boolean IsActive {
            get { return (Boolean)GetValue(IsActiveProperty); }
            private set { SetValue(IsActivePropertyKey, value); }
            }
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal ViewElement PreviousSelectedElement {
            get
                {
                if (SelectionOrder.Count > 1)
                    return SelectionOrder[1];
                return null;
                }
            }

        #region P:OnScreenViewCardinality:OnScreenViewCardinality
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private static readonly DependencyPropertyKey OnScreenViewCardinalityPropertyKey = DependencyProperty.RegisterReadOnly("OnScreenViewCardinality", typeof(OnScreenViewCardinality), typeof(ViewGroup), new FrameworkPropertyMetadata(OnScreenViewCardinality.Zero, OnOnScreenViewCardinalityChanged));
        public static readonly DependencyProperty OnScreenViewCardinalityProperty = OnScreenViewCardinalityPropertyKey.DependencyProperty;
        public OnScreenViewCardinality OnScreenViewCardinality
            {
            get { return (OnScreenViewCardinality)GetValue(OnScreenViewCardinalityProperty); }
            protected set { SetValue(OnScreenViewCardinalityPropertyKey, value); }
            }
        #endregion
        #region P:HasDocumentGroupContainer:Boolean
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private static readonly DependencyPropertyKey HasDocumentGroupContainerPropertyKey = DependencyProperty.RegisterReadOnly("HasDocumentGroupContainer", typeof(Boolean), typeof(ViewGroup), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnHasDocumentGroupContainerChanged));
        public static readonly DependencyProperty HasDocumentGroupContainerProperty = HasDocumentGroupContainerPropertyKey.DependencyProperty;
        public Boolean HasDocumentGroupContainer
            {
            get { return (Boolean)GetValue(HasDocumentGroupContainerProperty); }
            protected set { SetValue(HasDocumentGroupContainerPropertyKey, Boxes.Box(value)); }
            }
        #endregion
        #region P:HasAutohiddenViews:Boolean
        private static readonly DependencyPropertyKey HasAutohiddenViewsPropertyKey = DependencyProperty.RegisterReadOnly("HasAutohiddenViews", typeof(Boolean), typeof(ViewGroup), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty HasAutohiddenViewsProperty = HasAutohiddenViewsPropertyKey.DependencyProperty;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean HasAutohiddenViews
            {
            get { return (Boolean)GetValue(HasAutohiddenViewsProperty); }
            protected set { SetValue(HasAutohiddenViewsPropertyKey, Boxes.Box(value)); }
            }
        #endregion
        #region P:IsDockingEnabled:Boolean
        private static readonly DependencyPropertyKey IsDockingEnabledPropertyKey = DependencyProperty.RegisterReadOnly("IsDockingEnabled", typeof(Boolean), typeof(ViewGroup), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsDockingEnabledChanged));
        public static readonly DependencyProperty IsDockingEnabledProperty = IsDockingEnabledPropertyKey.DependencyProperty;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean IsDockingEnabled
            {
            get { return (Boolean)GetValue(IsDockingEnabledProperty); }
            private set { SetValue(IsDockingEnabledPropertyKey, Boxes.Box(value)); }
            }
        #endregion

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IObservableCollection<ViewElement> VisibleChildren {
            get
                {
                if (visibleChildren == null)
                    InitializeVisibleChildren();
                return readOnlyVisibleChildren;
                }
            }

        private Int32 DeferRebuildVisibleChildrenCount {
            get
                {
                return _deferRebuildVisibleChildrenCount;
                }
            set
                {
                if (_deferRebuildVisibleChildrenCount == value)
                    return;
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _deferRebuildVisibleChildrenCount = value;
                if (_deferRebuildVisibleChildrenCount != 0 || !_rebuildVisibleChildrenRequestedWhileDeferred)
                    return;
                _rebuildVisibleChildrenRequestedWhileDeferred = false;
                RebuildVisibleChildren();
                }
            }

        private Suspender SelectionUpdateSuspender {
            get
                {
                if (_selectionUpdateSuspender == null)
                    _selectionUpdateSuspender = new Suspender(DoDeferredSelectionUpdate);
                return _selectionUpdateSuspender;
                }
            }

        public event EventHandler SelectedElementChanged;

        public event EventHandler HasDocumentGroupContainerChanged;

        public event EventHandler OnScreenViewCardinalityChanged;

        protected ViewGroup() {
            Children = new ViewElementCollection(this);
            SelectionOrder = new List<ViewElement>();
            Children.CollectionChanged += OnCollectionChanged;
            IsVisible = true;
            }

        public override ICustomXmlSerializer CreateSerializer() {
            return new ViewGroupCustomSerializer(this);
            }

        public void BeginInit() {
            Children.CollectionChanged -= OnCollectionChanged;
            }

        public void EndInit() {
            Children.CollectionChanged += OnCollectionChanged;
            OnCollectionChanged(Children, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

        private void UpdateSelectionOrder() {
            var index = SelectionOrder.IndexOf(SelectedElement);
            if (index <= 0)
                return;
            SelectionOrder.RemoveAt(index);
            SelectionOrder.Insert(0, SelectedElement);
            }

        public Int32 ChildIndexFromVisibleChildIndex(Int32 visiblePosition) {
            var num = -1;
            Int32 index;
            for (index = 0; index < Children.Count; ++index) {
                if (Children[index].IsVisible)
                    ++num;
                if (num == visiblePosition)
                    break;
                }
            return index;
            }

        private Boolean IsOwnedVisibleChild(ViewElement element) {
            if (Children.Contains(element))
                return element.IsVisible;
            return false;
            }

        private void InitializeVisibleChildren() {
            visibleChildren = new ObservableCollection<ViewElement>();
            readOnlyVisibleChildren = new ImmutableObservableCollection<ViewElement>(visibleChildren);
            RebuildVisibleChildren();
            }

        public IDisposable DeferRebuildVisibleChildren() {
            return new DeferRebuildVisibleChildrenScope(this);
            }

        protected void RebuildVisibleChildren()
            {
            if (DeferRebuildVisibleChildrenCount > 0) {_rebuildVisibleChildrenRequestedWhileDeferred = true; }
            else if (visibleChildren == null)         { InitializeVisibleChildren(); }
            else {
                var activeView = ViewManager.Instance.ActiveView;
                using (ViewManager.DeferActiveViewChanges()) {
                    var intList = new List<Int32>(visibleChildren.Count);
                    var i = 0;
                    var j = 0;
                    while (j < Children.Count) {
                        var flag1 = true;
                        var flag2 = false;
                        try {
                            var child = Children[j];
                            if (child.IsVisible) {
                                flag2 = true;
                                if (i == visibleChildren.Count)
                                    visibleChildren.Add(child);
                                else if (visibleChildren[i] != child) {
                                    if (!IsOwnedVisibleChild(visibleChildren[i])) {
                                        flag1 = false;
                                        intList.Add(i);
                                        }
                                    else {
                                        var oldIndex = visibleChildren.IndexOf(child);
                                        if (oldIndex < 0)
                                            visibleChildren.Insert(i, child);
                                        else
                                            visibleChildren.Move(oldIndex, i);
                                        }
                                    }
                                }
                            }
                        finally
                            {
                            if (flag1) { ++j; }
                            if (flag2) { ++i; }
                            }
                        }
                    while (i < visibleChildren.Count) { intList.Add(i++); }
                    intList.Reverse();
                    foreach (var index3 in intList) { visibleChildren.RemoveAt(index3); }
                    ViewManager.Instance.SetActiveView(activeView, ActivationType.Default);
                    }
                UpdateChildrenDependentProperties();
                }
            }

        [Conditional("DEBUG")]
        private void ValidateVisibleChildren() {
            }

        [Conditional("DEBUG")]
        private void EnsureVisibleChildrenCollectionHasNoInvalidEntries() {
            using (var enumerator = visibleChildren.GetEnumerator()) {
                do
                    ;
                while (enumerator.MoveNext() && IsOwnedVisibleChild(enumerator.Current));
                }
            }

        [Conditional("DEBUG")]
        private void EnsureAllVisibleElementsOfChildrenCollectionAreInVisibleChildrenInSameOrder() {
            var num1 = -1;
            foreach (var viewElement in Children.Where(child => child.IsVisible)) {
                var num2 = visibleChildren.IndexOf(viewElement);
                if (num2 < 0 || num2 < num1)
                    break;
                num1 = num2;
                }
            }

        public virtual Boolean IsChildOnScreen(Int32 childIndex) {
            if (childIndex < 0 || childIndex >= Children.Count)
                throw new ArgumentOutOfRangeException(nameof(childIndex));
            if (IsOnScreen)
                return Children[childIndex].IsVisible;
            return false;
            }

        private void UpdateChildrenDependentProperties() {
            UpdateOnScreenViewCardinality();
            UpdateHasDocumentGroupContainer();
            UpdateIsDockingEnabled();
            UpdateHasAutohiddenViews();
            }

        protected virtual void UpdateOnScreenViewCardinality() {
            var screenViewCardinality = OnScreenViewCardinality.Zero;
            foreach (var visibleChild in VisibleChildren) {
                var viewGroup = visibleChild as ViewGroup;
                if (viewGroup != null) {
                    if (viewGroup.OnScreenViewCardinality == OnScreenViewCardinality.Many) {
                        screenViewCardinality = OnScreenViewCardinality.Many;
                        break;
                        }
                    if (viewGroup.OnScreenViewCardinality == OnScreenViewCardinality.One)
                        screenViewCardinality = OnScreenViewCardinality.One;
                    }
                else if (visibleChild is View)
                    screenViewCardinality = OnScreenViewCardinality.One;
                }
            OnScreenViewCardinality = screenViewCardinality;
            }

        protected virtual void UpdateHasDocumentGroupContainer() {
            HasDocumentGroupContainer = Find<DocumentGroupContainer>(false) != null;
            }

        protected virtual void UpdateHasAutohiddenViews() {
            if (Parent == null)
                return;
            Parent.UpdateHasAutohiddenViews();
            }

        protected virtual void UpdateIsDockingEnabled() {
            var container = Find<DocumentGroupContainer>(false);
            if (container != null && Find<View>(e =>
            {
                if (e.IsVisible)
                    return container.Find(f => Equals(f, e), false) == null;
                return false;
            }, false) != null)
                IsDockingEnabled = false;
            else
                IsDockingEnabled = true;
            }

        public virtual Boolean CanHostPinnedViews() {
            return false;
            }

        private static void OnOnScreenViewCardinalityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewGroup = (ViewGroup)obj;
            if (viewGroup.Parent != null)
                viewGroup.Parent.UpdateOnScreenViewCardinality();
            viewGroup.OnOnScreenViewCardinalityChanged(args);
            }

        protected virtual void OnOnScreenViewCardinalityChanged(DependencyPropertyChangedEventArgs args) {
            // ISSUE: reference to a compiler-generated field
            OnScreenViewCardinalityChanged.RaiseEvent(this);
            }

        private static void OnHasDocumentGroupContainerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewGroup = (ViewGroup)obj;
            if (viewGroup.Parent != null)
                viewGroup.Parent.UpdateHasDocumentGroupContainer();
            // ISSUE: reference to a compiler-generated field
            viewGroup.HasDocumentGroupContainerChanged.RaiseEvent(viewGroup);
            }

        private static void OnIsDockingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewGroup = (ViewGroup)obj;
            if (viewGroup.Parent == null)
                return;
            viewGroup.Parent.UpdateIsDockingEnabled();
            }

        private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            OnChildVisibilityChanged(null);
            OnChildrenChanged(e);
            }

        protected internal virtual void OnChildrenChanged(NotifyCollectionChangedEventArgs e) {
            if (!FloatSite.IsFloating(this)) { return; }
            var viewElementList = new List<ViewElement>();
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null) {
                        var enumerator = e.NewItems.GetEnumerator();
                        try {
                            while (enumerator.MoveNext()) {
                                var current = (ViewElement)enumerator.Current;
                                viewElementList.AddRange(current.FindAll(ve => true));
                                }
                            break;
                            }
                        finally
                            {
                            var disposable = enumerator as IDisposable;
                            if (disposable != null)
                                disposable.Dispose();
                            }
                        }
                    else
                        break;
                case NotifyCollectionChangedAction.Reset:
                    using (var enumerator = Children.GetEnumerator()) {
                        while (enumerator.MoveNext()) {
                            var current = enumerator.Current;
                            viewElementList.AddRange(current.FindAll(ve => true));
                            }
                        break;
                        }
                }
            foreach (var viewElement in viewElementList) {
                viewElement.FloatingWidth = FloatingWidth;
                viewElement.FloatingHeight = FloatingHeight;
                }
            }

        protected internal override void OnAncestorChanged() {
            base.OnAncestorChanged();
            foreach (var viewElement in new List<ViewElement>(Children))
                viewElement.OnAncestorChanged();
            }

        protected virtual void OnSelectedElementChanged() {
            }

        protected void UpdateSelection(Int32 previousSelectionIndex) {
            if (SelectionUpdateSuspender.IsSuspended) {
                if (previousSelectionIndex < 0 || previousSelectionIndex >= Children.Count)
                    return;
                _previouslySelectedElementWhileSuspended = Children[previousSelectionIndex];
                }
            else
                UpdateSelectionCore(previousSelectionIndex);
            }

        protected virtual void CheckSelectedElementSet(ViewElement suggestedItemToSelect) {
            Validate.IsNotNull(suggestedItemToSelect, "suggestedItemToSelect");
            }

        protected virtual void UpdateSelectionCore(Int32 previousSelectionIndex) {
            }

        protected internal void OnChildVisibilityChanged(ViewElement item = null) {
            var selectedElement = SelectedElement;
            if (selectedElement != null && !selectedElement.IsVisible)
                UpdateSelection(Children.IndexOf(selectedElement));
            else if (item != null)
                CheckSelectedElementSet(item);
            OnChildVisibilityChangedCore();
            }

        protected internal virtual void OnChildVisibilityChangedCore() {
            if (!IsCollapsible) {
                childVisiblityChangedWhileNotCollapsible = true;
                }
            else {
                IsVisible = Children.Any(child => child.IsVisible);
                RebuildVisibleChildren();
                }
            }

        protected override void OnIsCollapsibleChanged() {
            base.OnIsCollapsibleChanged();
            if (!IsCollapsible || !childVisiblityChangedWhileNotCollapsible)
                return;
            childVisiblityChangedWhileNotCollapsible = false;
            OnChildVisibilityChanged(null);
            }

        protected internal virtual void OnChildPinnedStatusChanged(ViewElement element) {
            }

        public virtual Boolean CanMoveTab(Int32 oldIndex, Int32 newIndex) {
            return true;
            }

        public abstract Boolean IsChildAllowed(ViewElement e);

        protected override void OnIsVisibleChanged() {
            base.OnIsVisibleChanged();
            if (IsVisible)
                return;
            foreach (var child in Children)
                child.IsVisible = false;
            }

        protected internal void AbsorbElementWithOnlyOneChild(ViewGroup childGroup) {
            Validate.IsNotNull(childGroup, "childGroup");
            if (!Equals(childGroup.Parent, this))
                throw new ArgumentException("childGroup must be a child of this group");
            if (childGroup.Children.Count != 1)
                throw new ArgumentException("childGroup must have exactly one child");
            var child = childGroup.Children[0];
            Children[Children.IndexOf(childGroup)] = child;
            child.TryCollapse();
            }

        public override ViewElement Find(Predicate<ViewElement> predicate, Boolean preferRootFirst = false) {
            if (predicate == null)
                return null;
            if (preferRootFirst)
                return FindRootFirst(predicate);
            return FindLeafFirst(predicate);
            }

        private ViewElement FindLeafFirst(Predicate<ViewElement> predicate) {
            foreach (var child in Children) {
                var viewElement = child.Find(predicate, false);
                if (viewElement != null)
                    return viewElement;
                }
            return base.Find(predicate, false);
            }

        private ViewElement FindRootFirst(Predicate<ViewElement> predicate) {
            var viewElement = base.Find(predicate, true);
            if (viewElement == null) {
                foreach (var child in Children) {
                    viewElement = child.Find(predicate, true);
                    if (viewElement != null)
                        break;
                    }
                }
            return viewElement;
            }

        public override IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate) {
            if (predicate != null) {
                foreach (var child in Children) {
                    foreach (var viewElement in child.FindAll(predicate))
                        yield return viewElement;
                    }
                foreach (var viewElement in BaseFindAll(predicate))
                    yield return viewElement;
                }
            }

        private IEnumerable<ViewElement> BaseFindAll(Predicate<ViewElement> predicate) {
            return base.FindAll(predicate);
            }

        #region M:IEnumerable<ViewElement>.GetEnumerator:IEnumerator<ViewElement>
        IEnumerator<ViewElement> IEnumerable<ViewElement>.GetEnumerator() {
            return Children.GetEnumerator();
            }
        #endregion
        #region M:IEnumerable.GetEnumerator:IEnumerator
        IEnumerator IEnumerable.GetEnumerator()
            {
            return Children.GetEnumerator();
            }
        #endregion

        public override String ToString() {
            return String.Format("{0}, Children = {1}, VisibleChildren = {2}, DockedWidth = {3}, DockedHeight = {4}", (Object)GetType().Name, (Object)Children.Count, (Object)VisibleChildren.Count, (Object)DockedWidth, (Object)DockedHeight);
            }

        private void DoDeferredSelectionUpdate() {
            if (_previouslySelectedElementWhileSuspended == null)
                return;
            var previousSelectionIndex = Children.IndexOf(_previouslySelectedElementWhileSuspended);
            _previouslySelectedElementWhileSuspended = null;
            if (previousSelectionIndex < 0)
                return;
            UpdateSelection(previousSelectionIndex);
            }

        public IDisposable SuspendSelectionUpdates() {
            return SelectionUpdateSuspender.Suspend();
            }

        private class ViewElementCollection : OwnershipCollection<ViewElement>
            {
            public ViewGroup Owner { get; }

            public ViewElementCollection(ViewGroup owner) {
                Owner = owner;
                }

            private void AddSelection(ViewElement item) {
                Owner.SelectionOrder.Add(item);
                }

            private void SetSelection(Int32 index, ViewElement item) {
                Owner.SelectionOrder[Owner.SelectionOrder.IndexOf(this[index])] = item;
                if (!this[index].IsSelected)
                    return;
                if (item.IsVisible)
                    item.IsSelected = true;
                else
                    Owner.UpdateSelection(index);
                }

            private void RemoveSelection(Int32 index) {
                Owner.SelectionOrder.Remove(this[index]);
                if (!this[index].IsSelected)
                    return;
                Owner.UpdateSelection(index);
                }

            private void ClearSelection() {
                Owner.SelectionOrder.Clear();
                Owner.SelectedElement = null;
                }

            protected override void InsertItem(Int32 index, ViewElement item) {
                if (!Owner.IsChildAllowed(item))
                    throw new InvalidOperationException("Invalid element type added to ViewGroup");
                AddSelection(item);
                base.InsertItem(index, item);
                if (item.IsSelected)
                    Owner.SelectedElement = item;
                else
                    Owner.CheckSelectedElementSet(item);
                }

            protected override void SetItem(Int32 index, ViewElement item) {
                if (!Owner.IsChildAllowed(item)) { throw new InvalidOperationException("Invalid element type added to ViewGroup"); }
                SetSelection(index, item);
                base.SetItem(index, item);
                }

            protected override void RemoveItem(Int32 index) {
                RemoveSelection(index);
                base.RemoveItem(index);
                if (Owner == null) { return; }
                Owner.TryCollapse();
                }

            protected override void ClearItems() {
                ClearSelection();
                base.ClearItems();
                if (Owner == null) { return; }
                Owner.TryCollapse();
                }

            #region M:LoseOwnership(ViewElement)
            protected override void LoseOwnership(ViewElement element)
                {
                ViewManager.ClearViewManager(element);
                element.Parent = null;
                }
            #endregion
            #region M:TakeOwnership(ViewElement)
            protected override void TakeOwnership(ViewElement element) {
                element.Detach();
                element.Parent = Owner;
                ViewManager.BindViewManager(element, Owner);
                }
            #endregion

            protected override void OnMaximumItemsExceeded(ViewElement item) {
                }
            }

        private class DeferRebuildVisibleChildrenScope : DisposableObject
            {
            private ViewGroup _group;

            public DeferRebuildVisibleChildrenScope(ViewGroup group) {
                _group = group;
                ++_group.DeferRebuildVisibleChildrenCount;
                }

            protected override void DisposeManagedResources() {
                --_group.DeferRebuildVisibleChildrenCount;
                }
            }

        #region M:OnChildActivityChanged
        internal void OnChildActivityChanged() {
            IsActive = Children.OfType<View>().Any(i => i.IsActive);
            }
        #endregion
        #region M:Add(ViewElement)
        public void Add(ViewElement item) {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            Children.Add(item);
            }
        #endregion
        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal override void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (!e.Handled) {
                foreach (var child in Children) {
                    child.OnCanExecuteCommand(sender, e);
                    if (e.Handled) { return; }
                    }
                }
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal override void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                foreach (var child in Children) {
                    child.OnExecutedCommand(sender, e);
                    if (e.Handled) { return; }
                    }
                }
            }
        #endregion
        }
    }
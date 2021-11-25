using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Serialization;
using BinaryStudio.DataProcessing.Annotations;

namespace BinaryStudio.PlatformUI.Shell
    {
    [XamlSerializable]
    [ContentProperty("Content")]
    [DefaultProperty("Content")]
    public abstract class ViewElement : DependencyObject, ICustomXmlSerializable, IDependencyObjectCustomSerializerAccess, INotifyPropertyChanged
        {
        internal const Double UnfloatedPosition = Double.NaN;
        private const Double AutoHideWidthDefaultValue = 300.0;
        private const Double AutoHideHeightDefaultValue = 300.0;
        private const Double FloatingWidthDefaultValue = 300.0;
        private const Double FloatingHeightDefaultValue = 200.0;
        private const Double FloatingLeftDefaultValue = Double.NaN;
        private const Double FloatingTopDefaultValue = Double.NaN;
        internal const Double MinimumWidthDefaultValue = 30.0;
        internal const Double MinimumHeightDefaultValue = 30.0;
        private const WindowState FloatingWindowStateDefaultValue = WindowState.Normal;
        private const DockRestrictionType DockRestrictionDefaultValue = DockRestrictionType.None;
        protected internal static DependencyPropertyKey WindowProfilePropertyKey;
        public static readonly DependencyProperty WindowProfileProperty;
        private Int32 preventCollapseReferences;
        private Boolean pendingVisibility;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual WindowProfileSerializationVariants SerializationVariants {
            get
                {
                return WindowProfileSerializationVariants.Default;
                }
            }

        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(ViewElement), new PropertyMetadata(default(Boolean), OnIsSelectedChanged));
        [DefaultValue(false)]
        public Boolean IsSelected {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((ViewElement)sender).OnIsSelectedChanged();
            }

        protected virtual void OnIsSelectedChanged() {
            IsSelectedChanged.RaiseEvent(this);
            UpdateParentSelectedElement();
            }
        #endregion
        #region P:IsVisible:Boolean
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(Boolean), typeof(ViewElement), new PropertyMetadata(Boxes.BooleanFalse, OnIsVisibleChanged, CoerceIsVisible));
        [DefaultValue(false)]
        public Boolean IsVisible
            {
            get { return (Boolean)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:Parent:ViewGroup
        private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterReadOnly("Parent", typeof(ViewGroup), typeof(ViewElement), new PropertyMetadata(null, OnParentChanged));
        public static readonly DependencyProperty ParentProperty = ParentPropertyKey.DependencyProperty;
        private static void OnParentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((ViewElement)sender).OnParentChanged((ViewGroup)e.OldValue);
            }
        protected virtual void OnParentChanged(ViewGroup o) {
            ParentChanged.RaiseEvent(this);
            OnAncestorChanged();
            if ((o != null) && Equals(o.SelectedElement, this)) { o.SelectedElement = null; }
            UpdateParentSelectedElement();
            }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewGroup Parent
            {
            get { return (ViewGroup)GetValue(ParentProperty); }
            internal set { SetValue(ParentPropertyKey, value); }
            }
        #endregion
        #region P:WindowProfile:WindowProfile
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WindowProfile WindowProfile {
            get
                {
                return WindowProfile.FindWindowProfile(this);
                }
            }
        #endregion
        #region P:DockedHeight:SplitterLength
        public static readonly DependencyProperty DockedHeightProperty = DependencyProperty.Register("DockedHeight", typeof(SplitterLength), typeof(ViewElement), new FrameworkPropertyMetadata(new SplitterLength(200.0, (SplitterUnitType)1), OnDockedHeightChanged));
        [DefaultValue(typeof(SplitterLength), "200")]
        public SplitterLength DockedHeight
            {
            get { return (SplitterLength)GetValue(DockedHeightProperty); }
            set { SetValue(DockedHeightProperty, value); }
            }
        #endregion
        #region P:DockedWidth:SplitterLength
        public static readonly DependencyProperty DockedWidthProperty = DependencyProperty.Register("DockedWidth", typeof(SplitterLength), typeof(ViewElement), new FrameworkPropertyMetadata(new SplitterLength(200.0, (SplitterUnitType)1), OnDockedWidthChanged));
        [DefaultValue(typeof(SplitterLength), "200")]
        public SplitterLength DockedWidth
            {
            get { return (SplitterLength)GetValue(DockedWidthProperty); }
            set { SetValue(DockedWidthProperty, value); }
            }
        #endregion
        #region P:AutoHideWidth:Double
        public static readonly DependencyProperty AutoHideWidthProperty = DependencyProperty.Register("AutoHideWidth", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(AutoHideWidthDefaultValue));
        [DefaultValue(AutoHideWidthDefaultValue)]
        public Double AutoHideWidth
            {
            get { return (Double)GetValue(AutoHideWidthProperty); }
            set { SetValue(AutoHideWidthProperty, value); }
            }
        #endregion
        #region P:AutoHideHeight:Double
        public static readonly DependencyProperty AutoHideHeightProperty = DependencyProperty.Register("AutoHideHeight", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(AutoHideHeightDefaultValue));
        [DefaultValue(AutoHideHeightDefaultValue)]
        public Double AutoHideHeight
            {
            get { return (Double)GetValue(AutoHideHeightProperty); }
            set { SetValue(AutoHideHeightProperty, value); }
            }
        #endregion
        #region P:Display:Int32
        public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register("Display", typeof(Int32), typeof(ViewElement), new FrameworkPropertyMetadata(Boxes.Int32Zero, OnDisplayChanged, CoerceDisplay));
        public Int32 Display
            {
            get { return (Int32)GetValue(DisplayProperty); }
            set { SetValue(DisplayProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:FloatingTop:Double
        public static readonly DependencyProperty FloatingTopProperty = DependencyProperty.Register("FloatingTop", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(FloatingTopDefaultValue, OnFloatingPositionChanged));
        [DefaultValue(FloatingTopDefaultValue)]
        public Double FloatingTop
            {
            get { return (Double)GetValue(FloatingTopProperty); }
            set { SetValue(FloatingTopProperty, value); }
            }
        #endregion
        #region P:FloatingLeft:Double
        public static readonly DependencyProperty FloatingLeftProperty = DependencyProperty.Register("FloatingLeft", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(FloatingLeftDefaultValue, OnFloatingPositionChanged));
        [DefaultValue(FloatingLeftDefaultValue)]
        public Double FloatingLeft
            {
            get { return (Double)GetValue(FloatingLeftProperty); }
            set { SetValue(FloatingLeftProperty, value); }
            }
        #endregion
        #region P:FloatingHeight:Double
        public static readonly DependencyProperty FloatingHeightProperty = DependencyProperty.Register("FloatingHeight", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(FloatingHeightDefaultValue, OnFloatingSizeChanged));
        [DefaultValue(FloatingHeightDefaultValue)]
        public Double FloatingHeight
            {
            get { return (Double)GetValue(FloatingHeightProperty); }
            set { SetValue(FloatingHeightProperty, value); }
            }
        #endregion
        #region P:FloatingWidth:Double
        public static readonly DependencyProperty FloatingWidthProperty = DependencyProperty.Register("FloatingWidth", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(FloatingWidthDefaultValue, OnFloatingSizeChanged));
        [DefaultValue(FloatingWidthDefaultValue)]
        public Double FloatingWidth
            {
            get { return (Double)GetValue(FloatingWidthProperty); }
            set { SetValue(FloatingWidthProperty, value); }
            }
        #endregion
        #region P:FloatingWindowState:WindowState
        public static readonly DependencyProperty FloatingWindowStateProperty;
        [DefaultValue(FloatingWindowStateDefaultValue)]
        public WindowState FloatingWindowState
            {
            get { return (WindowState)GetValue(FloatingWindowStateProperty); }
            set { SetValue(FloatingWindowStateProperty, value); }
            }
        #endregion
        #region P:DockRestriction:DockRestrictionType
        public static readonly DependencyProperty DockRestrictionProperty = DependencyProperty.Register("DockRestriction", typeof(DockRestrictionType), typeof(ViewElement), new PropertyMetadata(DockRestrictionDefaultValue));
        [DefaultValue(DockRestrictionDefaultValue)]
        public DockRestrictionType DockRestriction
            {
            get { return (DockRestrictionType)GetValue(DockRestrictionProperty); }
            set { SetValue(DockRestrictionProperty, value); }
            }
        #endregion
        #region P:AreDockTargetsEnabled:Boolean
        public static readonly DependencyProperty AreDockTargetsEnabledProperty = DependencyProperty.Register("AreDockTargetsEnabled", typeof(Boolean), typeof(ViewElement), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        [DefaultValue(true)]
        public Boolean AreDockTargetsEnabled
            {
            get { return (Boolean)GetValue(AreDockTargetsEnabledProperty); }
            set { SetValue(AreDockTargetsEnabledProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:MinimumWidth:Double
        public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register("MinimumWidth", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(30.0));
        [DefaultValue(30.0)]
        public Double MinimumWidth
            {
            get { return (Double)GetValue(MinimumWidthProperty); }
            set { SetValue(MinimumWidthProperty, value); }
            }
        #endregion
        #region P:MinimumHeight:Double
        public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register("MinimumHeight", typeof(Double), typeof(ViewElement), new FrameworkPropertyMetadata(30.0));
        [DefaultValue(30.0)]
        public Double MinimumHeight
            {
            get { return (Double)GetValue(MinimumHeightProperty); }
            set { SetValue(MinimumHeightProperty, value); }
            }
        #endregion
        #region P:IsDragEnabled:Boolean
        public static readonly DependencyProperty IsDragEnabledProperty = DependencyProperty.Register("IsDragEnabled", typeof(Boolean), typeof(ViewElement), new PropertyMetadata(true));
        public Boolean IsDragEnabled
            {
            get { return (Boolean)GetValue(IsDragEnabledProperty); }
            set { SetValue(IsDragEnabledProperty, value); }
            }
        #endregion
        #region P:DataContext:Object
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(Object), typeof(ViewElement), new PropertyMetadata(default(Object), OnDataContextChanged));
        private static void OnDataContextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ViewElement);
            if (source != null) {
                source.OnDataContextChanged();
                }
            }

        protected virtual void OnDataContextChanged()
            {
            }

        public Object DataContext {
            get { return (Object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
            }
        #endregion
        #region P:ToolTip:Object
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(Object), typeof(ViewElement), new PropertyMetadata(default(Object)));
        public Object ToolTip
            {
            get { return (Object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
            }
        #endregion
        #region P:ViewManager:ViewManager
        public ViewManager ViewManager { get {
            var r = WindowProfile;
            return (r != null)
                ? r.ViewManager
                : null;
            }}
        #endregion

        public Boolean IsCollapsible {
            get
                {
                return preventCollapseReferences == 0;
                }
            }

        public Boolean IsOnScreen {
            get
                {
                return GetIsOnScreenCore();
                }
            }

        public event EventHandler IsVisibleChanged;
        public event EventHandler IsSelectedChanged;
        public event EventHandler ParentChanged;
        public event EventHandler FloatingPositionChanged;
        public event EventHandler FloatingSizeChanged;

        static ViewElement()
            {
            var propertyMetadata = new FrameworkPropertyMetadata(FloatingWindowStateDefaultValue);
            CoerceValueCallback coerceValueCallback = OnCoerceFloatingWindowState;
            propertyMetadata.CoerceValueCallback = coerceValueCallback;
            FloatingWindowStateProperty = DependencyProperty.Register("FloatingWindowState", typeof(WindowState), typeof(ViewElement), propertyMetadata);
            WindowProfilePropertyKey = DependencyProperty.RegisterReadOnly("WindowProfile", typeof(WindowProfile), typeof(ViewElement), new PropertyMetadata(null));
            WindowProfileProperty = WindowProfilePropertyKey.DependencyProperty;
            }

        protected ViewElement() {
            //if (!ViewElementFactory.Current.IsConstructionAllowed)
            //  throw new InvalidOperationException("ViewElements cannot be constructed except through factory methods.  Please use the static Create method on the ViewElement, or the ViewElementFactory directly.");
            }

        public virtual ICustomXmlSerializer CreateSerializer() {
            return new ViewElementCustomSerializer(this);
            }

        public virtual Type GetSerializedType() {
            return GetType();
            }

        Boolean IDependencyObjectCustomSerializerAccess.ShouldSerializeProperty(DependencyProperty dp) {
            return ShouldSerializeProperty(dp);
            }

        Object IDependencyObjectCustomSerializerAccess.GetValue(DependencyProperty dp) {
            return GetValue(dp);
            }

        private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            ((ViewElement)obj).OnIsVisibleChanged();
            }

        private static Object CoerceIsVisible(DependencyObject d, Object baseValue) {
            var viewElement = (ViewElement)d;
            if (viewElement.IsCollapsible)
                return baseValue;
            viewElement.pendingVisibility = (Boolean)baseValue;
            return Boxes.Box(viewElement.IsVisible);
            }

        protected virtual void OnIsVisibleChanged() {
            if (Parent != null)
                Parent.OnChildVisibilityChanged(this);
            // ISSUE: reference to a compiler-generated field
            IsVisibleChanged.RaiseEvent(this);
            }

        protected internal virtual void OnAncestorChanged() {
            }

        private void UpdateParentSelectedElement() {
            if (Parent == null)
                return;
            if (IsSelected) {
                Parent.SelectedElement = this;
                }
            else {
                if (!Equals(Parent.SelectedElement, this))
                    return;
                Parent.SelectedElement = null;
                }
            }

        public void TryCollapse() {
            if (!IsCollapsible)
                return;
            TryCollapseCore();
            }

        protected virtual void TryCollapseCore() {
            }

        private void AddRefCollapseScope() {
            var num = preventCollapseReferences + 1;
            preventCollapseReferences = num;
            if (num != 1)
                return;
            OnIsCollapsibleChanged();
            }

        private void ReleaseCollapseScope() {
            var num = preventCollapseReferences - 1;
            preventCollapseReferences = num;
            if (num != 0)
                return;
            OnIsCollapsibleChanged();
            }

        protected virtual void OnIsCollapsibleChanged() {
            if (IsCollapsible) {
                IsVisible = pendingVisibility;
                TryCollapse();
                }
            else
                pendingVisibility = IsVisible;
            }

        public IDisposable PreventCollapse() {
            return new PreventCollapseScope(this);
            }

        public virtual ViewElement Find(Predicate<ViewElement> predicate, Boolean preferRootFirst = false) {
            if (predicate == null) { return null; }
            if (predicate(this)) { return this; }
            return null;
            }

        public T Find<T>(Boolean preferRootFirst = false) where T : ViewElement {
            var num = preferRootFirst ? 1 : 0;
            return (T)Find(element => element is T, num != 0);
            }

        public T Find<T>(Predicate<T> predicate, Boolean preferRootFirst = false) where T : ViewElement {
            return (T)Find(element =>
            {
                if (element is T)
                    return predicate((T)element);
                return false;
            }, preferRootFirst);
            }

        public virtual IEnumerable<ViewElement> FindAll(Predicate<ViewElement> predicate) {
            if (predicate != null && predicate(this))
                yield return this;
            }

        public IEnumerable<T> FindAll<T>() where T : ViewElement {
            return FindAll(element => element is T).Cast<T>();
            }

        public IEnumerable<T> FindAll<T>(Predicate<T> predicate) where T : ViewElement {
            return FindAll<T>().Where(t => predicate(t));
            }

        public void Detach() {
            if (Parent == null) { return; }
            Parent.Children.Remove(this);
            }

        protected virtual void OnDockedWidthChanged() {
            }

        protected virtual void OnDockedHeightChanged() {
            }

        protected virtual void ValidateDockedWidth(SplitterLength width) {
            }

        protected virtual Boolean GetIsOnScreenCore() {
            if (Parent != null)
                return Parent.IsChildOnScreen(Parent.Children.IndexOf(this));
            return false;
            }

        protected virtual void ValidateDockedHeight(SplitterLength height) {
            }

        public virtual ViewElement GetFloatingStructure(ViewGroup oldParent) {
            return this;
            }

        private static void OnDockedWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewElement = (ViewElement)obj;
            viewElement.ValidateDockedWidth((SplitterLength)args.NewValue);
            viewElement.OnDockedWidthChanged();
            }

        private static void OnDockedHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewElement = (ViewElement)obj;
            viewElement.ValidateDockedHeight((SplitterLength)args.NewValue);
            viewElement.OnDockedHeightChanged();
            }

        private static Object CoerceDisplay(DependencyObject d, Object baseValue) {
            if ((Int32)baseValue < 0)
                return Boxes.Int32Zero;
            if ((Int32)baseValue > Screen.DisplayCount - 1)
                return Boxes.Box(Screen.DisplayCount - 1);
            return baseValue;
            }

        private static void OnDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var viewElement = (ViewElement)d;
            // ISSUE: reference to a compiler-generated field
            viewElement.FloatingPositionChanged.RaiseEvent(viewElement);
            }

        private static void OnFloatingPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewElement = (ViewElement)obj;
            // ISSUE: reference to a compiler-generated field
            viewElement.FloatingPositionChanged.RaiseEvent(viewElement);
            }

        private static void OnFloatingSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var newValue = (Double)args.NewValue;
            if (newValue < 0.0)
                throw new ArgumentOutOfRangeException(nameof(args), newValue, "FloatingWidth and FloatingHeight must be non-negative.");
            var viewElement = (ViewElement)obj;
            // ISSUE: reference to a compiler-generated field
            viewElement.FloatingSizeChanged.RaiseEvent(viewElement);
            }

        private static Object OnCoerceFloatingWindowState(DependencyObject obj, Object baseValue) {
            var floatSite = obj as FloatSite;
            if (floatSite != null && floatSite.IsIndependent || obj is MainSite)
                return baseValue;
            var windowState = (WindowState)baseValue;
            var viewElement = (ViewElement)obj;
            if (windowState == WindowState.Minimized)
                windowState = viewElement.FloatingWindowState;
            return windowState;
            }

        public ViewElement FindRootElement() {
            var viewElement = this;
            while (viewElement.Parent != null)
                viewElement = viewElement.Parent;
            return viewElement;
            }

        public static ViewElement FindRootElement(ViewElement element) {
            Validate.IsNotNull(element, "element");
            return element.FindRootElement();
            }

        private class PreventCollapseScope : IDisposable
            {
            private ViewElement Element { get; }

            private Boolean IsDisposed { get; set; }

            public PreventCollapseScope(ViewElement element) {
                Element = element;
                Element.AddRefCollapseScope();
                }

            public void Dispose() {
                if (IsDisposed)
                    return;
                Element.ReleaseCollapseScope();
                IsDisposed = true;
                }
            }

        #region M:OnPropertyChanged(DependencyPropertyChangedEventArgs)
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }
        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        #endregion

        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal virtual void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = false;
            e.Handled = false;
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal virtual void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            e.Handled = false;
            }
        #endregion
        }
    }
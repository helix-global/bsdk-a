using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Serialization;

namespace BinaryStudio.PlatformUI.Shell {
    public class View<T> : View
        {
        public new T Content { get; }
        public View(T content) {
            Content = content;
            base.Content = Content;
            }
        }

    [XamlSerializable]
    [ContentProperty("Content")]
    [DefaultProperty("Content")]
    public class View : ViewElement
        {
        public View() {
            Name = Guid.NewGuid().ToString("N");
            Title   = Name;
            ToolTip = Name;
            IsVisible = true;
            }

        private static View invalidview;

        #region P:DocumentTabTitleTemplate:DataTemplate
        public static readonly DependencyProperty DocumentTabTitleTemplateProperty = DependencyProperty.RegisterAttached("DocumentTabTitleTemplate", typeof(DataTemplate), typeof(View), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate DocumentTabTitleTemplate {
            get { return (DataTemplate)GetValue(DocumentTabTitleTemplateProperty); }
            set { SetValue(DocumentTabTitleTemplateProperty, value); }
            }
        public static DataTemplate GetDocumentTabTitleTemplate(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (DataTemplate)source.GetValue(DocumentTabTitleTemplateProperty);
            }

        public static void SetDocumentTabTitleTemplate(DependencyObject source, DataTemplate value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(DocumentTabTitleTemplateProperty, value);
            }
        #endregion
        #region P:TabTitleTemplate:DataTemplate
        public static readonly DependencyProperty TabTitleTemplateProperty = DependencyProperty.RegisterAttached("TabTitleTemplate", typeof(DataTemplate), typeof(View), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate TabTitleTemplate {
            get { return (DataTemplate)GetValue(TabTitleTemplateProperty); }
            set { SetValue(TabTitleTemplateProperty, value); }
            }
        public static DataTemplate GetTabTitleTemplate(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (DataTemplate)source.GetValue(TabTitleTemplateProperty);
            }

        public static void SetTabTitleTemplate(DependencyObject source, DataTemplate value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(TabTitleTemplateProperty, value);
            }
        #endregion
        #region P:TitleTemplate:DataTemplate
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(View), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate TitleTemplate {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
            }
        public static DataTemplate GetTitleTemplate(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (DataTemplate)source.GetValue(TitleTemplateProperty);
            }

        public static void SetTitleTemplate(DependencyObject source, DataTemplate value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(TitleTemplateProperty, value);
            }
        #endregion
        #region P:Title:Object
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(Object), typeof(View), new PropertyMetadata(default(Object)));
        [DefaultValue(null)]
        [Localizable(true)]
        public Object Title {
            get { return (Object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
            }
        #endregion
        #region P:Name:String
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(String), typeof(View));
        public String Name {
            get { return (String)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
            }
        #endregion
        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(View), new FrameworkPropertyMetadata(null, OnContentChanged));
        private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as View);
            if (source != null) {
                source.OnContentChanged();
                }
            }

        protected virtual void OnContentChanged()
            {
            DataContext = Content;
            }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object Content {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        #region P:IsActive:Boolean
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(Boolean), typeof(View), new PropertyMetadata(default(Boolean),OnIsActiveChanged, OnIsActiveCoerceValue));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Boolean IsActive {
            get { return (Boolean)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
            }

        private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as View;
            if (source != null) {
                source.OnIsActiveChanged();
                }
            }

        protected void OnIsActiveChanged() {
            #if DEBUG
            Debug.Print("View[{0}].OnIsActiveChanged:{1}", Title, IsActive);
            #endif
            var parent = Parent;
            if (parent != null) { parent.OnChildActivityChanged(); }
            if (IsActive) {
                ViewManager.Instance.SetActiveView(this, ActivationType.Default);
                }
            }

        private static Object OnIsActiveCoerceValue(DependencyObject sender, Object value) {
            var flag = (Boolean)value;
            if (ViewManager.Instance.IsPendingActiveView & flag) {
                ViewManager.Instance.SetActiveView(sender as View, ActivationType.Default);
                flag = false;
                }
            return flag;
            }
        #endregion
        #region P:IsPinned:Boolean
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(Boolean), typeof(View), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsPinnedChanged, OnIsPinnedCoerceValue));
        public Boolean IsPinned {
            get { return (Boolean)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, Boxes.Box(value)); }
            }
        private static void OnIsPinnedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (View)sender;
            if (source.Parent != null) { source.Parent.OnChildPinnedStatusChanged(source); }
            source.RaisePinStatusChanged();
            }

        private static Object OnIsPinnedCoerceValue(DependencyObject sender, Object value) {
            var source = (View)sender;
            if (source.IsPinned != (Boolean)value && DockOperations.IsTogglePinStatusPrevented) { return Boxes.Box(source.IsPinned); }
            if (source.Parent != null && source.Parent.CanHostPinnedViews()) { return value; }
            return Boxes.BooleanFalse;
            }
        #endregion
        #region P:InvalidView:View
        internal static View InvalidView { get {
            if (invalidview == null) {
                invalidview = Create();
                invalidview.Name = "Invalid View";
                invalidview.Title = invalidview.Name;
                }
            return invalidview;
            }}
        #endregion
        #region P:Picture:Object
        public static readonly DependencyProperty PictureProperty = DependencyProperty.Register("Picture", typeof(Object), typeof(View), new PropertyMetadata(default(Object)));
        public Object Picture {
            get { return (Object)GetValue(PictureProperty); }
            set { SetValue(PictureProperty, value); }
            }
        #endregion

        public static event EventHandler<IsViewSelectedEventArgs> IsViewSelectedChanged;
        public event EventHandler Shown;
        public event CancelEventHandler Showing;
        public event EventHandler Hidden;
        public event CancelEventHandler Hiding;
        public event EventHandler<DockEventArgs> DockPositionChanging;
        public event EventHandler<DockEventArgs> DockPositionChanged;
        public event EventHandler PinStatusChanged;

        #region M:CreateSerializer:ICustomXmlSerializer
        public override ICustomXmlSerializer CreateSerializer() {
            return new ViewCustomSerializer(this);
            }
        #endregion
        #region M:Show:Boolean
        public Boolean Show() {
            using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                if (IsVisible)       { return true;  }
                if (!RaiseShowing()) { return false; }
                IsVisible = true;
                Shown.RaiseEvent(this);
                return true;
                }
            }
        #endregion
        #region M:Hide:Boolean
        public Boolean Hide() {
            if (!IsVisible)     { return true;  }
            if (!RaiseHiding()) { return false; }
            IsVisible = false;
            Hidden.RaiseEvent(this);
            return true;
            }
        #endregion
        #region M:ShowInFront:Boolean
        public Boolean ShowInFront() {
            using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                if (!Show()) { return false; }
                IsSelected = true;
                return true;
                }
            }
        #endregion
        #region M:RaiseShowing:Boolean
        protected Boolean RaiseShowing() {
            var e = new CancelEventArgs(false);
            Showing.RaiseEvent(this, e);
            return !e.Cancel;
            }
        #endregion
        #region M:RaiseHiding:Boolean
        protected Boolean RaiseHiding() {
            var e = new CancelEventArgs(false);
            Hiding.RaiseEvent(this, e);
            return !e.Cancel;
            }
        #endregion
        #region M:RaiseDockPositionChanging(DockAction)
        internal void RaiseDockPositionChanging(DockAction action) {
            DockPositionChanging.RaiseEvent(null, new DockEventArgs(action, this, true));
            }
        #endregion
        #region M:RaiseDockPositionChanged(DockAction,Boolean)
        internal void RaiseDockPositionChanged(DockAction action, Boolean isActionSuccessful) {
            DockPositionChanged.RaiseEvent(null, new DockEventArgs(action, this, isActionSuccessful));
            }
        #endregion
        #region M:RaisePinStatusChanged
        protected void RaisePinStatusChanged() {
            PinStatusChanged.RaiseEvent(this);
            }
        #endregion
        #region M:ValidateDockedWidth(SplitterLength)
        protected override void ValidateDockedWidth(SplitterLength width) {
            if (width.IsFill) { throw new ArgumentException("View does not accept Fill values for DockedWidth."); }
            }
        #endregion
        #region M:ValidateDockedHeight(SplitterLength)
        protected override void ValidateDockedHeight(SplitterLength height) {
            if (height.IsFill) { throw new ArgumentException("View does not accept Fill values for DockedHeight."); }
            }
        #endregion
        #region M:OnIsSelectedChanged
        protected override void OnIsSelectedChanged() {
            base.OnIsSelectedChanged();
            IsViewSelectedChanged.RaiseEvent(null, new IsViewSelectedEventArgs(this));
            }
        #endregion
        #region M:ToString:String
        public override String ToString() {
            return String.Format("{0}, Title = {1}, Name = {2}, DockedWidth = {3}, DockedHeight = {4}", (Object)GetType().Name,
                (Object)(Title == null ? "<null>" : Title.ToString()),
                (Object)Name, (Object)DockedWidth, (Object)DockedHeight);
            }
        #endregion
        #region M:GetFloatingStructure(ViewGroup):ViewElement
        public override ViewElement GetFloatingStructure(ViewGroup oldParent) {
            if (!DockOperations.IsDockPositionChanging ||
                DockOperations.LastDockAction != DockAction.Undock ||
                !(oldParent is TabGroup)) {
                return base.GetFloatingStructure(oldParent);
                }
            var tabGroup = TabGroup.Create();
            tabGroup.Children.Add(this);
            return tabGroup;
            }
        #endregion
        #region M:Create<T>(T):View<T>
        public static View<T> Create<T>(T content) {
            return new View<T>(content);
            }
        #endregion
        #region M:Create:View
        public static View Create() {
            return ViewElementFactory.Current.CreateView();
            }
        #endregion
        #region M:Create(WindowProfile,String):View
        public static View Create(WindowProfile owningProfile, String name) {
            if (owningProfile == null) { throw new ArgumentNullException(nameof(owningProfile)); }
            var view = Create();
            Initialize(view, owningProfile, name);
            return view;
            }
        #endregion
        #region M:Create(WindowProfile,String,Type):View
        public static View Create(WindowProfile owningProfile, String name, Type viewType) {
            if (owningProfile == null) { throw new ArgumentNullException(nameof(owningProfile)); }
            var view = ViewElementFactory.Current.CreateView(viewType);
            Initialize(view, owningProfile, name);
            return view;
            }
        #endregion
        #region M:Create(ViewGroup,String,Type):View
        public static View Create(ViewGroup parent, String name, Type viewType) {
            if (parent == null) { throw new ArgumentNullException(nameof(parent)); }
            var view = ViewElementFactory.Current.CreateView(viewType);
            Initialize(view, parent, name);
            return view;
            }
        #endregion
        #region M:Initialize(View,ViewGroup,String)
        private static void Initialize(View view, ViewGroup parent, String name) {
            view.Name = name;
            parent.Children.Add(view);
            }
        #endregion
        #region M:Initialize(View,WindowProfile,String)
        private static void Initialize(View view, WindowProfile owningProfile, String name) {
            view.Name = name;
            view.Float(owningProfile);
            }
        #endregion
        }
    }
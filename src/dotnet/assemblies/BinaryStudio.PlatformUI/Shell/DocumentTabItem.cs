using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DocumentTabItem : GroupControlTabItem
        {
        public static readonly DependencyProperty TabStateProperty = DependencyProperty.Register("TabState", typeof(TabState), typeof(DocumentTabItem), new FrameworkPropertyMetadata(TabState.Normal, null, CoerceTabState));
        public static readonly DependencyProperty EffectiveTabStateProperty = DependencyProperty.RegisterAttached("EffectiveTabState", typeof(TabState), typeof(DocumentTabItem), new FrameworkPropertyMetadata(TabState.Normal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.RegisterAttached("RowIndex", typeof(Int32), typeof(DocumentTabItem), new FrameworkPropertyMetadata(Boxes.Int32Zero, OnRowIndexChanged));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(Boolean), typeof(DocumentTabItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsActiveChanged));
        public static readonly DependencyProperty IsPreviewTabProperty = DependencyProperty.Register("IsPreviewTab", typeof(Boolean), typeof(DocumentTabItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, OnIsPreviewTabChanged));
        public static readonly DependencyProperty IsTabVisibleProperty = DependencyProperty.RegisterAttached("IsTabVisible", typeof(Boolean), typeof(DocumentTabItem), new PropertyMetadata(Boxes.BooleanFalse));

        private Boolean IsMultiSelected
            {
            get
                {
                if (View == null)
                    return false;
                return MultiSelectionManager.Instance.Contains(View);
                }
            }

        private Boolean IsInMultiSelectGroup
            {
            get
                {
                if (View == null || View.Parent == null)
                    return false;
                return View.Parent == MultiSelectionManager.Instance.ContainingGroup;
                }
            }

        public View View
            {
            get
                {
                return DataContext as View;
                }
            }

        public TabState TabState
            {
            get { return (TabState)GetValue(TabStateProperty); }
            set { SetValue(TabStateProperty, value); }
            }

        public Boolean IsActive
            {
            get
                {
                return (Boolean)GetValue(IsActiveProperty);
                }
            }

        public Boolean IsPreviewTab
            {
            get
                {
                return (Boolean)GetValue(IsPreviewTabProperty);
                }
            }

        public static event EventHandler<RowIndexChangedEventArgs> RowIndexChanged;

        public DocumentTabItem()
            {
            DataContextChanged += OnDataContextChanged;
            }

        private static Object CoerceTabState(DependencyObject d, Object value)
            {
            return ((DocumentTabItem)d).CoerceTabState((TabState)value);
            }

        private Object CoerceTabState(TabState value)
            {
            if (!IsPreviewTab)
                return TabState.Normal;
            return value;
            }

        public static TabState GetEffectiveTabState(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (TabState)element.GetValue(EffectiveTabStateProperty);
            }

        public static Int32 GetRowIndex(View view)
            {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return (Int32)view.GetValue(RowIndexProperty);
            }

        public static void SetRowIndex(View view, Int32 row)
            {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            view.SetValue(RowIndexProperty, row);
            }

        private static void OnRowIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            var view = (View)obj;
            // ISSUE: reference to a compiler-generated field
            RowIndexChanged.RaiseEvent(null, new RowIndexChangedEventArgs(view));
            }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            ((DocumentTabItem)d).OnIsActiveChanged(e);
            }

        private void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
            {
            if (!(Boolean)e.NewValue || !IsSelected)
                return;
            MaybeCancelMultiSelect();
            }

        private static void OnIsPreviewTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            ((DocumentTabItem)d).OnIsPreviewTabChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
            }

        private void OnIsPreviewTabChanged(Boolean oldValue, Boolean newValue)
            {
            CoerceValue(TabStateProperty);
            }

        public static Boolean GetIsTabVisible(View view)
            {
            Validate.IsNotNull(view, "view");
            return (Boolean)view.GetValue(IsTabVisibleProperty);
            }

        public static void SetIsTabVisible(View view, Boolean value)
            {
            Validate.IsNotNull(view, "view");
            view.SetValue(IsTabVisibleProperty, Boxes.Box(value));
            }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            {
            base.OnMouseLeftButtonDown(e);
            var dataContext = DataContext as View;
            if (dataContext != null && (dataContext.IsPinned || DocumentGroup.GetIsPreviewView(dataContext)))
                dataContext.IsSelected = true;
            if (!NativeMethods.IsControlPressed())
                return;
            ViewCommands.MultiSelectCommand.Execute(DataContext, this);
            }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
            {
            base.OnMouseLeftButtonUp(e);
            if (NativeMethods.IsControlPressed())
                return;
            ViewCommands.CancelMultiSelectionCommand.Execute(null, this);
            }

        protected override void OnSelected(RoutedEventArgs e)
            {
            base.OnSelected(e);
            var documentGroupControl = ItemsControl.ItemsControlFromItemContainer(this) as DocumentGroupControl;
            if (documentGroupControl != null)
                documentGroupControl.TouchAccessOrder(this);
            if (!IsActive)
                return;
            MaybeCancelMultiSelect();
            }

        private void MaybeCancelMultiSelect()
            {
            if (MultiSelectionManager.Instance.SelectedElementCount <= 1 || IsMultiSelected || IsInMultiSelectGroup && NativeMethods.IsControlPressed() && NativeMethods.IsLeftButtonPressed())
                return;
            ViewCommands.CancelMultiSelectionCommand.Execute(null, this);
            }

        private void OnDataContextChanged(Object sender, DependencyPropertyChangedEventArgs e)
            {
            var binding1 = new Binding();
            binding1.Source = this;
            var propertyPath1 = new PropertyPath(TabStateProperty);
            binding1.Path = propertyPath1;
            var num1 = 1;
            binding1.Mode = (BindingMode)num1;
            var binding2 = (BindingBase)binding1;
            var newValue = e.NewValue as View;
            if (newValue != null)
                {
                var multiBinding = new MultiBinding();
                multiBinding.Converter = EffectiveTabStateConverter.Instance;
                multiBinding.Bindings.Add(binding2);
                var bindings = multiBinding.Bindings;
                var binding3 = new Binding();
                binding3.Source = newValue;
                var propertyPath2 = new PropertyPath(ViewElement.IsSelectedProperty);
                binding3.Path = propertyPath2;
                var num2 = 1;
                binding3.Mode = (BindingMode)num2;
                bindings.Add(binding3);
                binding2 = multiBinding;
                var isActiveProperty = IsActiveProperty;
                var binding4 = new Binding();
                binding4.Source = newValue;
                var propertyPath3 = new PropertyPath(View.IsActiveProperty);
                binding4.Path = propertyPath3;
                var num3 = 1;
                binding4.Mode = (BindingMode)num3;
                BindingOperations.SetBinding(this, isActiveProperty, binding4);
                var previewTabProperty = IsPreviewTabProperty;
                var binding5 = new Binding();
                binding5.Source = newValue;
                var propertyPath4 = new PropertyPath(DocumentGroup.IsPreviewViewProperty);
                binding5.Path = propertyPath4;
                var num4 = 1;
                binding5.Mode = (BindingMode)num4;
                BindingOperations.SetBinding(this, previewTabProperty, binding5);
                var visibilityProperty = VisibilityProperty;
                var binding6 = new Binding();
                binding6.Source = newValue;
                var propertyPath5 = new PropertyPath(IsTabVisibleProperty);
                binding6.Path = propertyPath5;
                var num5 = 3;
                binding6.Mode = (BindingMode)num5;
                var visibilityConverter = new BooleanToVisibilityConverter();
                binding6.Converter = visibilityConverter;
                BindingOperations.SetBinding(this, visibilityProperty, binding6);
                }
            else
                {
                BindingOperations.ClearBinding(this, IsActiveProperty);
                BindingOperations.ClearBinding(this, IsPreviewTabProperty);
                BindingOperations.ClearBinding(this, VisibilityProperty);
                }
            BindingOperations.SetBinding(this, EffectiveTabStateProperty, binding2);
            }

        private class EffectiveTabStateConverter : MultiValueConverter<TabState, Boolean, TabState>
            {
            private static EffectiveTabStateConverter _instance;

            public static EffectiveTabStateConverter Instance
                {
                get
                    {
                    if (_instance == null)
                        _instance = new EffectiveTabStateConverter();
                    return _instance;
                    }
                }

            protected override TabState Convert(TabState tabState, Boolean selected, Object parameter, CultureInfo culture)
                {
                if (selected)
                    return TabState.Normal;
                return tabState;
                }
            }
        }
    }

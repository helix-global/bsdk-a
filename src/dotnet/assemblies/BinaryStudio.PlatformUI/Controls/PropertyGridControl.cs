using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Controls.Internal;

namespace BinaryStudio.PlatformUI.Controls
{
    public class PropertyGridControl : Control {
        static PropertyGridControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridControl), new FrameworkPropertyMetadata(typeof(PropertyGridControl)));
            }

        #region P:SelectedObject:Object
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(Object), typeof(PropertyGridControl), new PropertyMetadata(default(Object), OnSelectedObjectChanged));
        private static void OnSelectedObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is PropertyGridControl source)
                {
                source.OnSelectedObjectChanged();
                }
            }

        private void OnSelectedObjectChanged() {
            if (ItemsHost != null) {
                ItemsHost.ItemsSource = (SelectedObject != null)
                    ? GridEntry.GetEntries(null, SelectedObject, 0, this)
                    : null;
                }
            }

        public Object SelectedObject {
            get { return GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
            }
        #endregion
        #region P:LeftColumnWidth:GridLength
        public static readonly DependencyProperty LeftColumnWidthProperty = DependencyProperty.Register("LeftColumnWidth", typeof(GridLength), typeof(PropertyGridControl), new PropertyMetadata(new GridLength(1.0, GridUnitType.Star)));
        public GridLength LeftColumnWidth {
            get { return (GridLength)GetValue(LeftColumnWidthProperty); }
            set { SetValue(LeftColumnWidthProperty, value); }
            }
        #endregion
        #region P:RightColumnWidth:GridLength
        public static readonly DependencyProperty RightColumnWidthProperty = DependencyProperty.Register("RightColumnWidth", typeof(GridLength), typeof(PropertyGridControl), new PropertyMetadata(new GridLength(2.0, GridUnitType.Star)));
        public GridLength RightColumnWidth {
            get { return (GridLength)GetValue(RightColumnWidthProperty); }
            set { SetValue(RightColumnWidthProperty, value); }
            }
        #endregion
        #region P:TextWrapping:TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(PropertyGridControl), new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public TextWrapping TextWrapping
            {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
            }
        #endregion
        #region P:DescriptionPanelTemplate:DataTemplate
        public static readonly DependencyProperty DescriptionPanelTemplateProperty = DependencyProperty.Register("DescriptionPanelTemplate", typeof(DataTemplate), typeof(PropertyGridControl), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate DescriptionPanelTemplate
            {
            get { return (DataTemplate)GetValue(DescriptionPanelTemplateProperty); }
            set { SetValue(DescriptionPanelTemplateProperty, value); }
            }
        #endregion
        #region P:SelectedPropertyDescriptor:PropertyDescriptor
        private static readonly DependencyPropertyKey SelectedPropertyDescriptorPropertyKey = DependencyProperty.RegisterReadOnly("SelectedPropertyDescriptor", typeof(PropertyDescriptor), typeof(PropertyGridControl), new PropertyMetadata(default(PropertyDescriptor), OnSelectedPropertyDescriptorChanged));
        public static readonly DependencyProperty SelectedPropertyDescriptorProperty = SelectedPropertyDescriptorPropertyKey.DependencyProperty;
        private static void OnSelectedPropertyDescriptorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as PropertyGridControl);
            if (source != null) {
                source.OnSelectedPropertyDescriptorChanged();
                }
            }

        protected virtual void OnSelectedPropertyDescriptorChanged() {
            }

        public PropertyDescriptor SelectedPropertyDescriptor
            {
            get { return (PropertyDescriptor)GetValue(SelectedPropertyDescriptorProperty); }
            internal set { SetValue(SelectedPropertyDescriptorPropertyKey, value); }
            }
        #endregion
        #region P:SelectedValue:Object
        private static readonly DependencyPropertyKey SelectedValuePropertyKey = DependencyProperty.RegisterReadOnly("SelectedValue", typeof(Object), typeof(PropertyGridControl), new PropertyMetadata(default(Object), OnSelectedValueChanged));
        public static readonly DependencyProperty SelectedValueProperty = SelectedValuePropertyKey.DependencyProperty;
        private static void OnSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is PropertyGridControl source) {
                source.OnSelectedValueChanged();
                }
            }

        private void OnSelectedValueChanged() {
            var handler = Selected;
            if (handler != null) {
                handler(this, EventArgs.Empty);
                }
            }

        public Object SelectedValue
            {
            get { return GetValue(SelectedValueProperty); }
            internal set { SetValue(SelectedValuePropertyKey, value); }
            }
        #endregion
        #region P:IsDescriptionPanelVisible:Boolean
        public static readonly DependencyProperty IsDescriptionPanelVisibleProperty = DependencyProperty.Register("IsDescriptionPanelVisible", typeof(Boolean), typeof(PropertyGridControl), new PropertyMetadata(true));
        public Boolean IsDescriptionPanelVisible
            {
            get { return (Boolean)GetValue(IsDescriptionPanelVisibleProperty); }
            set { SetValue(IsDescriptionPanelVisibleProperty, value); }
            }
        #endregion
        #region P:IsDefaultExpanded:Boolean
        public static readonly DependencyProperty IsDefaultExpandedProperty = DependencyProperty.Register("IsDefaultExpanded", typeof(Boolean), typeof(PropertyGridControl), new PropertyMetadata(default(Boolean)));
        public Boolean IsDefaultExpanded
            {
            get { return (Boolean)GetValue(IsDefaultExpandedProperty); }
            set { SetValue(IsDefaultExpandedProperty, value); }
            }
        #endregion
        #region P:ReadOnlyForeground:Brush
        public static readonly DependencyProperty ReadOnlyForegroundProperty = DependencyProperty.Register("ReadOnlyForeground", typeof(Brush), typeof(PropertyGridControl), new PropertyMetadata(SystemColors.GrayTextBrush));
        public Brush ReadOnlyForeground
            {
            get { return (Brush)GetValue(ReadOnlyForegroundProperty); }
            set { SetValue(ReadOnlyForegroundProperty, value); }
            }
        #endregion

        #region M:OnApplyTemplate
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            ItemsHost = GetTemplateChild("ItemsHost") as TreeView;
            OnSelectedObjectChanged();
            }
        #endregion

        public event EventHandler Selected;

        internal void Edit(GridEntry e) {
            if (e != null) {
                e.IsSelected = true;
                }
            }

        private TreeView ItemsHost { get; set; }
        }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Internal
{
    public class GridEntryContentPresenter : Control {
        static GridEntryContentPresenter() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridEntryContentPresenter), new FrameworkPropertyMetadata(typeof(GridEntryContentPresenter)));
            CommandManager.RegisterClassCommandBinding(typeof(GridEntryContentPresenter), new CommandBinding(CancelEditCommand, OnCancelEditCommandExecuted, OnCanExecuteCancelEditCommand));
            }


        public static RoutedCommand BeginEditCommand  = new RoutedCommand(nameof(BeginEditCommand),  typeof(GridEntryContentPresenter));
        public static RoutedCommand EndEditCommand    = new RoutedCommand(nameof(EndEditCommand),    typeof(GridEntryContentPresenter));

        #region E:CancelEditCommand:RoutedCommand
        public static RoutedCommand CancelEditCommand = new RoutedCommand(nameof(CancelEditCommand), typeof(GridEntryContentPresenter));
        private static void OnCanExecuteCancelEditCommand(Object sender, CanExecuteRoutedEventArgs e) {
            var source = sender as GridEntryContentPresenter;
            if (source != null) {
                source.OnCanExecuteCancelEditCommand(e);
                }
            }

        private void OnCanExecuteCancelEditCommand(CanExecuteRoutedEventArgs e) {
            e.CanExecute = IsEditing;
            e.Handled = true;
            }

        private static void OnCancelEditCommandExecuted(Object sender, ExecutedRoutedEventArgs e) {
            var source = sender as GridEntryContentPresenter;
            if (source != null) {
                source.OnCancelEditCommandExecuted(e);
                }
            }

        private void OnCancelEditCommandExecuted(ExecutedRoutedEventArgs e) {
            IsEditing = false;
            e.Handled = true;
            }
        #endregion

        #region E:EndEdit:RoutedEvent
        public static readonly RoutedEvent EndEditEvent = EventManager.RegisterRoutedEvent("EndEdit", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GridEntryContentPresenter));
        public static void AddEndEditHandler(UIElement source, RoutedEventHandler handler) {
            if (source != null) {
                source.AddHandler(EndEditEvent, handler);
                }
            }
        public static void RemoveEndEditHandler(UIElement source, RoutedEventHandler handler) {
            if (source != null) {
                source.RemoveHandler(EndEditEvent, handler);
                }
            }
        #endregion
        #region P:EditTemplate:DataTemplate
        public static readonly DependencyProperty EditTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(GridEntryContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate EditTemplate {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
            }
        #endregion
        #region P:DisplayTemplate:DataTemplate
        public static readonly DependencyProperty DisplayTemplateProperty = DependencyProperty.Register("DisplayTemplate", typeof(DataTemplate), typeof(GridEntryContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate DisplayTemplate {
            get { return (DataTemplate)GetValue(DisplayTemplateProperty); }
            set { SetValue(DisplayTemplateProperty, value); }
            }
        #endregion
        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(GridEntryContentPresenter), new PropertyMetadata(default(Object)));
        public Object Content {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        #region P:IsEditing:Boolean
        private static readonly DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(Boolean), typeof(GridEntryContentPresenter), new PropertyMetadata(default(Boolean), OnIsEditingChanged));
        private static void OnIsEditingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((GridEntryContentPresenter)sender).OnIsEditingChanged();
        }

        private void OnIsEditingChanged() {}

        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;
        public Boolean IsEditing {
            get { return (Boolean)GetValue(IsEditingProperty); }
            private set { SetValue(IsEditingPropertyKey, value); }
            }
        #endregion

        private void BeginEdit() {
            IsEditing = true;
            }

        private void EndEdit() {
            var box = this.FindAll<TextBox>().FirstOrDefault(i => i.Focusable);
            var entry = DataContext as GridEntry;
            if ((entry != null) && (!entry.IsReadOnly)) {
                if (entry.Style == GridEntryStyle.TextBox) {
                    if (box != null) {
                        var binding = BindingOperations.GetBindingExpression(box, TextBox.TextProperty);
                        if (binding != null) {
                            binding.UpdateSource();
                            }
                        }
                    }
                }
            IsEditing = false;
            RaiseEvent(new RoutedEventArgs(EndEditEvent));
            }

        private void CancelEdit() {
            CancelEditCommand.Execute(null, this);
            }

        #region M:OnMouseDown(MouseButtonEventArgs)
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            BeginEdit();
            if (ContentPresenter != null) {
                ContentPresenter.DoAfterLayoutUpdated(()=>{
                    var box = this.FindAll<TextBox>().FirstOrDefault(i => i.Focusable);
                    if (box != null) {
                        box.Focus();
                        box.DoAfterLayoutUpdated(()=>{
                            box.CaretIndex = box.GetCharacterIndexFromPoint(e.GetPosition(box), true);
                            box.Select(box.CaretIndex, 0);
                            });
                        return;
                        }
                    var c = this.FindAll<UIElement>().FirstOrDefault(i => i.Focusable);
                    if (c != null) {
                        c.Focus();
                        }
                    });
                }
            }
        #endregion
        #region M:OnLostKeyboardFocus(KeyboardFocusChangedEventArgs)
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            if (e.NewFocus != null) {
                if (e.OldFocus != null) {
                    if (((UIElement)e.NewFocus).FindAll<UIElement>(false, true).Any(i => ReferenceEquals(i, e.OldFocus))) {
                        e.Handled = true;
                        return;
                        }
                    var nparents = ((UIElement)e.NewFocus).FindAll<UIElement>(true, true).ToArray();
                    var oparents = ((UIElement)e.OldFocus).FindAll<UIElement>(true, true).ToArray();
                    if (nparents.Intersect(oparents).Any()) {
                        e.Handled = true;
                        return;
                        }
                    oparents = ((UIElement)e.OldFocus).FindAll<UIElement>(false, true).ToArray();
                    if (nparents.Intersect(oparents).Any()) {
                        e.Handled = true;
                        return;
                        }
                    }
                }
            else
                {
                e.Handled = true;
                return;
                }
            EndEdit();
            }
        #endregion
        #region M:OnApplyTemplate
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            ContentPresenter = GetTemplateChild("ContentPresenter") as ContentControl;
            }
        #endregion

        public GridEntryContentPresenter() {
            //CommandManager.AddExecutedHandler(this, OnExecutedCommand);
            }

        private void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            
            }

        internal ContentControl ContentPresenter;
        }
}
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
    {
    public class Asn1GridEntryContentPresenter : Control, INotifyPropertyChanged
        {
        static Asn1GridEntryContentPresenter()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Asn1GridEntryContentPresenter), new FrameworkPropertyMetadata(typeof(Asn1GridEntryContentPresenter)));
            }

        public static RoutedCommand BeginEditCommand  = new RoutedCommand(nameof(BeginEditCommand),  typeof(Asn1GridEntryContentPresenter));
        public static RoutedCommand EndEditCommand    = new RoutedCommand(nameof(EndEditCommand),    typeof(Asn1GridEntryContentPresenter));
        public static RoutedCommand CancelEditCommand = new RoutedCommand(nameof(CancelEditCommand), typeof(Asn1GridEntryContentPresenter));

        #region P:EditTemplate:DataTemplate
        public static readonly DependencyProperty EditTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(Asn1GridEntryContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate EditTemplate {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
            }
        #endregion
        #region P:DisplayTemplate:DataTemplate
        public static readonly DependencyProperty DisplayTemplateProperty = DependencyProperty.Register("DisplayTemplate", typeof(DataTemplate), typeof(Asn1GridEntryContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate DisplayTemplate {
            get { return (DataTemplate)GetValue(DisplayTemplateProperty); }
            set { SetValue(DisplayTemplateProperty, value); }
            }
        #endregion
        //#region P:Content:Object
        //public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(Asn1GridEntryContentPresenter), new PropertyMetadata(default(Object)));
        //public Object Content {
        //    get { return (Object)GetValue(ContentProperty); }
        //    set { SetValue(ContentProperty, value); }
        //    }
        //#endregion
        #region P:IsEditing:Boolean
        private static readonly DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(Boolean), typeof(Asn1GridEntryContentPresenter), new PropertyMetadata(default(Boolean)));
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
            IsEditing = false;
            }

        private void CancelEdit() {
            IsEditing = false;
            }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            //if (e.LeftButton == MouseButtonState.Pressed) {
                //Debug.Print("{0}:OnMouseDown", DateTime.Now.ToString("O"));
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
              //  }
            }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            //Debug.Print("{0}:OnLostKeyboardFocus({1},{2})", DateTime.Now.ToString("O"), e.OldFocus, e.NewFocus);
            if (e.NewFocus != null) {
                if (e.OldFocus != null) {
                    if (((UIElement)e.NewFocus).FindAll<UIElement>(true).Any(i => ReferenceEquals(i, e.OldFocus))) {
                        e.Handled = true;
                        return;
                        }
                    }
                }
            EndEdit();
            }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            if (e.NewFocus != null) {
                if ((e.NewFocus is MenuBase) ||
                    (e.NewFocus is MenuItem))
                    {
                    e.Handled = true;
                    }
                }
            }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            ContentPresenter = GetTemplateChild("ContentPresenter") as ContentControl;
            }

        #region M:OnPropertyChanged(String)
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        #endregion
        #region M:OnPropertyChanged(DependencyPropertyChangedEventArgs)
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }
        #endregion

        internal ContentControl ContentPresenter;
        }
    }
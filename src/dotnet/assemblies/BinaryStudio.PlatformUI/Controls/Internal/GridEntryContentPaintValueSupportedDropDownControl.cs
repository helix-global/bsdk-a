using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class GridEntryContentPaintValueSupportedDropDownControl : Control, IServiceProvider
        {
        private ToggleButton DropDownToggleButton;
        private WindowsFormsHost FormsHost;
        private Button EllipsisButton;
        private Popup PART_Popup;
        private TextBox PART_TextBox;

        static GridEntryContentPaintValueSupportedDropDownControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridEntryContentPaintValueSupportedDropDownControl), new FrameworkPropertyMetadata(typeof(GridEntryContentPaintValueSupportedDropDownControl)));
            }

        #region P:IsDropDownOpen:Boolean
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(Boolean), typeof(GridEntryContentPaintValueSupportedDropDownControl), new FrameworkPropertyMetadata(false));
        public Boolean IsDropDownOpen {
            get { return (Boolean)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty,value); }
            }
        #endregion
        #region P:EditStyle:UITypeEditorEditStyle
        private static readonly DependencyPropertyKey EditStylePropertyKey = DependencyProperty.RegisterReadOnly("EditStyle", typeof(UITypeEditorEditStyle), typeof(GridEntryContentPaintValueSupportedDropDownControl), new FrameworkPropertyMetadata(UITypeEditorEditStyle.None));
        public  static readonly DependencyProperty EditStyleProperty = EditStylePropertyKey.DependencyProperty;
        public UITypeEditorEditStyle EditStyle {
            get { return (UITypeEditorEditStyle)GetValue(EditStyleProperty); }
            private set { SetValue(EditStylePropertyKey, value); }
            }
        #endregion
        #region P:Value:Object
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(GridEntryContentPaintValueSupportedDropDownControl), new FrameworkPropertyMetadata(null));
        public String Text {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty,value); }
            }
        #endregion

        //public UITypeEditorEditStyle EditStyle { get {
        //    var context = DataContext as ITypeDescriptorContext;
        //    if (context != null) {
        //        var editor = context.GetEditor();
        //        return (editor != null)
        //            ? editor.GetEditStyle()
        //            : UITypeEditorEditStyle.None;
        //        }
        //    return UITypeEditorEditStyle.None;
        //    }}
        

        //protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
        //    base.OnPreviewGotKeyboardFocus(e);
        //    if (PART_TextBox != null) {
        //        PART_TextBox.Focus();
        //        e.Handled = true;
        //        }
        //    }

        internal WindowsFormsHost WindowsFormsHost{get{
            return FormsHost;
            }}

        /// <summary>
        /// Closes any previously opened drop down control area.
        /// </summary>
        public void CloseDropDown() {
            e = true;
            if (FormsHost != null) {
                IsDropDownOpen = false;
                FormsHost.Child = null;
                IsEditorActivated = false;
                }
            Focus();
            }

        volatile Boolean e = false;

        internal Boolean IsNestedFocused { get {
            if (FormsHost != null) {
                if (FormsHost.Child != null) {
                    return FormsHost.Child.Focused;
                    }
                }
            return false;
            }}

        IDictionary<Int32, String> MessageNumber2String = new Dictionary<Int32,String>{
            {0x0007, "WM_SETFOCUS"},
            {0x0008, "WM_KILLFOCUS"},
            {0x0046, "WM_WINDOWPOSCHANGING"},
            {0x0003, "WM_MOVE"},
            {0x0002, "WM_DESTROY"},
            {0x0005, "WM_SIZE"},
            {0x000E, "WM_GETTEXTLENGTH"},
            {0x000D, "WM_GETTEXT"},
            {0x0083, "WM_NCCALCSIZE"},
            {0x0047, "WM_WINDOWPOSCHANGED"},
            {0x0082, "WM_NCDESTROY"},
            {0x0018, "WM_SHOWWINDOW"},
            {0x0020, "WM_SETCURSOR"},
            {0x0021, "WM_MOUSEACTIVATE"},
            {0x0022, "WM_CHILDACTIVATE"},
            {0x0023, "WM_QUEUESYNC"},
            {0x0201, "WM_LBUTTONDOWN"},
            {0x0210, "WM_PARENTNOTIFY"}
            };

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnGotKeyboardFocus(e);
            if (ReferenceEquals(e.OriginalSource, this)) {
                if (PART_TextBox != null) {
                    PART_TextBox.Focus();
                    }
                }
            }

        /// <summary>
        /// Displays the specified control in a drop down area below a value field of the property grid that provides this service.
        /// </summary>
        /// <param name="control">The drop down list <see cref="T:System.Windows.Forms.Control"/> to open. </param>
        public void DropDownControl(System.Windows.Forms.Control control) {
            if (control != null) {
                if (FormsHost != null) {
                    FormsHost.Child = control;
                    FormsHost.KeyDown += OnKeyDown;
                    //FormsHost.MessageHook += delegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
                    //    String r;
                    //    if (MessageNumber2String.TryGetValue(msg, out r)) {
                    //        Debug.Print("{0},{1},{2},{3}", hwnd, r, wParam, lParam);
                    //        }
                    //    else
                    //        {
                    //        Debug.Print("{0},{1:X},{2},{3}", hwnd, msg, wParam, lParam);
                    //        }
                    //    return IntPtr.Zero;
                    //    };
                    e = false;
                    control.Focus();
                    while (!e) {
                        Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new Action(delegate { }));
                        if (!control.ContainsFocus) { break; }
                        }
                    FormsHost.KeyDown -= OnKeyDown;
                    CloseDropDown();
                    }
                }
           }

        private void OnKeyDown(Object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                this.e = true;
                }
            }

        /// <summary>
        /// Shows the specified <see cref="T:System.Windows.Forms.Form"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Forms.DialogResult"/> indicating the result code returned by the <see cref="T:System.Windows.Forms.Form"/>.
        /// </returns>
        /// <param name="dialog">The <see cref="T:System.Windows.Forms.Form"/> to display. </param>
        public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.Form dialog) {
            return dialog.ShowDialog();
            }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
        public Object GetService(Type serviceType) {
            return serviceType == typeof(IWindowsFormsEditorService)
                ? this
                : null;
            }

        protected override Size MeasureOverride(Size constraint) {
            var r =  base.MeasureOverride(constraint);
            IsDropDownOpen = false;
            return r;
            }

        #region M:GetEditor(ITypeDescriptorContext):UITypeEditor
        private static UITypeEditor GetEditor(ITypeDescriptorContext context) {
            if (context != null) {
                var descriptor = context.PropertyDescriptor;
                if (descriptor != null) {
                    return descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
                    }
                }
            return null;
            }
        #endregion

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            PART_Popup = GetTemplateChild("PART_Popup") as Popup;
            DropDownToggleButton = GetTemplateChild("DropDownToggleButton") as ToggleButton;
            EllipsisButton = GetTemplateChild("EllipsisButton") as Button;
            PART_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            FormsHost = GetTemplateChild("FormsHost") as WindowsFormsHost;
            #region UITypeEditorEditStyle.DropDown
            if (DropDownToggleButton != null) {
                DropDownToggleButton.Checked += delegate{
                    Dispatcher.BeginInvoke(new Action(delegate{
                        var context = DataContext as ITypeDescriptorContext;
                        if (context != null) {
                            var editor = GetEditor(context);
                            if ((editor != null) && (editor.GetEditStyle() == UITypeEditorEditStyle.DropDown)) {
                                var descriptor = context.PropertyDescriptor;
                                if (descriptor != null) {
                                    var source = (context.Instance is ICustomTypeDescriptor)
                                        ? ((ICustomTypeDescriptor)context.Instance).GetPropertyOwner(descriptor)
                                        : context.Instance;
                                    var value = descriptor.GetValue(source);
                                    IsEditorActivated = true;
                                    var n = editor.EditValue(context, this, value);
                                    IsEditorActivated = false;
                                    if (!descriptor.IsReadOnly) {
                                        descriptor.SetValue(source, n);
                                        var binding = BindingOperations.GetBindingExpression(PART_TextBox, TextBox.TextProperty);
                                        if (binding != null) {
                                            binding.UpdateTarget();
                                            }
                                        }
                                    }
                                }
                            }
                        }),DispatcherPriority.ApplicationIdle);
                    };
                DropDownToggleButton.Unchecked += delegate{
                    e = true;
                    IsEditorActivated = false;
                    };
                }
            #endregion
            #region UITypeEditorEditStyle.Modal
            if (EllipsisButton != null) {
                EllipsisButton.Click += delegate{
                    var context = DataContext as ITypeDescriptorContext;
                    if (context != null) {
                        var editor = GetEditor(context);
                        if ((editor != null) && (editor.GetEditStyle() == UITypeEditorEditStyle.Modal)) {
                            var descriptor = context.PropertyDescriptor;
                            if (descriptor != null) {
                                var source = (context.Instance is ICustomTypeDescriptor)
                                    ? ((ICustomTypeDescriptor)context.Instance).GetPropertyOwner(descriptor)
                                    : context.Instance;
                                var value = descriptor.GetValue(source);
                                IsEditorActivated = true;
                                var ui = ResolveTypeEditor(editor);
                                var n = (Object)null;
                                if (ui != null) {
                                    n = ui.EditValue(context, this, value);
                                    }
                                else
                                    {
                                    n = editor.EditValue(context, this, value);
                                    }
                                
                                IsEditorActivated = false;
                                if (!descriptor.IsReadOnly) {
                                    descriptor.SetValue(source, n);
                                    var binding = BindingOperations.GetBindingExpression(PART_TextBox, TextBox.TextProperty);
                                    if (binding != null) {
                                        binding.UpdateTarget();
                                        }
                                    }
                                }
                            }
                        }
                    };
                }
            #endregion
            }

        private ITypeEditor ResolveTypeEditor(UITypeEditor editor) {
            var type = editor.GetType();
            //if (editor.GetType() == typeof(System.Drawing.Design.FontEditor)) {
            //    return new FontEditor();
            //    }
            return null;
            }

        public Boolean IsEditorActivated {get;set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (ReferenceEquals(e.Property, DataContextProperty)) {
                var context = DataContext as ITypeDescriptorContext;
                if (context != null) {
                    var editor = GetEditor(context);
                    EditStyle = (editor != null)
                        ? editor.GetEditStyle()
                        : UITypeEditorEditStyle.None;
                    }
                else
                    {
                    EditStyle = UITypeEditorEditStyle.None;
                    }
                }
            }
        }
    }
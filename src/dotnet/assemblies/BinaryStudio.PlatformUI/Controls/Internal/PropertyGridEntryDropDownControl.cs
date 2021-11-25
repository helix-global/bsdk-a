using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.PlatformUI.Shell.Controls;
using Microsoft.Win32;
using Control = System.Windows.Forms.Control;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class PropertyGridEntryDropDownControl : ContentControl, IServiceProvider, IWindowsFormsEditorService
        {
        static PropertyGridEntryDropDownControl() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEntryDropDownControl), new FrameworkPropertyMetadata(typeof(PropertyGridEntryDropDownControl)));
            }

        #region P:IsDropDownOpen:Boolean
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(Boolean), typeof(PropertyGridEntryDropDownControl), new PropertyMetadata(default(Boolean), OnIsDropDownOpenChanged));
        private static void OnIsDropDownOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((PropertyGridEntryDropDownControl)sender).OnIsDropDownOpenChanged();
            }

        private void OnIsDropDownOpenChanged() {
            if (IsDropDownOpen) {
                    TopLevelHwndSource = null;
                    FocusTracker.Instance.TrackFocus -= OnTrackFocus;
                    Dispatcher.BeginInvoke(new Action(()=>{
                        var context = DataContext as ITypeDescriptorContext;
                        if (context != null) {
                            var descriptor = context.PropertyDescriptor;
                            if (descriptor != null) {
                                var editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
                                if ((editor != null) && (editor.GetEditStyle() == UITypeEditorEditStyle.DropDown)) {
                                    var source = (context.Instance is ICustomTypeDescriptor)
                                        ? ((ICustomTypeDescriptor)context.Instance).GetPropertyOwner(descriptor)
                                        : context.Instance;
                                    var value = descriptor.GetValue(source);
                                    editor.EditValue(context, this, value);
                                    }
                                }
                            }
                        }),DispatcherPriority.ApplicationIdle);
                }
            else
                {
                DestroyFormsHost();
                }
            }

        public Boolean IsDropDownOpen {
            get { return (Boolean)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
            }
        #endregion

        #region M:OnApplyTemplate
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            Host = GetTemplateChild("Host") as Decorator;
            }
        #endregion
        //#region M:OnPreviewKeyboardFocusLost(Object,EventArgs)
        //private void OnPreviewKeyboardFocusLost(Object sender, EventArgs e) {
        //    GridEntryContentPresenter.CancelEditCommand.Execute(null, this);
        //    IsDropDownOpen = false;
        //    }
        //#endregion

        //protected override void OnLostFocus(RoutedEventArgs e) {
        //    Debug.Print(@"PropertyGridEntryDropDownControl.OnLostFocus");
        //    base.OnLostFocus(e);
        //    }

        private void OnLostFocus(Object sender, RoutedEventArgs e) {
            IsDropDownOpen = false;
            TopLevelHwndSource = UtilityMethods.FindTopLevelHwndSource(this);
            FocusTracker.Instance.TrackFocus += OnTrackFocus;
            }

        #region M:OnTrackFocus(Object,TrackFocusEventArgs)
        private void OnTrackFocus(Object sender, TrackFocusEventArgs e) {
            if (TopLevelHwndSource != null) {
                if (TopLevelHwndSource.Handle != e.HwndGainFocus) {
                    if (FocusableHwndHost.IsSelfOrDescendentWindow(TopLevelHwndSource.Handle, e.HwndGainFocus)) {
                        TopLevelHwndSource = null;
                        FocusTracker.Instance.TrackFocus -= OnTrackFocus;
                        GridEntryContentPresenter.CancelEditCommand.Execute(null, this);
                        }
                    }
                }
            }
        #endregion

        private static String ToString(IntPtr handle) {
            if (handle != IntPtr.Zero) {
                var builder = new StringBuilder();
                builder.AppendFormat("{0:X8}", (UInt64)handle);
                builder.AppendFormat(@" ""{0}""", NativeMethods.GetWindowText(handle));
                builder.AppendFormat(@" {{{0}}}", NativeMethods.GetClassName(handle));
                return builder.ToString();
                }
            return null;
            }

        //protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
        //    //var n = e.NewFocus as DependencyObject;
        //    //if (n != null) {
        //    //    if (IsDropDownOpen) {
        //    //        if (Host != null) {
        //    //            if (!Host.IsDescendantOf(n) && !Host.IsAncestorOf(n)) {
        //    //                GridEntryContentPresenter.CancelEditCommand.Execute(null, this);
        //    //                IsDropDownOpen = false;
        //    //                return;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    Debug.Print(@"PropertyGridEntryDropDownControl.OnLostKeyboardFocus:OldFocus=""{0}"",""{1}""",e.OldFocus,e.NewFocus);
        //    base.OnLostKeyboardFocus(e);
        //    }

        private WindowsFormsHost FormsHost;
        private Decorator Host;
        private HwndSource TopLevelHwndSource;

        #region M:IServiceProvider.GetService(Type):Object
        Object IServiceProvider.GetService(Type type) {
            if (type == typeof(IWindowsFormsEditorService)) { return this; }
            return null;
            }
        #endregion
        #region M:IWindowsFormsEditorService.CloseDropDown
        void IWindowsFormsEditorService.CloseDropDown() {
            IsDropDownOpen = false;
            var target = FormsHost;
            if (target != null) {
                target.Child = null;
                }
            }
        #endregion
        #region M:IWindowsFormsEditorService.DropDownControl(Control)
        void IWindowsFormsEditorService.DropDownControl(Control control) {
            EnsureFormsHost();
            var target = FormsHost;
            if (target != null) {
                target.Child = control;
                control.Focus();
                while (IsDropDownOpen) {
                    Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new Action(delegate { }));
                    Thread.Sleep(1);
                    //if (!control.ContainsFocus) { break; }
                    }
                }
            }
        #endregion
        #region M:IWindowsFormsEditorService.ShowDialog(Form)
        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
            throw new NotImplementedException();
            }
        #endregion
        #region M:EnsureFormsHost
        private void EnsureFormsHost() {
            var source = UtilityMethods.FindTopLevelHwndSource(this);
            if (Host != null) {
                if (FormsHost == null) {
                    FormsHost = new FocusableWindowsFormsHost(source);
                    FormsHost.LostFocus += OnLostFocus;
                    Host.Child = FormsHost;
                    }
                }
            }
        #endregion
        #region M:DestroyFormsHost
        private void DestroyFormsHost() {
            if (FormsHost != null) {
                FormsHost.LostFocus -= OnLostFocus;
                FormsHost.Child = null;
                FormsHost.Dispose();
                FormsHost = null;
                }
            }
        #endregion
        }
    }
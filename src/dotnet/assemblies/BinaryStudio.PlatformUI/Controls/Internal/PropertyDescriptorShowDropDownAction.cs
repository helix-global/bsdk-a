using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;
using System.Windows.Interactivity;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class PropertyDescriptorShowDropDownAction : TargetedTriggerAction<DependencyObject>, IServiceProvider, IWindowsFormsEditorService
        {
        protected override void Invoke(Object parameter) {
            var target = AssociatedObject as FrameworkElement;
            if (target != null) {
                var context = target.DataContext as ITypeDescriptorContext;
                if (context != null) {
                    var descriptor = context.PropertyDescriptor;
                    if (descriptor != null) {
                        var editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
                        if (editor != null) {
                            var value = descriptor.GetValue(context.Instance);
                            Dispatcher.BeginInvoke(new Action(delegate{
                                editor.EditValue(context, this, value);
                                }), DispatcherPriority.ApplicationIdle);
                            }
                        }
                    }
                }
            }


        Object IServiceProvider.GetService(Type type) {
            if (type == typeof(IWindowsFormsEditorService)) { return this; }
            return null;
            }

        void IWindowsFormsEditorService.CloseDropDown() {
            escape = true;
            var target = Target as WindowsFormsHost;
            if (target != null) {
                target.Child.LostFocus -= OnLostFocus;
                target.Child = null;
                var popup = target.FindAll<Popup>(true, true).FirstOrDefault();
                if (popup != null) {
                    popup.IsOpen = false;
                    }
                }
            }

        void IWindowsFormsEditorService.DropDownControl(Control control) {
            var target = Target as WindowsFormsHost;
            if (target != null) {
                target.Child = control;
                control.Focus();
                control.LostFocus += OnLostFocus;
                escape = false;
                while (!escape) {
                    Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new Action(delegate { }));
                    Thread.Sleep(1);
                    //if (!control.ContainsFocus) { break; }
                    }
                return;
                }
            }

        private void OnLostFocus(Object sender, EventArgs e)
        {
            return;
        }

        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
        {
            throw new NotImplementedException();
        }

        private Boolean escape;
        }
    }
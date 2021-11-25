using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Interactivity;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class PropertyDescriptorShowEditorDialogAction : TriggerAction<DependencyObject>, IServiceProvider, IWindowsFormsEditorService
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
                            editor.EditValue(context, this, value);
                            }
                        }
                    }
                }
            }

        public Object GetService(Type type) {
            if (type == typeof(IWindowsFormsEditorService)) { return this; }
            return null;
            }

        void IWindowsFormsEditorService.CloseDropDown() {
            throw new NotSupportedException();
            }

        void IWindowsFormsEditorService.DropDownControl(Control control) {
            throw new NotSupportedException();
            }

        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog) {
            if (dialog != null) {
                return dialog.ShowDialog();
                }
            throw new NotSupportedException();
            }
        }
    }
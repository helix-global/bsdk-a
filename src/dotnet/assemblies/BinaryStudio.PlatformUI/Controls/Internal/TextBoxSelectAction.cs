using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    internal class TextBoxSelectAction : TargetedTriggerAction<TextBox> {
        #region P:StartIndex:Int32
        public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register("StartIndex", typeof(Int32), typeof(TextBoxSelectAction), new PropertyMetadata(default(Int32)));
        public Int32 StartIndex {
            get { return (Int32)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
            }
        #endregion
        protected override void Invoke(Object parameter) {
            var target = Target;
            if (target != null) {
                if (StartIndex == -1) {
                    var text = target.Text;
                    if (!String.IsNullOrWhiteSpace(text)) {
                        target.Select(text.Length, 0);
                        }
                    }
                }
            }
        }
    }
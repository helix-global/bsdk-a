using System;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class ProgressRing : Control
        {
        static ProgressRing()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
            }

        public ProgressRingTemplateSettings TemplateSettings { get; }

        #region P:IsActive:Boolean
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(Boolean), typeof(ProgressRing), new PropertyMetadata(default(Boolean), OnIsActiveChanged));
        private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ProgressRing);
            if (source != null) {
                source.OnIsActiveChanged();
                }
            }

        protected void OnIsActiveChanged()
            {
            if (IsActive)
                {
                VisualStateManager.GoToState(this, "Active", true);
                }
            else
                {
                VisualStateManager.GoToState(this, "Inactive", true);
                }
            }

        public Boolean IsActive
            {
            get { return (Boolean)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
            }
        #endregion
        public Double EllipseDiameter
            {
            get { return TemplateSettings.EllipseDiameter; }
            set { TemplateSettings.EllipseDiameter = value; }
            }

        public ProgressRing()
            {
            TemplateSettings = new ProgressRingTemplateSettings();
            }

        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            OnIsActiveChanged();
            }
        }
    }

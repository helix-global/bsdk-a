using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class SingleRowTabPanel : ReorderTabPanel
        {
        static SingleRowTabPanel()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SingleRowTabPanel), new FrameworkPropertyMetadata(typeof(SingleRowTabPanel)));
            }

        #region P:SingleRowTabPanel.IsFirst:Boolean
        private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
        private static void SetIsFirst(DependencyObject source, Boolean value) {
            source.SetValue(IsFirstPropertyKey, value);
            }
        public static Boolean GetIsFirst(DependencyObject source) {
            return (Boolean)source.GetValue(IsFirstProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.IsLast:Boolean
        private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsLastProperty = IsLastPropertyKey.DependencyProperty;
        private static void SetIsLast(DependencyObject source, Boolean value) {
            source.SetValue(IsLastPropertyKey, value);
            }
        public static Boolean GetIsLast(DependencyObject source) {
            return (Boolean)source.GetValue(IsLastProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.IsImmediatelyBeforeSelection:Boolean
        private static readonly DependencyPropertyKey IsImmediatelyBeforeSelectionPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsImmediatelyBeforeSelection", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsImmediatelyBeforeSelectionProperty = IsImmediatelyBeforeSelectionPropertyKey.DependencyProperty;
        private static void SetIsImmediatelyBeforeSelection(DependencyObject source, Boolean value) {
            source.SetValue(IsImmediatelyBeforeSelectionPropertyKey, value);
            }
        public static Boolean GetIsImmediatelyBeforeSelection(DependencyObject source) {
            return (Boolean)source.GetValue(IsImmediatelyBeforeSelectionProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.IsImmediatelyAfterSelection:Boolean
        private static readonly DependencyPropertyKey IsImmediatelyAfterSelectionPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsImmediatelyAfterSelection", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsImmediatelyAfterSelectionProperty = IsImmediatelyAfterSelectionPropertyKey.DependencyProperty;
        private static void SetIsImmediatelyAfterSelection(DependencyObject source, Boolean value) {
            source.SetValue(IsImmediatelyAfterSelectionPropertyKey, value);
            }
        public static Boolean GetIsImmediatelyAfterSelection(DependencyObject source) {
            return (Boolean)source.GetValue(IsImmediatelyAfterSelectionProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.IsTruncatingTabs:Boolean
        private static readonly DependencyPropertyKey IsTruncatingTabsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsTruncatingTabs", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsTruncatingTabsProperty = IsTruncatingTabsPropertyKey.DependencyProperty;
        private static void SetIsTruncatingTabs(DependencyObject source, Boolean value) {
            source.SetValue(IsTruncatingTabsPropertyKey, value);
            }
        public static Boolean GetIsTruncatingTabs(DependencyObject source) {
            return (Boolean)source.GetValue(IsTruncatingTabsProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.UseCompressedTabStyle:Boolean
        private static readonly DependencyPropertyKey UseCompressedTabStylePropertyKey = DependencyProperty.RegisterAttachedReadOnly("UseCompressedTabStyle", typeof(Boolean), typeof(SingleRowTabPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty UseCompressedTabStyleProperty = UseCompressedTabStylePropertyKey.DependencyProperty;
        private static void SetUseCompressedTabStyle(DependencyObject source, Boolean value) {
            source.SetValue(UseCompressedTabStylePropertyKey, value);
            }
        public static Boolean GetUseCompressedTabStyle(DependencyObject source) {
            return (Boolean)source.GetValue(UseCompressedTabStyleProperty);
            }
        #endregion
        #region P:SingleRowTabPanel.CalculatedTabSize:Size
        public static readonly DependencyProperty CalculatedTabSizeProperty = DependencyProperty.RegisterAttached("CalculatedTabSize", typeof(Size), typeof(SingleRowTabPanel), new PropertyMetadata(default(Size)));
        public static void SetCalculatedTabSize(DependencyObject source, Size value) {
            source.SetValue(CalculatedTabSizeProperty, value);
            }
        public static Size GetCalculatedTabSize(DependencyObject source) {
            return (Size)source.GetValue(CalculatedTabSizeProperty);
            }
        #endregion

        #region M:GetSelectedIndex:Int32
        private Int32 GetSelectedIndex() {
            for (var i = 0; i < InternalChildren.Count; i++) {
                var e = InternalChildren[i];
                if (e != null && Selector.GetIsSelected(e)) {
                    return i;
                    }
                }
            return -1;
            }
        #endregion
        #region M:GetMinimumWidth(UIElement):Double
        private static Double GetMinimumWidth(UIElement child) {
            var r = 0.0;
            var e = child as FrameworkElement;
            if (e != null) {
                r = e.MinWidth;
                }
            return r;
            }
        #endregion

        #region M:CalculateTruncationThreshold(List<Double>,Double,Double,Double)
        private static void CalculateTruncationThreshold(List<Double> values, Double ρ, out Double τ, out Double ν) {
            values.Sort();
            for (var i = 1; i < values.Count; i++) {
                var α = values[values.Count - i];
                var β = values[values.Count - i - 1];
                var Δ = α - β;
                if (ρ - Δ * i < 0.0) {
                    τ = α;
                    ν = α - ρ / i;
                    return;
                    }
                ρ -= Δ * i;
                }
            τ = values[0];
            ν = τ - ρ / values.Count;
            }
        #endregion
        #region M:ArrangeOverride(Size):Size
        protected override Size ArrangeOverride(Size finalSize) {
            var r = UndockingOffset;
            foreach (UIElement e in InternalChildren) {
                var sz = GetCalculatedTabSize(e);
                e.Arrange(new Rect(r, 0.0, sz.Width, sz.Height));
                r += sz.Width;
                }
            return finalSize;
            }
        #endregion
        #region M:MeasureOverride(Size):Size
        protected override Size MeasureOverride(Size availableSize) {
            var c = InternalChildren.Count;
            var index = GetSelectedIndex();
            for (var i = 0; i < c; i++) {
                var e = InternalChildren[i];
                if (e != null) {
                    SetIsFirst(e, (i == 0));
                    SetIsLast(e, (i == c - 1));
                    SetIsImmediatelyBeforeSelection(e, (index >= 0) && (i == index - 1));
                    SetIsImmediatelyAfterSelection(e,  (index >= 0) && (i == index + 1));
                    }
                }
            var H = 0.0;
            var W = UndockingOffset;
            var list = new List<Double>(InternalChildren.Count);
            var sz = new Size(Double.PositiveInfinity, availableSize.Height);
            foreach (UIElement e in InternalChildren) {
                if (UndockingLength != 0.0) {
                    sz.Width = UndockingLength;
                    }
                e.Measure(sz);
                var desiredSize = e.DesiredSize;
                SetCalculatedTabSize(e, desiredSize);
                H = Math.Max(H, desiredSize.Height);
                list.Add(desiredSize.Width);
                W += desiredSize.Width;
                }
            H = Math.Min(availableSize.Height, H);
            if (W > availableSize.Width) {
                SetIsTruncatingTabs(this, true);
                Double τ, ν;
                CalculateTruncationThreshold(list, W - availableSize.Width, out τ, out ν);
                SetUseCompressedTabStyle(this, ν <= 40.0);
                foreach (UIElement e in InternalChildren) {
                    var α = e.DesiredSize;
                    var β = α.Width;
                    if (β >= τ) {
                        β = Math.Max(GetMinimumWidth(e), ν);
                        var size = new Size(β, α.Height);
                        SetCalculatedTabSize(e, size);
                        e.Measure(size);
                        }
                    }
                return new Size(availableSize.Width, H);
                }
            SetIsTruncatingTabs(this, false);
            SetUseCompressedTabStyle(this, false);
            return new Size(W, H);
            }
        #endregion
        }
    }

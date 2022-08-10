using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class XYViewportPanel : ScrollablePanel, IScrollInfo
        {
        static XYViewportPanel()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewportPanel), new FrameworkPropertyMetadata(typeof(XYViewportPanel)));
            }

        #region P:XYViewportPanel.PLeft:Double
        public static readonly DependencyProperty PLeftProperty = DependencyProperty.RegisterAttached("PLeft", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPLeft(DependencyObject element, Double value)
            {
            element.SetValue(PLeftProperty, value);
            }

        public static Double GetPLeft(DependencyObject element)
            {
            return (Double)element.GetValue(PLeftProperty);
            }
        #endregion
        #region P:XYViewportPanel.PRight:Double
        public static readonly DependencyProperty PRightProperty = DependencyProperty.RegisterAttached("Right", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPRight(DependencyObject element, Double value)
            {
            element.SetValue(PRightProperty, value);
            }

        public static Double GetPRight(DependencyObject element)
            {
            return (Double)element.GetValue(PRightProperty);
            }
        #endregion
        #region P:XYViewportPanel.PTop:Double
        public static readonly DependencyProperty PTopProperty = DependencyProperty.RegisterAttached("Top", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPTop(DependencyObject element, Double value)
            {
            element.SetValue(PTopProperty, value);
            }

        public static Double GetPTop(DependencyObject element)
            {
            return (Double)element.GetValue(PTopProperty);
            }
        #endregion
        #region P:XYViewportPanel.PBottom:Double
        public static readonly DependencyProperty PBottomProperty = DependencyProperty.RegisterAttached("Bottom", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPBottom(DependencyObject element, Double value)
            {
            element.SetValue(PBottomProperty, value);
            }

        public static Double GetPBottom(DependencyObject element)
            {
            return (Double)element.GetValue(PBottomProperty);
            }
        #endregion
        #region P:XYViewportPanel.PWidth:Double
        public static readonly DependencyProperty PWidthProperty = DependencyProperty.RegisterAttached("Width", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPWidth(DependencyObject element, Double value)
            {
            element.SetValue(PWidthProperty, value);
            }

        public static Double GetPWidth(DependencyObject element)
            {
            return (Double)element.GetValue(PWidthProperty);
            }
        #endregion
        #region P:XYViewportPanel.PHeight:Double
        public static readonly DependencyProperty PHeightProperty = DependencyProperty.RegisterAttached("Height", typeof(Double), typeof(XYViewportPanel), new PropertyMetadata(Double.NaN));
        public static void SetPHeight(DependencyObject element, Double value)
            {
            element.SetValue(PHeightProperty, value);
            }

        public static Double GetPHeight(DependencyObject element)
            {
            return (Double)element.GetValue(PHeightProperty);
            }
        #endregion

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(XYViewportPanel), new PropertyMetadata(default(Thickness)));

        public Thickness Padding
            {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
            }

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="constraint">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size constraint)
            {
            constraint = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            var rc = new Rect(0.0, 0.0, 0.0, 0.0);
            foreach (UIElement internalChild in InternalChildren) {
                if (internalChild != null) {
                    internalChild.Measure(constraint);
                    var x = 0.0;
                    var y = 0.0;
                    var left = XYViewport.GetLeft(internalChild);
                    if (!DoubleUtil.IsNaN(left))
                        {
                        x = left;
                        }
                    else
                        {
                        var right = XYViewport.GetRight(internalChild);
                        if (!DoubleUtil.IsNaN(right))
                            {
                            x = rc.Width - internalChild.DesiredSize.Width - right;
                            }
                        }
                    var top = XYViewport.GetTop(internalChild);
                    if (!DoubleUtil.IsNaN(top))
                        {
                        y = top;
                        }
                    else
                        {
                        var bottom = XYViewport.GetBottom(internalChild);
                        if (!DoubleUtil.IsNaN(bottom))
                            {
                            y = rc.Height - internalChild.DesiredSize.Height - bottom;
                            }
                        }
                    rc.Union(new Rect(new Point(x, y), internalChild.DesiredSize));
                    }
                }
            var size = rc.Size;
            size.Width  = Math.Max(size.Width  + Padding.Left + Padding.Right, 0);
            size.Height = Math.Max(size.Height + Padding.Top  + Padding.Bottom, 0);
            return size;
            }

        /// <summary>When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class. </summary>
        /// <returns>The actual size used.</returns>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize) {
            foreach (UIElement internalChild in InternalChildren) {
                if (internalChild != null) {
                    var x = 0.0;
                    var y = 0.0;
                    var left = XYViewport.GetLeft(internalChild);
                    if (!DoubleUtil.IsNaN(left))
                        {
                        x = left;
                        }
                    else
                        {
                        var right = XYViewport.GetRight(internalChild);
                        if (!DoubleUtil.IsNaN(right))
                            {
                            x = finalSize.Width - internalChild.DesiredSize.Width - right;
                            }
                        }
                    var top = XYViewport.GetTop(internalChild);
                    if (!DoubleUtil.IsNaN(top))
                        {
                        y = top;
                        }
                    else
                        {
                        var bottom = XYViewport.GetBottom(internalChild);
                        if (!DoubleUtil.IsNaN(bottom))
                            {
                            y = finalSize.Height - internalChild.DesiredSize.Height - bottom;
                            }
                        }
                    internalChild.Arrange(new Rect(new Point(Padding.Left+x, Padding.Top+y), internalChild.DesiredSize));
                    }
                }
            return finalSize;
            }

        public void BeginDragOperation()
            {
            }

        public void EndDragOperation()
            {
            }
        }
    }
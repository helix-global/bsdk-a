using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class XYViewportItemSizeControl : Control
        {
        static XYViewportItemSizeControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewportItemSizeControl), new FrameworkPropertyMetadata(typeof(XYViewportItemSizeControl)));
            }

        #region P:ShowsPreview:Boolean
        public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(Boolean), typeof(XYViewportItemSizeControl), new PropertyMetadata(default(Boolean)));
        public Boolean ShowsPreview
            {
            get { return (Boolean)GetValue(ShowsPreviewProperty); }
            set { SetValue(ShowsPreviewProperty, value); }
            }
        #endregion
        #region P:PreviewStyle:Style
        public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register("PreviewStyle", typeof(Style), typeof(XYViewportItemSizeControl), new PropertyMetadata(default(Style)));
        public Style PreviewStyle
            {
            get { return (Style)GetValue(PreviewStyleProperty); }
            set { SetValue(PreviewStyleProperty, value); }
            }
        #endregion
        #region P:LinkedControl:Control
        public static readonly DependencyProperty LinkedControlProperty = DependencyProperty.Register("LinkedControl", typeof(Control), typeof(XYViewportItemSizeControl), new PropertyMetadata(default(Control)));
        public Control LinkedControl
            {
            get { return (Control)GetValue(LinkedControlProperty); }
            set { SetValue(LinkedControlProperty, value); }
            }
        #endregion

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            InstallThumbHandlers(ThumbRight       = GetTemplateChild("ThumbRight")       as Thumb);
            InstallThumbHandlers(ThumbBottom      = GetTemplateChild("ThumbBottom")      as Thumb);
            InstallThumbHandlers(ThumbRightBottom = GetTemplateChild("ThumbRightBottom") as Thumb);
            InstallThumbHandlers(ThumbTop         = GetTemplateChild("ThumbTop")         as Thumb);
            InstallThumbHandlers(ThumbLeft        = GetTemplateChild("ThumbLeft")        as Thumb);
            InstallThumbHandlers(ThumbRightTop    = GetTemplateChild("ThumbRightTop")    as Thumb);
            InstallThumbHandlers(ThumbLeftBottom  = GetTemplateChild("ThumbLeftBottom")  as Thumb);
            InstallThumbHandlers(ThumbLeftTop     = GetTemplateChild("ThumbLeftTop")     as Thumb);
            InstallThumbHandlers(ThumbAll         = GetTemplateChild("ThumbAll")         as Thumb);
            }

        private void InstallThumbHandlers(Thumb source) {
            if (source != null) {
                source.DragStarted   += OnDragStarted;
                source.DragCompleted += OnDragCompleted;
                source.DragDelta     += OnDragDelta;
                }
            }

        private void OnPreviewDragOver(Object sender, DragEventArgs e)
            {
            Debug.Print("OnPreviewDragOver");
            }

        private void RemovePreviewAdorner() {
            if (Adorner != null) {
                (VisualTreeHelper.GetParent(Adorner) as AdornerLayer).Remove(Adorner);
                Adorner = null;
                }
            }

        private void InvalidateParent() {
            if (Parent != null) {
                if (Parent is UIElement) {
                    ((UIElement)Parent).InvalidateMeasure();
                    }
                }
            }

        private void InvalidateParentPanel() {
            var panel = this.FindAll<XYViewportPanel>(true).FirstOrDefault();
            if (panel != null) {
                panel.InvalidateMeasure();
                }
            }

        private void Arrange(GeometrySelectionAdorner source)
            {
            XYViewportPanel.SetPLeft(LinkedControl,XYViewport.GetLeft(LinkedControl) + source.OffsetX);
            XYViewportPanel.SetPTop(LinkedControl,XYViewport.GetTop(LinkedControl)  + source.OffsetY);
            XYViewportPanel.SetPRight(LinkedControl,Double.NaN);
            XYViewportPanel.SetPBottom(LinkedControl,Double.NaN);
            XYViewportPanel.SetPWidth(LinkedControl,source.Width);
            XYViewportPanel.SetPHeight(LinkedControl,source.Height);
            InvalidateParentPanel();
            }

        private void DoPreviewDragOver()
            {
            }

        #region M:OnDragStarted(Object,DragStartedEventArgs)
        private void OnDragStarted(Object sender, DragStartedEventArgs e) {
            IsDragging = true;
            if (ShowsPreview) {
                DraggingThumb = (Thumb)sender;
                var layer = AdornerLayer.GetAdornerLayer(LinkedControl);
                if (layer != null) {
                    layer.Add(Adorner = new GeometrySelectionAdorner(LinkedControl));
                    Adorner.SelectionStrokeBrush = Brushes.Red;
                    Adorner.SelectionFillBrush   = Brushes.Red;
                    Adorner.OffsetY = 0;
                    }
                ItemsHostPanel = this.FindAll<XYViewportPanel>(true).FirstOrDefault();
                if (ItemsHostPanel != null)
                    {
                    ItemsHostPanel.BeginDragOperation();
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,new Action(DoPreviewDragOver));
                    }
                e.Handled = true;
                }
            }
        #endregion
        #region M:OnDragCompleted(Object,DragCompletedEventArgs)
        private void OnDragCompleted(Object sender, DragCompletedEventArgs e) {
            IsDragging = false;
            if (ShowsPreview) {
                ItemsHostPanel.EndDragOperation();
                LinkedControl.Width  = Adorner.Width;
                LinkedControl.Height = Adorner.Height;
                XYViewport.SetLeft(LinkedControl, XYViewport.GetLeft(LinkedControl) + Adorner.OffsetX);
                XYViewport.SetTop(LinkedControl,  XYViewport.GetTop(LinkedControl)  + Adorner.OffsetY);
                XYViewportPanel.SetPLeft(LinkedControl,Double.NaN);
                XYViewportPanel.SetPTop(LinkedControl,Double.NaN);
                XYViewportPanel.SetPRight(LinkedControl,Double.NaN);
                XYViewportPanel.SetPBottom(LinkedControl,Double.NaN);
                XYViewportPanel.SetPWidth(LinkedControl,Double.NaN);
                XYViewportPanel.SetPHeight(LinkedControl,Double.NaN);
                RemovePreviewAdorner();
                InvalidateParentPanel();
                e.Handled = true;
                }
            }
        #endregion
        #region M:OnDragDelta(Object,DragDeltaEventArgs)
        private void OnDragDelta(Object sender, DragDeltaEventArgs e) {
            e.Handled = true;
            //Debug.Print($"OnDragDelta:{e.HorizontalChange};{e.VerticalChange}");
            if (IsDragging && !ShowsPreview) {
                var left = XYViewport.GetLeft(LinkedControl);
                var top  = XYViewport.GetTop(LinkedControl);
                #region ThumbRight
                if (ReferenceEquals(sender, ThumbRight))
                    {
                    Width = Math.Max(Width + e.HorizontalChange, MinWidth);
                    }
                #endregion
                #region ThumbBottom
                else if (ReferenceEquals(sender, ThumbBottom))
                    {
                    Height = Math.Max(Height + e.VerticalChange, MinHeight);
                    }
                #endregion
                #region ThumbRightBottom
                else if (ReferenceEquals(sender, ThumbRightBottom))
                    {
                    Width  = Math.Max(Width + e.HorizontalChange, MinWidth);
                    Height = Math.Max(Height + e.VerticalChange, MinHeight);
                    }
                #endregion
                #region ThumbTop
                else if (ReferenceEquals(sender, ThumbTop))
                    {
                    Height = Math.Max(Height + e.VerticalChange, MinHeight);
                    Canvas.SetTop(LinkedControl, top + e.VerticalChange);
                    }
                #endregion
                #region ThumbLeft
                else if (ReferenceEquals(sender, ThumbLeft))
                    {
                    Width = Math.Max(Width + e.HorizontalChange, MinWidth);
                    Canvas.SetLeft(LinkedControl, left + e.HorizontalChange);
                    }
                #endregion
                #region ThumbRightTop
                else if (ReferenceEquals(sender, ThumbRightTop))
                    {
                    var height = Height;
                    height -= e.VerticalChange;
                    Height  = Math.Max(height, MinHeight);
                    Width   = Math.Max(Width + e.HorizontalChange, MinWidth);
                    Canvas.SetTop(LinkedControl, top + e.VerticalChange);
                    }
                #endregion
                }
            else if (IsDragging && ShowsPreview) {
                #region ThumbRight
                if (ReferenceEquals(sender, ThumbRight))
                    {
                    Adorner.Width = Math.Max(ActualWidth + e.HorizontalChange, MinWidth);
                    Adorner.Height = ActualHeight;
                    Adorner.InvalidateVisual();
                    }
                #endregion
                #region ThumbBottom
                else if (ReferenceEquals(sender, ThumbBottom))
                    {
                    Adorner.Width = ActualWidth;
                    Adorner.Height = Math.Max(ActualHeight + e.VerticalChange, MinHeight);
                    Adorner.InvalidateVisual();
                    }
                #endregion
                #region ThumbRightBottom
                else if (ReferenceEquals(sender, ThumbRightBottom))
                    {
                    Adorner.Width  = Math.Max(ActualWidth  + e.HorizontalChange, MinWidth);
                    Adorner.Height = Math.Max(ActualHeight + e.VerticalChange, MinHeight);
                    Adorner.InvalidateVisual();
                    }
                #endregion
                #region ThumbLeft
                else if (ReferenceEquals(sender, ThumbLeft))
                    {
                    Adorner.Width = Math.Max(ActualWidth - e.HorizontalChange, MinWidth);
                    Adorner.Height = ActualHeight;
                    Adorner.InvalidateVisual();
                    Adorner.OffsetX = e.HorizontalChange;
                    }
                #endregion
                #region ThumbTop
                else if (ReferenceEquals(sender, ThumbTop))
                    {
                    Adorner.Width = ActualWidth;
                    Adorner.Height = Math.Max(ActualHeight - e.VerticalChange, MinHeight);
                    Adorner.OffsetY = e.VerticalChange;
                    }
                #endregion
                #region ThumbLeftBottom
                else if (ReferenceEquals(sender, ThumbLeftBottom))
                    {
                    Adorner.Width  = Math.Max(ActualWidth  - e.HorizontalChange, MinWidth);
                    Adorner.Height = Math.Max(ActualHeight + e.VerticalChange, MinHeight);
                    Adorner.OffsetX = e.HorizontalChange;
                    }
                #endregion
                #region ThumbLeftTop
                else if (ReferenceEquals(sender, ThumbLeftTop))
                    {
                    Adorner.Width  = Math.Max(ActualWidth  - e.HorizontalChange, MinWidth);
                    Adorner.Height = Math.Max(ActualHeight - e.VerticalChange, MinHeight);
                    Adorner.OffsetX = e.HorizontalChange;
                    Adorner.OffsetY = e.VerticalChange;
                    }
                #endregion
                #region ThumbRightTop
                else if (ReferenceEquals(sender, ThumbRightTop))
                    {
                    Adorner.Width  = Math.Max(ActualWidth  + e.HorizontalChange, MinWidth);
                    Adorner.Height = Math.Max(ActualHeight - e.VerticalChange, MinHeight);
                    Adorner.OffsetY = e.VerticalChange;
                    }
                #endregion
                #region ThumbAll
                else if (ReferenceEquals(sender, ThumbAll))
                    {
                    Adorner.OffsetY = e.VerticalChange;
                    Adorner.OffsetX = e.HorizontalChange;
                    Adorner.Width = ActualWidth;
                    Adorner.Height = ActualHeight;
                    Adorner.InvalidateVisual();
                    //var si = Parent as IScrollInfo;
                    //if (si != null) {
                    //    var owner = si.ScrollOwner;
                    //    var pt = Mouse.GetPosition(owner);
                    //    //if (pt.X >= owner.ActualWidth - 40) {
                    //    si.SetHorizontalOffset(si.HorizontalOffset + e.HorizontalChange);
                    //    //si.SetVerticalOffset(si.VerticalOffset + e.VerticalChange);
                    //        //}
                    //    //si.MakeVisible(Adorner,
                    //    //    new Rect(
                    //    //        Left.GetValueOrDefault() + Adorner.OffsetX*0.5,
                    //    //        Top.GetValueOrDefault() + Adorner.OffsetY*0.5,
                    //    //        Adorner.Width,Adorner.Height)
                    //    //    );
                    //    }
                    ////Adorner.BringIntoView();
                    //InvalidateMeasure();
                    //InvalidateParent();
                    }
                #endregion
                Arrange(Adorner);
                //ItemsHostPanel.DoPreviewDragOver();
                }
            }
        #endregion

        private Thumb ThumbBottom;
        private Thumb ThumbLeft;
        private Thumb ThumbLeftBottom;
        private Thumb ThumbLeftTop;
        private Thumb ThumbRight;
        private Thumb ThumbRightBottom;
        private Thumb ThumbRightTop;
        private Thumb ThumbTop;
        private Thumb ThumbAll;
        private Boolean IsDragging = false;
        private Thumb DraggingThumb;
        private GeometrySelectionAdorner Adorner;
        private XYViewportPanel ItemsHostPanel;
        }
    }

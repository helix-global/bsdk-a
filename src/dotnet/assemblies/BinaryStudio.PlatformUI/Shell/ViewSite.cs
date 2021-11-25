using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell {
    [XamlSerializable]
    public abstract class ViewSite : ViewGroup
        {
        #region P:Child:ViewElement
        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(ViewElement), typeof(ViewSite), new PropertyMetadata(OnChildChanged));
        private static void OnChildChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var viewSite = (ViewSite)obj;
            var newValue = args.NewValue as ViewElement;
            if (newValue == null) {
                if (viewSite.Children.Count <= 0)
                    return;
                viewSite.Children.Clear();
                }
            else if (viewSite.Children.Count == 0) {
                viewSite.Children.Add(newValue);
                }
            else {
                if (Equals(viewSite.Children[0], newValue))
                    return;
                viewSite.Children[0] = newValue;
                }
            }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewElement Child {
            get { return (ViewElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
            }
        #endregion

        protected ViewSite() {
            Children.CollectionChanged += OnChildrenChanged;
            }

        private void OnChildrenChanged(Object sender, NotifyCollectionChangedEventArgs args) {
            if (Children.Count > 1)
                throw new InvalidOperationException("ViewSite does not support multiple children");
            Child = Children.Count > 0 ? Children[0] : null;
            }

        public override Boolean IsChildOnScreen(Int32 childIndex) {
            if (childIndex < 0 || childIndex >= Children.Count)
                throw new ArgumentOutOfRangeException(nameof(childIndex));
            if (IsOnScreen && childIndex == 0)
                return Child.IsVisible;
            return false;
            }

        protected override Boolean GetIsOnScreenCore() {
            return IsVisible;
            }

        public void EnsureOnScreen() {
            Int32 display;
            Rect relativeRect;
            Screen.AbsoluteRectToRelativeRect(GetOnScreenPosition(Display, new Rect(FloatingLeft, FloatingTop, FloatingWidth, FloatingHeight)), out display, out relativeRect);
            Display = display;
            FloatingLeft = relativeRect.Left;
            FloatingTop = relativeRect.Top;
            FloatingWidth = relativeRect.Width;
            FloatingHeight = relativeRect.Height;
            }

        internal static Rect GetOnScreenPosition(Int32 display, Rect floatRect) {
            return GetOnScreenPosition(Screen.RelativeRectToAbsoluteRect(display, floatRect));
            }

        internal static Rect GetOnScreenPosition(Rect floatRect) {
            var rect = floatRect;
            floatRect = DpiHelper.LogicalToDeviceUnits(floatRect);
            Rect screenSubRect;
            Rect monitorRect;
            Screen.FindMaximumSingleMonitorRectangle(floatRect, out screenSubRect, out monitorRect);
            if (screenSubRect.Width == 0.0 || screenSubRect.Height == 0.0) {
                Rect workAreaRect;
                Screen.FindMonitorRectsFromPoint(NativeMethods.GetCursorPos(), out monitorRect, out workAreaRect);
                var logicalUnits = DpiHelper.DeviceToLogicalUnits(workAreaRect);
                if (rect.Width > logicalUnits.Width)
                    rect.Width = logicalUnits.Width;
                if (rect.Height > logicalUnits.Height)
                    rect.Height = logicalUnits.Height;
                if (logicalUnits.Right <= rect.X)
                    rect.X = logicalUnits.Right - rect.Width;
                if (logicalUnits.Left > rect.X + rect.Width)
                    rect.X = logicalUnits.Left;
                if (logicalUnits.Bottom <= rect.Y)
                    rect.Y = logicalUnits.Bottom - rect.Height;
                if (logicalUnits.Top > rect.Y + rect.Height)
                    rect.Y = logicalUnits.Top;
                }
            return rect;
            }

        protected override void UpdateHasAutohiddenViews() {
            var num = 0;
            HasAutohiddenViews = Find((Predicate<AutoHideGroup>)(g => g.IsVisible), num != 0) != null;
            }
        }
    }
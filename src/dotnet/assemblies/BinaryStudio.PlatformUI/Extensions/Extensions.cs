using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Utilities;

namespace BinaryStudio.PlatformUI.Extensions
    {
    public static class Extensions
        {
        public static void Invoke(this Dispatcher source, Action callback, DispatcherPriority priority) {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (callback == null) { throw new ArgumentNullException("callback"); }
            source.Invoke(priority, callback);
            }
        public static void ThrowIfNullOrEmpty(this String value, String message) {
            if (value == null)
                throw new ArgumentNullException(message);
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException(message);
            }

        public static void CopyTo(this Stream sourceStream, Stream targetStream) {
            var buffer = new Byte[4096];
            Int32 count;
            while ((count = sourceStream.Read(buffer, 0, 4096)) > 0)
                targetStream.Write(buffer, 0, count);
            }

        public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement) {
            if (sourceElement == null)
                return null;
            if (sourceElement is Visual)
                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
            return LogicalTreeHelper.GetParent(sourceElement);
            }

        public static TAncestorType FindAncestorOrSelf<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject {
            return obj.FindAncestorOrSelf<TAncestorType, DependencyObject>(GetVisualOrLogicalParent);
            }

        public static TAncestorType FindAncestorOrSelf<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : DependencyObject {
            var ancestorType = (Object)obj as TAncestorType;
            if (ancestorType != null)
                return ancestorType;
            return obj.FindAncestor<TAncestorType, TElementType>(parentEvaluator);
            }

        public static Object FindAncestorOrSelf<TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator, Func<TElementType, Boolean> ancestorSelector) {
            if (ancestorSelector(obj))
                return obj;
            return obj.FindAncestor(parentEvaluator, ancestorSelector);
            }

        public static TAncestorType FindAncestor<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject {
            return obj.FindAncestor<TAncestorType, DependencyObject>(GetVisualOrLogicalParent);
            }

        public static TAncestorType FindAncestor<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : class {
            return obj.FindAncestor(parentEvaluator, ancestor => (Object)ancestor is TAncestorType) as TAncestorType;
            }

        public static Object FindAncestor<TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator, Func<TElementType, Boolean> ancestorSelector) {
            for (var elementType = parentEvaluator(obj); (Object)elementType != null; elementType = parentEvaluator(elementType)) {
                if (ancestorSelector(elementType))
                    return elementType;
                }
            return null;
            }

        public static Boolean IsAncestorOf<TElementType>(this TElementType element, TElementType other, Func<TElementType, TElementType> parentEvaluator) where TElementType : class {
            for (var elementType = parentEvaluator(other); (Object)elementType != null; elementType = parentEvaluator(elementType)) {
                if (elementType == element)
                    return true;
                }
            return false;
            }

        public static Boolean IsLogicalAncestorOf(this DependencyObject element, DependencyObject other) {
            if (other == null)
                return false;
            return element.IsAncestorOf(other, GetVisualOrLogicalParent);
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static Boolean IsDescendantOf(this DependencyObject source, DependencyObject other) {
            if (other != null) {
                return FindDescendant(source, (i)=> Equals(i, other)) != null;
                }
            return false;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DependencyObject FindDescendant(this DependencyObject source, Predicate<DependencyObject> predicate) {
            if (source == null) { return null; }
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(source); ++childIndex) {
                var child = VisualTreeHelper.GetChild(source, childIndex);
                if (child != null) {
                    if (predicate(child))
                        return child;
                    var descendant = child.FindDescendant(predicate);
                    if (descendant != null)
                        return descendant;
                    }
                }
            return null;
            }

        public static T FindDescendant<T>(this DependencyObject source) where T : class {
            if (source == null)
                return default(T);
            var obj1 = default(T);
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(source); ++childIndex) {
                var child = VisualTreeHelper.GetChild(source, childIndex);
                if (child != null) {
                    obj1 = child as T;
                    if (obj1 == null) {
                        obj1 = child.FindDescendant<T>();
                        if (obj1 != null)
                            break;
                        }
                    else
                        break;
                    }
                }
            return obj1;
            }

        public static T FindDescendantReverse<T>(this DependencyObject obj) where T : class {
            if (obj == null)
                return default(T);
            var obj1 = default(T);
            for (var childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex) {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null) {
                    obj1 = child as T;
                    if (obj1 == null) {
                        obj1 = child.FindDescendantReverse<T>();
                        if (obj1 != null)
                            break;
                        }
                    else
                        break;
                    }
                }
            return obj1;
            }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : class {
            if (obj == null)
                return Enumerable.Empty<T>();
            var descendants = new List<T>();
            obj.TraverseVisualTree<T>(child => descendants.Add(child));
            return descendants;
            }

        public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class {
            if (obj == null)
                return;
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex) {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null) {
                    var obj1 = child as T;
                    child.TraverseVisualTreeReverse(action);
                    if (obj1 != null)
                        action(obj1);
                    }
                }
            }
        
        public static IEnumerable<T> FindDescendantsReverse<T>(this DependencyObject obj) where T : class {
            if (obj == null)
                return Enumerable.Empty<T>();
            var descendants = new List<T>();
            obj.TraverseVisualTreeReverse<T>(child => descendants.Add(child));
            return descendants;
            }

        public static void TraverseVisualTreeReverse<T>(this DependencyObject obj, Action<T> action) where T : class {
            if (obj == null)
                return;
            for (var childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex) {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null) {
                    var obj1 = child as T;
                    child.TraverseVisualTreeReverse(action);
                    if (obj1 != null)
                        action(obj1);
                    }
                }
            }

        public static DependencyObject FindCommonAncestor(this DependencyObject obj1, DependencyObject obj2) {
            return obj1.FindCommonAncestor(obj2, GetVisualOrLogicalParent);
            }

        public static T FindCommonAncestor<T>(this T obj1, T obj2, Func<T, T> parentEvaluator) where T : DependencyObject {
            if (obj1 == null || obj2 == null)
                return default(T);
            var objSet = new HashSet<T>();
            for (obj1 = parentEvaluator(obj1); (Object)obj1 != null; obj1 = parentEvaluator(obj1))
                objSet.Add(obj1);
            for (obj2 = parentEvaluator(obj2); (Object)obj2 != null; obj2 = parentEvaluator(obj2)) {
                if (objSet.Contains(obj2))
                    return obj2;
                }
            return default(T);
            }

        public static void AddPropertyChangeHandler<T>(this T instance, DependencyProperty property, EventHandler handler) where T : DependencyObject {
            instance.AddPropertyChangeHandler(property, handler, typeof(T));
            }

        public static void AddPropertyChangeHandler<T>(this T instance, DependencyProperty property, EventHandler handler, Type targetType) where T : DependencyObject {
            DependencyPropertyDescriptor.FromProperty(property, targetType).AddValueChanged(instance, handler);
            }

        public static void RemovePropertyChangeHandler<T>(this T instance, DependencyProperty property, EventHandler handler) where T : DependencyObject {
            instance.RemovePropertyChangeHandler(property, handler, typeof(T));
            }

        public static void RemovePropertyChangeHandler<T>(this T instance, DependencyProperty property, EventHandler handler, Type targetType) where T : DependencyObject {
            DependencyPropertyDescriptor.FromProperty(property, targetType).RemoveValueChanged(instance, handler);
            }

        public static Boolean IsTextTrimmed(this TextBlock textBlock) {
            if (textBlock.TextTrimming == TextTrimming.None)
                return false;
            var textFormattingMode = TextOptions.GetTextFormattingMode(textBlock);
            var typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            var formattedText = new FormattedText(textBlock.Text, CultureInfo.CurrentCulture, textBlock.FlowDirection, typeface, textBlock.FontSize, textBlock.Foreground, new NumberSubstitution(), textFormattingMode);
            if (textBlock.SnapsToDevicePixels || textBlock.UseLayoutRounding || textFormattingMode == TextFormattingMode.Display)
                return (Int32)textBlock.ActualWidth < (Int32)formattedText.Width;
            return LayoutDoubleUtil.LessThan(textBlock.ActualWidth,formattedText.Width);
            }

        //public static Boolean IsTopmost(IntPtr hWnd) {
        //    var pwi = new WINDOWINFO();
        //    pwi.cbSize = Marshal.SizeOf(pwi);
        //    if (!NativeMethods.GetWindowInfo(hWnd, ref pwi))
        //        return false;
        //    return (pwi.dwExStyle & 8) == 8;
        //    }

        public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, Object source, TEventArgs args) where TEventArgs : EventArgs {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        public static void RaiseEvent(this EventHandler eventHandler, Object source) {
            eventHandler.RaiseEvent(source, EventArgs.Empty);
            }

        public static void RaiseEvent(this EventHandler eventHandler, Object source, EventArgs args) {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        public static void RaiseEvent(this CancelEventHandler eventHandler, Object source, CancelEventArgs args) {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        //public static void RaiseEvent(this EventHandler<DependencyPropertyChangedEventArgs> eventHandler, Object source, DependencyPropertyChangedEventArgs args) {
        //    if (eventHandler == null)
        //        return;
        //    eventHandler(source, args);
        //    }

        public static void RaiseEvent(this PropertyChangedEventHandler eventHandler, Object source, PropertyChangedEventArgs args) {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        public static void RaiseEvent(this PropertyChangedEventHandler eventHandler, Object source, String propertyName) {
            if (eventHandler == null)
                return;
            eventHandler(source, new PropertyChangedEventArgs(propertyName));
            }

        public static void RaiseEvent(this PropertyChangingEventHandler eventHandler, Object source, PropertyChangingEventArgs args) {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        public static void RaiseEvent(this PropertyChangingEventHandler eventHandler, Object source, String propertyName) {
            if (eventHandler == null)
                return;
            eventHandler(source, new PropertyChangingEventArgs(propertyName));
            }

        public static void RaiseEvent(this NotifyCollectionChangedEventHandler eventHandler, Object source, NotifyCollectionChangedEventArgs args) {
            if (eventHandler == null)
                return;
            eventHandler(source, args);
            }

        #if !NET40
        public static async Task RaiseEventAsync<T>(this Delegate eventHandler, Object source, T args) {
            if ((Object)eventHandler == null)
                return;
            var delegateArray = eventHandler.GetInvocationList();
            for (var index = 0; index < delegateArray.Length; ++index)
                await (Task)delegateArray[index].DynamicInvoke(source, (Object)args);
            delegateArray = null;
            }
        #endif

        public static Boolean IsNearlyEqual(this Double x, Double y) {
            return LayoutDoubleUtil.AreClose(x, y);
            }

        public static Boolean IsSignificantlyLessThan(this Double x, Double y) {
            return LayoutDoubleUtil.LessThan(x, y);
            }

        public static Boolean IsSignificantlyGreaterThan(this Double x, Double y) {
            return LayoutDoubleUtil.GreaterThan(x, y);
            }

        public static Boolean IsConnectedToPresentationSource(this DependencyObject obj) {
            return PresentationSource.FromDependencyObject(obj) != null;
            }

        //public static Boolean AcquireWin32Focus(this DependencyObject obj, out IntPtr previousFocus) {
        //    var hwndSource = PresentationSource.FromDependencyObject(obj) as HwndSource;
        //    if (hwndSource != null) {
        //        previousFocus = NativeMethods.GetFocus();
        //        if (previousFocus != hwndSource.Handle) {
        //            NativeMethods.SetFocus(hwndSource.Handle);
        //            return true;
        //            }
        //        }
        //    previousFocus = IntPtr.Zero;
        //    return false;
        //    }

        public static Boolean IsClipped(this UIElement element) {
            var scrollContentPresenter = element.FindAncestor<ScrollContentPresenter>();
            if (scrollContentPresenter == null) {
                return false;
                }
            var rect = element.TransformToAncestor(scrollContentPresenter).TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
            var rect2 = new Rect(0.0, 0.0, scrollContentPresenter.RenderSize.Width, scrollContentPresenter.RenderSize.Height);
            rect2.Intersect(rect);
            return !LayoutDoubleUtil.AreClose(rect, rect2);
            }

        public static Boolean IsTrimmed(this UIElement element) {
            var textBlock = element as TextBlock;
            if (textBlock == null)
                return false;
            return textBlock.IsTextTrimmed();
            }

        public static Color ToColorFromArgb(this UInt32 colorValue) {
            return Color.FromArgb((Byte)(colorValue >> 24), (Byte)(colorValue >> 16), (Byte)(colorValue >> 8), (Byte)colorValue);
            }

        public static Color ToColorFromRgba(this UInt32 colorValue) {
            return Color.FromArgb((Byte)(colorValue >> 24), (Byte)colorValue, (Byte)(colorValue >> 8), (Byte)(colorValue >> 16));
            }

        public static UInt32 ToRgba(this Color color) {
            return (UInt32)(color.A << 24 | color.B << 16 | color.G << 8) | color.R;
            }

        public static UInt32 ToArgb(this Color color) {
            return (UInt32)(color.A << 24 | color.R << 16 | color.G << 8) | color.B;
            }

        public static Boolean IsCritical(this Exception ex) {
            if (!(ex is StackOverflowException) && !(ex is AccessViolationException) && (!(ex is AppDomainUnloadedException) && !(ex is BadImageFormatException)))
                return ex is DivideByZeroException;
            return true;
            }

        public static Int32 GetIntAttribute(this XmlReader reader, String attributeName, Int32 defaultValue = 0) {
            Validate.IsNotNullAndNotEmpty(attributeName, "attributeName");
            var attribute = reader.GetAttribute(attributeName);
            Int32 result;
            if (!String.IsNullOrEmpty(attribute) && Int32.TryParse(attribute, out result))
                return result;
            return defaultValue;
            }

        public static Double GetDoubleAttribute(this XmlReader reader, String attributeName, Double defaultValue = 0.0) {
            Validate.IsNotNullAndNotEmpty(attributeName, "attributeName");
            var attribute = reader.GetAttribute(attributeName);
            Double result;
            if (!String.IsNullOrEmpty(attribute) && Double.TryParse(attribute, out result))
                return result;
            return defaultValue;
            }

        public static Guid GetGuidAttribute(this XmlReader reader, String attributeName) {
            Validate.IsNotNullAndNotEmpty(attributeName, "attributeName");
            var attribute = reader.GetAttribute(attributeName);
            Guid result;
            if (!String.IsNullOrEmpty(attribute) && Guid.TryParse(attribute, out result))
                return result;
            return Guid.Empty;
            }

        public static String ToDimensionString(this Size size) {
            if (size.IsEmpty)
                return size.ToString();
            return size.Width.ToString() + "x" + size.Height.ToString();
            }

        private static Char HexDigitToCharLowercase(Byte n) {
            return (Char)(n + ((Int32)n < 10 ? 48 : 87));
            }

        private static void AppendLowerCaseHexDigits(StringBuilder sb, Byte b) {
            sb.Append(HexDigitToCharLowercase((Byte)((UInt32)b >> 4)));
            sb.Append(HexDigitToCharLowercase((Byte)(b & 15U)));
            }

        public static String ToLowercaseString(this Color color) {
            using (var reusableResourceHolder = ReusableStringBuilder.AcquireDefault(9)) {
                var resource = reusableResourceHolder.Resource;
                resource.Append('#');
                AppendLowerCaseHexDigits(resource, color.A);
                AppendLowerCaseHexDigits(resource, color.R);
                AppendLowerCaseHexDigits(resource, color.G);
                AppendLowerCaseHexDigits(resource, color.B);
                return resource.ToString();
                }
            }

        public static String ToLowercaseString(this Color? color) {
            if (!color.HasValue)
                return String.Empty;
            return color.Value.ToLowercaseString();
            }

        public static T GetValueIfCreated<T>(this Lazy<T> lazy) where T : class {
            if (!lazy.IsValueCreated)
                return default(T);
            return lazy.Value;
            }

        #region M:DoAfterLayoutUpdated(UIElement,Action)
        public static void DoAfterLayoutUpdated(this UIElement source,Action predicate) {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }
            EventHandler handler = null;
            handler = delegate {
                predicate.Invoke();
                source.LayoutUpdated -= handler;
                };
            source.LayoutUpdated += handler;
            }
        #endregion
        #region M:DoAfterLayoutUpdated<T>(T,Action<T>)
        public static void DoAfterLayoutUpdated<T>(this T source,Action<T> predicate)
            where T: UIElement {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException("predicate"); }
            EventHandler handler = null;
            handler = delegate {
                predicate.Invoke(source);
                source.LayoutUpdated -= handler;
                };
            source.LayoutUpdated += handler;
            }
        #endregion
        #region M:DoAfterLoaded<T>(T,Action<T>)
        public static void DoAfterLoaded<T>(this T source,Action<T> predicate)
            where T: FrameworkElement {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException("predicate"); }
            RoutedEventHandler handler = null;
            handler = delegate {
                predicate.Invoke(source);
                source.Loaded -= handler;
                };
            source.Loaded += handler;
            }
        #endregion
        #region M:DoAfterLoaded<T>(T,Action)
        public static void DoAfterLoaded<T>(this T source,Action predicate)
            where T: FrameworkElement {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException("predicate"); }
            RoutedEventHandler handler = null;
            handler = delegate {
                predicate.Invoke();
                source.Loaded -= handler;
                };
            source.Loaded += handler;
            }
        #endregion
        #region M:DoAfterPreviewLostKeyboardFocus<T>(T,Action)
        public static void DoAfterPreviewLostKeyboardFocus<T>(this T source,Action predicate)
            where T: FrameworkElement {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException("predicate"); }
            KeyboardFocusChangedEventHandler handler = null;
            handler = delegate {
                predicate.Invoke();
                source.PreviewLostKeyboardFocus -= handler;
                };
            source.PreviewLostKeyboardFocus += handler;
            }
        #endregion
        #region M:DoAfterPreviewLostKeyboardFocus<T>(T,Action<KeyboardFocusChangedEventArgs>)
        public static void DoAfterPreviewLostKeyboardFocus<T>(this T source,Action<KeyboardFocusChangedEventArgs> predicate)
            where T: FrameworkElement {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (predicate == null) { throw new ArgumentNullException("predicate"); }
            KeyboardFocusChangedEventHandler handler = null;
            handler = delegate (Object sender, KeyboardFocusChangedEventArgs e) {
                predicate.Invoke(e);
                source.PreviewLostKeyboardFocus -= handler;
                };
            source.PreviewLostKeyboardFocus += handler;
            }
        #endregion
        #region M:SetFocusAfterLayoutUpdated(Control)
        public static void SetFocusAfterLayoutUpdated(this Control source) {
            if (source == null) { throw new ArgumentNullException("source"); }
            DoAfterLayoutUpdated(source, i => {
                source.Focus();
                });
            }
        #endregion
        #region M:FindAll<T>(DependencyObject,Boolean):IEnumerable<T>
        public static IEnumerable<T> FindAll<T>(this DependencyObject source, Boolean flag = false)
            where T : DependencyObject
            {
            if (source != null) {
                if (flag) {
                    var e = VisualTreeHelper.GetParent(source);
                    while (e != null) {
                        if (e is T) {
                            yield return (T)e;
                            }
                        e = VisualTreeHelper.GetParent(e);
                        }
                    }
                else
                    {
                    var count = VisualTreeHelper.GetChildrenCount(source);
                    for (var i = 0; i < count; i++) {
                        var e = VisualTreeHelper.GetChild(source, i);
                        if (e is T) { yield return (T)e; }
                        foreach (var c in FindAll<T>(e, flag)) {
                            yield return c;
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:FindAll<T>(DependencyObject,Boolean,Boolean):IEnumerable<T>
        public static IEnumerable<T> FindAll<T>(this DependencyObject source, Boolean logical, Boolean flag)
            where T : DependencyObject
            {
            if (source != null) {
                if (logical)
                    {
                    if (flag) {
                        var e = LogicalTreeHelper.GetParent(source);
                        while (e != null) {
                            if (e is T) {
                                yield return (T)e;
                                }
                            e = LogicalTreeHelper.GetParent(e);
                            }
                        }
                    else
                        {
                        foreach (var e in LogicalTreeHelper.GetChildren(source).OfType<DependencyObject>()) {
                            if (e is T) { yield return (T)e; }
                            foreach (var c in FindAll<T>(e, true, false)) {
                                yield return c;
                                }
                            }
                        }
                    }
                else
                    {
                    if (flag) {
                        var e = VisualTreeHelper.GetParent(source);
                        while (e != null) {
                            if (e is T) {
                                yield return (T)e;
                                }
                            e = VisualTreeHelper.GetParent(e);
                            }
                        }
                    else
                        {
                        var count = VisualTreeHelper.GetChildrenCount(source);
                        for (var i = 0; i < count; i++) {
                            var e = VisualTreeHelper.GetChild(source, i);
                            if (e is T) { yield return (T)e; }
                            foreach (var c in FindAll<T>(e, false)) {
                                yield return c;
                                }
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:SetBinding(DependencyObject,DependencyProperty,DependencyObject,DependencyProperty,BindingMode):BindingExpressionBase
        public static BindingExpressionBase SetBinding(this DependencyObject target, DependencyProperty targetProperty, DependencyObject source, DependencyProperty sourceProperty, BindingMode mode) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            if (sourceProperty != null) {
                return BindingOperations.SetBinding(target, targetProperty, new Binding() {
                    Source = source,
                    Path = new PropertyPath(sourceProperty),
                    Mode = mode
                    });
                }
            return null;
            }
        #endregion

        public static void UpdateDataSourceProperty(this FrameworkElement element, String propertyname, Object value)
            {
            element.UpdateDataSourceProperty(propertyname, BuiltInPropertyValue.Create(value));
            }

        public static void UpdateDataSourceProperty(this FrameworkElement element, String propertyname, PropertyValueBase value) {
            if (element.DataContext == null) { return; }
            }

        public static void BindSizeToDataSource(this FrameworkElement source) {
            if (source != null) {
                source.SizeChanged += delegate(Object sender, SizeChangedEventArgs e)
                    {
                    var r = sender as FrameworkElement;
                    if (r.IsConnectedToPresentationSource()) {
                        if (e.WidthChanged)  { r.UpdateDataSourceProperty("Width",  e.NewSize.Width); }
                        if (e.HeightChanged) { r.UpdateDataSourceProperty("Height", e.NewSize.Height); }
                        }
                    };
                source.DataContextChanged += delegate(Object sender, DependencyPropertyChangedEventArgs e)
                    {
                    var r = sender as FrameworkElement;
                    if ((r != null) && (e.NewValue != null)) {
                        r.UpdateDataSourceProperty("Width",  r.ActualWidth);
                        r.UpdateDataSourceProperty("Height", r.ActualHeight);
                        }
                    };
                }
            }
        }
    }
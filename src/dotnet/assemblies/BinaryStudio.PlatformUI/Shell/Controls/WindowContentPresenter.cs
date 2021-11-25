using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class WindowContentPresenter : Decorator
        {
        private Size lastclientsize;

        /**
         * <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="FrameworkElement"/>-derived class.</summary>
         * <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
         * <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
         * */
        protected override Size MeasureOverride(Size availableSize) {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source != null) {
                if (!NativeMethods.IsIconic(source.Handle)) {
                    NativeMethods.GetClientRect(source.Handle, out RECT rc);
                    var width  = rc.Width  * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
                    var height = rc.Height * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
                    lastclientsize = new Size(width, height);
                    }
                return base.MeasureOverride(lastclientsize);
                }
            return base.MeasureOverride(availableSize);
            }
        }
    }
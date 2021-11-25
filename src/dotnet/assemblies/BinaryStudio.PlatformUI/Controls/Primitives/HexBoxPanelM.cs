using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class HexBoxPanelM : Panel
        {
        /// <summary>When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class. </summary>
        /// <returns>The actual size used.</returns>
        /// <param name="finalsize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalsize)
            {
            var υ = 0.0;
            for (var i = 0; i < InternalChildren.Count; i++) {
                var e = InternalChildren[i];
                var ζ = e.DesiredSize.Height;
                e.Arrange(new Rect(0, υ, finalsize.Width, ζ));
                var c = e as FrameworkElement;
                if (c != null) {
                    var context = c.DataContext as Markups.MarkupElement;
                    if (context != null) {
                        context.Update(null, null, Math.Round(υ + ζ * 0.5));
                        }
                    }
                υ += ζ;
                }
            return finalsize;
            }

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availablesize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availablesize) {
            var ξ = 0.0;
            foreach (UIElement e in InternalChildren) {
                e.Measure(availablesize);
                ξ += e.DesiredSize.Height;
                }
            if (ξ > availablesize.Height) {
                if (ξ > 0) {
                    ξ = availablesize.Height / ξ;
                    for (var i = 0; i < InternalChildren.Count; i++) {
                        var e = InternalChildren[i];
                        var ζ = e.DesiredSize.Height * ξ;
                        e.Measure(new Size(availablesize.Width, ζ));
                        }
                    }
                }
            return base.MeasureOverride(availablesize);
            }
        }
    }
using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    public class ViewElementCustomSerializer : DependencyObjectCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return ViewElement.IsSelectedProperty;
                yield return ViewElement.IsVisibleProperty;
                yield return ViewElement.DockedHeightProperty;
                yield return ViewElement.DockedWidthProperty;
                yield return ViewElement.AutoHideWidthProperty;
                yield return ViewElement.AutoHideHeightProperty;
                yield return ViewElement.DisplayProperty;
                yield return ViewElement.FloatingTopProperty;
                yield return ViewElement.FloatingLeftProperty;
                yield return ViewElement.FloatingHeightProperty;
                yield return ViewElement.FloatingWidthProperty;
                yield return ViewElement.FloatingWindowStateProperty;
                yield return ViewElement.DockRestrictionProperty;
                yield return ViewElement.AreDockTargetsEnabledProperty;
                yield return ViewElement.MinimumWidthProperty;
                yield return ViewElement.MinimumHeightProperty;
                }
            }

        public ViewElementCustomSerializer(ViewElement element)
          : base(element)
            {
            }
        }
    }
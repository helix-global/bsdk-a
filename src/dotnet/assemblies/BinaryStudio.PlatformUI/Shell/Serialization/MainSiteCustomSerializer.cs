using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
{
    internal class MainSiteCustomSerializer : ViewGroupCustomSerializer
    {
        protected override IEnumerable<DependencyProperty> SerializableProperties
        {
            get
            {
                yield return ViewElement.DisplayProperty;
                yield return ViewElement.FloatingTopProperty;
                yield return ViewElement.FloatingLeftProperty;
                yield return ViewElement.FloatingHeightProperty;
                yield return ViewElement.FloatingWidthProperty;
                yield return ViewElement.FloatingWindowStateProperty;
            }
        }

        public MainSiteCustomSerializer(MainSite site)
            : base(site)
        {
        }
    }
}
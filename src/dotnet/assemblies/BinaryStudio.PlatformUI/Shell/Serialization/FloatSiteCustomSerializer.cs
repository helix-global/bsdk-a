using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class FloatSiteCustomSerializer : ViewGroupCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return FloatSite.IdProperty;
                foreach (var serializableProperty in base.SerializableProperties)
                    yield return serializableProperty;
                }
            }

        public FloatSiteCustomSerializer(FloatSite site)
          : base(site)
            {
            }
        }
    }
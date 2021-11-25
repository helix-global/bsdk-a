using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class DockGroupCustomSerializer : ViewGroupCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return DockGroup.OrientationProperty;
                foreach (var serializableProperty in base.SerializableProperties)
                    yield return serializableProperty;
                }
            }

        public DockGroupCustomSerializer(DockGroup group)
          : base(@group)
            {
            }
        }
    }
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class AutoHideChannelCustomSerializer : ViewGroupCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return DockPanel.DockProperty;
                yield return AutoHideChannel.OrientationProperty;
                foreach (var serializableProperty in base.SerializableProperties)
                    yield return serializableProperty;
                }
            }

        public AutoHideChannelCustomSerializer(AutoHideChannel channel)
          : base(channel)
            {
            }
        }
    }
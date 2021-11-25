using System.Collections.Generic;
using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Serialization
    {
    internal class ViewBookmarkCustomSerializer : ViewElementCustomSerializer
        {
        protected override IEnumerable<DependencyProperty> SerializableProperties
            {
            get
                {
                yield return ViewBookmark.NameProperty;
                yield return ViewBookmark.AccessOrderProperty;
                yield return ViewBookmark.ViewBookmarkTypeProperty;
                foreach (var serializableProperty in base.SerializableProperties)
                    yield return serializableProperty;
                }
            }

        public ViewBookmarkCustomSerializer(ViewBookmark mark)
          : base(mark)
            {
            }
        }
    }
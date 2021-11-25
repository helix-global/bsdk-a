using System.Collections.Specialized;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class LayoutSynchronizedItemsControl : ItemsControl
        {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            LayoutSynchronizer.Update(this);
            }
        }
    }

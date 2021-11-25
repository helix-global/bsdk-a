using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class HContextMenu : ContextMenu
        {
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new HMenuItem();
            }
        }
    }
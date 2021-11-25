using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class HMenu : Menu
        {
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new HMenuItem();
            }
        }
    }
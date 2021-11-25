using System.Windows;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UIListBox = System.Windows.Controls.ListBox;
    public class ListBox : UIListBox
        {
        /// <summary>Creates or identifies the element used to display a specified item. </summary>
        /// <returns>The element used to display a specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new ListBoxItem();
            }
        }
    }
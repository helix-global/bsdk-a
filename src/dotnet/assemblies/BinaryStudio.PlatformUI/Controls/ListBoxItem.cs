using System.Windows.Input;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UIListBoxItem = System.Windows.Controls.ListBoxItem;
    public class ListBoxItem : UIListBoxItem
        {
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs" /> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
            {
            base.OnLostKeyboardFocus(e);
            IsSelected = false;
            }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs" /> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
            {
            base.OnGotKeyboardFocus(e);
            IsSelected = true;
            }
        }
    }
using System.Windows;
using System.Windows.Input;

namespace BinaryStudio.PlatformUI
    {
    public class CommandSource
        {
        public static readonly RoutedUICommand OpenBinaryData = new RoutedUICommand(nameof(OpenBinaryData),nameof(OpenBinaryData), typeof(CommandSource));
        public static readonly RoutedEvent OpenBinaryDataEvent = EventManager.RegisterRoutedEvent("OpenBinaryData", RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(CommandSource));
        }
    }
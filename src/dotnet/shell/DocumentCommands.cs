using System.Windows.Input;

#pragma warning disable 1591

namespace shell
    {
    public class DocumentCommands
        {
        public static readonly RoutedUICommand ConvertToBase64 = new RoutedUICommand(nameof(ConvertToBase64),nameof(ConvertToBase64), typeof(DocumentCommands));
        public static readonly RoutedUICommand OpenBase64      = new RoutedUICommand(nameof(OpenBase64),nameof(OpenBase64), typeof(DocumentCommands));
        public static readonly RoutedUICommand OpenRegistryKey = new RoutedUICommand(nameof(OpenRegistryKey),nameof(OpenRegistryKey), typeof(DocumentCommands));
        public static readonly RoutedUICommand ObjectIdentifierInfo = new RoutedUICommand(nameof(ObjectIdentifierInfo),nameof(ObjectIdentifierInfo), typeof(DocumentCommands));
        }
    }
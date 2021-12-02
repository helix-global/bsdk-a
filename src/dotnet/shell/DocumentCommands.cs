using System.Windows.Input;

namespace shell
    {
    public class DocumentCommands
        {
        public static readonly RoutedUICommand ConvertToBase64 = new RoutedUICommand(nameof(ConvertToBase64),nameof(ConvertToBase64), typeof(DocumentCommands));    
        }
    }
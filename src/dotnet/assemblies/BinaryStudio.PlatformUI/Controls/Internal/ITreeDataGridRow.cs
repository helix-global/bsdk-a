using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    internal interface ITreeDataGridRow
        {
        ITreeDataGridRow ParentRow { get; }
        ItemContainerGenerator ItemContainerGenerator { get; }
        }
    }
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace BinaryStudio.PlatformUI.Controls
    {
    [ContentProperty(nameof(Templates))]
    [DefaultProperty(nameof(Templates))]
    public class DataGridCellTemplateSelector
        {
        public ObservableCollection<DataGridCellTemplate> Templates { get; }
        public DataGridCellTemplateSelector() {
            Templates = new ObservableCollection<DataGridCellTemplate>();
            }
        }
    }

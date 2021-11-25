using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    using UIDataTemplateSelector = System.Windows.Controls.DataTemplateSelector;
    public class DataTemplateSelector : UIDataTemplateSelector
        {
        public ObservableCollection<DataTemplate> Templates { get; }

        public DataTemplateSelector() {
            Templates = new ObservableCollection<DataTemplate>();
            }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container) {
            if (item != null) {
                foreach (var template in Templates) {
                    if (template.DataType is Type) {
                        var type = (Type)template.DataType;
                        if (type == item.GetType()) { return template; }
                        }
                    }
                }
            return base.SelectTemplate(item, container);
            }
        }
    }
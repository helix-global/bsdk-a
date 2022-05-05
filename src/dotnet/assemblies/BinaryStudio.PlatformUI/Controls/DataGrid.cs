using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UIDataGrid = System.Windows.Controls.DataGrid;
    public class DataGrid : UIDataGrid
        {
        public static StyleKey<DataGrid> DataGridTextColumnElementStyleKey { get; }
        public static StyleKey<DataGrid> DefaultDataGridTextColumnEditingElementStyleKey { get; }
        static DataGrid()
            {
            DataGridTextColumnElementStyleKey = new StyleKey<DataGrid>();
            DefaultDataGridTextColumnEditingElementStyleKey = new StyleKey<DataGrid>();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGrid), new FrameworkPropertyMetadata(typeof(DataGrid)));
            }

        public DataGrid()
            {
            CommandManager.AddCanExecuteHandler(this, OnCanExecuteCommand);
            CommandManager.AddExecutedHandler(this, OnExecutedCommand);
            }

        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(nameof(CellTemplateSelector), typeof(DataGridCellTemplateSelector), typeof(DataGrid), new PropertyMetadata(default(DataGridCellTemplateSelector)));
        public DataGridCellTemplateSelector CellTemplateSelector {
            get { return (DataGridCellTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
            }

        private static void SetResourceReference(DependencyObject target, DependencyProperty targetProperty, Object key) {
            var bridge = new BridgeReference();
            target.SetBinding(targetProperty, bridge, BridgeReference.TargetProperty, BindingMode.OneWay);
            bridge.SetResourceReference(BridgeReference.SourceProperty, key);
            }

        /// <summary>Raises the <see cref="E:System.Windows.Controls.DataGrid.AutoGeneratingColumn"/> event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e) {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor) {
                if (!descriptor.IsBrowsable) {
                    e.Cancel = true;
                    return;
                    }
                }
            var size = CommonUtilities.GetSize(e.Column.Header, e.Column.HeaderTemplate);
            var selector = CellTemplateSelector;
            if (selector != null) {
                foreach (var value in selector.Templates) {
                    if (value.DataType.IsAssignableFrom(e.PropertyType)) {
                        var r = new DataGridTemplateBoundColumn{
                            Header = e.Column.Header,
                            CellTemplate = value.DisplayTemplate,
                            CellEditingTemplate = value.EditTemplate,
                            Binding = new Binding(e.PropertyName),
                            Width = size.Width + SystemParameters.ScrollWidth*2
                            };
                        e.Column = r;
                        break;
                        }
                    }
                }
            base.OnAutoGeneratingColumn(e);
            }

        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (ReferenceEquals(e.Command, CommandSource.OpenBinaryData)) {
                e.CanExecute = e.Parameter is Byte[];
                e.Handled = true;
                }
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (ReferenceEquals(e.Command, CommandSource.OpenBinaryData)) {
                RaiseEvent(new OpenBinaryDataEventArgs(CommandSource.OpenBinaryDataEvent){
                    Data = e.Parameter as Byte[]
                    });
                e.Handled = true;
                }
            }
        #endregion
        }
    }
using System.Windows;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridTextColumn : TreeDataGridBoundColumn
        {
        private static ResourceKey DefaultETemplateKeyInternal;
        public static ResourceKey DefaultETemplateKey { get{
            return DefaultETemplateKeyInternal = DefaultETemplateKeyInternal??new ComponentResourceKey(typeof(TreeDataGridTextColumn), nameof(DefaultETemplateKey));
            }}

        private static ResourceKey DefaultDTemplateKeyInternal;
        public static ResourceKey DefaultDTemplateKey { get{
            return DefaultDTemplateKeyInternal = DefaultDTemplateKeyInternal??new ComponentResourceKey(typeof(TreeDataGridTextColumn), nameof(DefaultDTemplateKey));
            }}
        }
    }
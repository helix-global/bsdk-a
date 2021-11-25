using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
    {
    public class Asn1GridEntryColumnDefinition : DependencyObject
        {
        #region P:Width:GridLength
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(Asn1GridEntryColumnDefinition), new PropertyMetadata(default(GridLength)));
        public GridLength Width
            {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
            }
        #endregion
        #region P:Definition:ColumnDefinition
        public static readonly DependencyProperty DefinitionProperty = DependencyProperty.Register("Definition", typeof(ColumnDefinition), typeof(Asn1GridEntryColumnDefinition), new PropertyMetadata(default(ColumnDefinition)));
        public ColumnDefinition Definition
            {
            get { return (ColumnDefinition)GetValue(DefinitionProperty); }
            set { SetValue(DefinitionProperty, value); }
            }
        #endregion
        }
    }
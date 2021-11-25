using System;
using System.Windows;
using DataFormat = BinaryStudio.PlatformUI.Controls.Primitives.DataFormat;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
    {
    internal class Asn1GridEntryCommandParameter : DependencyObject
        {
        #region P:DataFormat:DataFormat
        public static readonly DependencyProperty DataFormatProperty = DependencyProperty.Register("DataFormat", typeof(DataFormat), typeof(Asn1GridEntryCommandParameter), new PropertyMetadata(default(DataFormat)));
        public DataFormat DataFormat
            {
            get { return (DataFormat)GetValue(DataFormatProperty); }
            set { SetValue(DataFormatProperty, value); }
            }
        #endregion
        #region P:Value:Object
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(Asn1GridEntryCommandParameter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
        public Object Value
            {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
            }
        #endregion
        }
    }
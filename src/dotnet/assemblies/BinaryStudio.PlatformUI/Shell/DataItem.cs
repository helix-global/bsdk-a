using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    public class DataItem : DependencyObject, IEnumerable<Object>
        {
        public ObservableCollection<Object> Items { get; }

        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(DataItem), new PropertyMetadata(default(Object)));
        public Object Content {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        #region P:DataItem.Header:Object
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(Object), typeof(DataItem), new PropertyMetadata(default(Object)));
        public static void SetHeader(DependencyObject source, Object value) {
            if (source == null) { throw new ArgumentNullException("source"); }
            source.SetValue(HeaderProperty, value);
            }

        public static Object GetHeader(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException("source"); }
            return (Object)source.GetValue(HeaderProperty);
            }
        public Object Header
            {
            get { return (Object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
            }
        #endregion

        public DataItem()
            {
            Items = new ObservableCollection<Object>();
            }

        public DataItem(Object header, Object content)
            :this(header)
            {
            Content = content;
            }

        public DataItem(Object header)
            :this()
            {
            Header = header;
            }

        IEnumerator<Object> IEnumerable<Object>.GetEnumerator() { return Items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return Items.GetEnumerator(); }
        }
    }
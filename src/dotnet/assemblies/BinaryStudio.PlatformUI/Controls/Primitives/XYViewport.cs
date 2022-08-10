using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    [ContentProperty("Children")]
    [DefaultProperty("Children")]
    public class XYViewport : Control
        {
        static XYViewport()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewport), new FrameworkPropertyMetadata(typeof(XYViewport)));
            }

        public XYViewport()
            {
            Children = new ObservableCollection<Object>();
            Children.CollectionChanged += OnChildrenCollectionChanged;
            }

        private void OnChildrenCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
            {
            }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Bindable(true)]
        public ObservableCollection<Object> Children { get; }
        #region P:ItemsSource:IEnumerable
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(XYViewport), new PropertyMetadata(default(IEnumerable),OnItemsSourceChanged));
        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is XYViewport source) {
                source.OnItemsSourceChanged(e);
                }
            }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is INotifyCollectionChanged o) { o.CollectionChanged -= OnSourceCollectionChanged; }
            if (e.NewValue is INotifyCollectionChanged n) { n.CollectionChanged += OnSourceCollectionChanged; }
            Children.Clear();
            if (ItemsSource != null) {
                foreach (var i in ItemsSource) {
                    Children.Add(i);
                    }
                }
            }

        private void OnSourceCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
            {
            }

        public IEnumerable ItemsSource
            {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
            }
        #endregion
        #region M:XYViewport.Left:Double
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(Double), typeof(XYViewport), new PropertyMetadata(default(Double)));
        public static void SetLeft(DependencyObject source, Double value)
            {
            source.SetValue(LeftProperty, value);
            }

        public static Double GetLeft(DependencyObject source)
            {
            return (Double)source.GetValue(LeftProperty);
            }
        #endregion
        #region M:XYViewport.Top:Double
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(Double), typeof(XYViewport), new PropertyMetadata(default(Double)));
        public static void SetTop(DependencyObject source, Double value)
            {
            source.SetValue(TopProperty, value);
            }

        public static Double GetTop(DependencyObject source)
            {
            return (Double)source.GetValue(TopProperty);
            }
        #endregion
        #region M:XYViewport.Right:Double
        public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached("Right", typeof(Double), typeof(XYViewport), new PropertyMetadata(default(Double)));
        public static void SetRight(DependencyObject source, Double value)
            {
            source.SetValue(RightProperty, value);
            }

        public static Double GetRight(DependencyObject source)
            {
            return (Double)source.GetValue(RightProperty);
            }
        #endregion
        #region M:XYViewport.Bottom:Double
        public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached("Bottom", typeof(Double), typeof(XYViewport), new PropertyMetadata(default(Double)));
        public static void SetBottom(DependencyObject source, Double value)
            {
            source.SetValue(BottomProperty, value);
            }

        public static Double GetBottom(DependencyObject source)
            {
            return (Double)source.GetValue(BottomProperty);
            }
        #endregion
        }
    }

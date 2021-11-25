using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BinaryStudio.DataProcessing.Annotations;

namespace BinaryStudio.PlatformUI
{
    public class HierarchicalPresenter : DependencyObject, IEnumerable, INotifyPropertyChanged
        {
        #region M:OnPropertyChanged(String)
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        #endregion
        #region M:OnPropertyChanged(DependencyPropertyChangedEventArgs)
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }
        #endregion

        public virtual Object Source { get;protected set; }

        #region P:Header:Object
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(Object), typeof(HierarchicalPresenter), new PropertyMetadata(default(Object)));
        public Object Header {
            get { return (Object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
            }
        #endregion
        #region P:HeaderTemplate:DataTemplate
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HierarchicalPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate HeaderTemplate {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
            }
        #endregion
        #region P:ItemsSource:IEnumerable
        public virtual IEnumerable ItemsSource { get {
            yield break;
            }}
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(HierarchicalPresenter), new PropertyMetadata(default(Boolean), OnIsSelectedChanged));
        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HierarchicalPresenter);
            if (source != null) {
                source.OnIsSelectedChanged();
                }
            }

        private void OnIsSelectedChanged()
            {
            }

        public Boolean IsSelected {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion

        public IEnumerator GetEnumerator() { return ItemsSource.GetEnumerator(); }
        }

    public abstract class HierarchicalPresenter<T> : HierarchicalPresenter
        {
        public new T Source { get; }
        protected HierarchicalPresenter(T source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Source = source;
            base.Source = source;
            }
        }
    }
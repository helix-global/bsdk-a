using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace BinaryStudio.PlatformUI
    {
    public abstract class NotifyPropertyChangedDispatcherObject : INotifyPropertyChanged
        {
        private class CollectionChangedObject
            {
            private INotifyCollectionChanged source;
            private NotifyPropertyChangedDispatcherObject host;
            private String propertyname;

            public CollectionChangedObject(NotifyPropertyChangedDispatcherObject host, INotifyCollectionChanged source, String propertyname) {
                this.source = source;
                this.host = host;
                this.propertyname = propertyname;
                source.CollectionChanged += OnCollectionChanged;
                }

            private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
                {
                host.OnPropertyChanged(propertyname);
                }

            ~CollectionChangedObject()
                {
                source.CollectionChanged -= OnCollectionChanged;
                }
            }

        public Dispatcher Dispatcher { get; }
        public Boolean InvokeRequired { get { return Dispatcher.Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId; }}

        //#region M:OnCollectionChanged(Object,NotifyCollectionChangedEventArgs)
        //private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        //    {
        //    throw new NotImplementedException();
        //    }
        //#endregion
        #region M:OnPropertyChanged(String)
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyname = null) {
            var handler = PropertyChanged;
            if (handler != null) {
                var e = new PropertyChangedEventArgs(propertyname);
                if (InvokeRequired) {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(()=>{
                        handler.Invoke(this, e);
                        }));
                    }
                else
                    {
                    handler.Invoke(this, e);
                    }
                }
            }
        #endregion
        #region M:SetValue<T>(ref T,T,String):Boolean
        protected Boolean SetValue<T>(ref T field, T value, [CallerMemberName] String propertyName = null) {
            var r = true;
            var equatable = value as IEquatable<T>;
            if (equatable != null) {
                r = equatable.Equals(field);
                }
            else if (typeof(T).IsSubclassOf(typeof(Enum)))
                {
                r = Equals(value, field);
                }
            else
                {
                r = Equals(value, field);
                }
            if (!r)
                {
                RemoceCollectionChangedHandler(propertyName, field as INotifyCollectionChanged);
                field = value;
                AddCollectionChangedHandler(propertyName, field as INotifyCollectionChanged);
                OnPropertyChanged(propertyName);
                }
            return !r;
            }
        #endregion
        #region M:AddCollectionChangedHandler(String,INotifyCollectionChanged)
        private void AddCollectionChangedHandler(String propertyname, INotifyCollectionChanged value) {
            if (value != null) {
                map[value] = new CollectionChangedObject(this, value, propertyname);
                }
            }
        #endregion
        #region M:RemoceCollectionChangedHandler(String,INotifyCollectionChanged)
        private void RemoceCollectionChangedHandler(String propertyname, INotifyCollectionChanged value) {
            if (value != null) {
                map.Remove(propertyname);
                }
            }
        #endregion

        protected NotifyPropertyChangedDispatcherObject()
            :this(Dispatcher.CurrentDispatcher)
            {
            }

        protected NotifyPropertyChangedDispatcherObject(Dispatcher dispatcher)
            {
            if (dispatcher == null) { throw new ArgumentNullException(nameof(dispatcher)); }
            Dispatcher = dispatcher;
            }

        private readonly IDictionary<Object, CollectionChangedObject> map = new Dictionary<Object, CollectionChangedObject>();
        }
    }
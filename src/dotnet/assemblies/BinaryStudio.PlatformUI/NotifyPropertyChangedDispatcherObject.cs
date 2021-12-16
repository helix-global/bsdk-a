using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace BinaryStudio.PlatformUI
    {
    public abstract class NotifyPropertyChangedDispatcherObject : INotifyPropertyChanged, ICustomTypeDescriptor
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

        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetClassName:String
        String ICustomTypeDescriptor.GetClassName()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetComponentName:String
        String ICustomTypeDescriptor.GetComponentName()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        /// <summary>Returns a type converter for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or <see langword="null"/> if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.</returns>
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            return TypeDescriptor.GetConverter(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultEvent:EventDescriptor
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        Object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents):EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        /// <summary>Returns the properties for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties for this component instance.</returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
            return new PropertyDescriptorCollection(GetProperties().ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        /// <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.</returns>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
            return new PropertyDescriptorCollection(GetProperties(attributes).ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor):Object
        /// <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
        /// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
        /// <returns>An <see cref="T:System.Object"/> that represents the owner of the specified property.</returns>
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return GetPropertyOwner(descriptor);
            }
        #endregion

        #region M:GetProperties:IEnumerable<PropertyDescriptor>
        protected virtual IEnumerable<PropertyDescriptor> GetProperties() {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(GetType())) {
                yield return descriptor;
                }
            }
        #endregion
        #region M:GetProperties(Attribute[]):IEnumerable<PropertyDescriptor>
        protected virtual IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(GetType(), attributes)) {
                yield return descriptor;
                }
            }
        #endregion
        #region M:GetPropertyOwner(PropertyDescriptor):Object
        protected virtual Object GetPropertyOwner(PropertyDescriptor descriptor) {
            return this;
            }
        #endregion
        #region M:GetConverter:TypeConverter
        /// <summary>Returns a type converter for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or <see langword="null"/> if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.</returns>
        protected virtual TypeConverter GetConverter()
            {
            return TypeDescriptor.GetConverter(GetType());
            }
        #endregion
        }

    public abstract class NotifyPropertyChangedDispatcherObject<T> : NotifyPropertyChangedDispatcherObject
        {
        public T Source { get; }
        protected NotifyPropertyChangedDispatcherObject(T source) {
            Source = source;
            }

        #region M:GetProperties(Attribute[]):IEnumerable<PropertyDescriptor>
        protected override IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source, attributes)) {
                yield return descriptor;
                }
            }
        #endregion
        #region M:GetProperties():IEnumerable<PropertyDescriptor>
        protected override IEnumerable<PropertyDescriptor> GetProperties() {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source)) {
                yield return descriptor;
                }
            }
        #endregion
        #region M:GetPropertyOwner(PropertyDescriptor):Object
        protected override Object GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return Source;
            }
        #endregion

        /// <summary>Returns a type converter for this instance of a component.</summary>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter"/> that is the converter for this object, or <see langword="null"/> if there is no <see cref="T:System.ComponentModel.TypeConverter"/> for this object.</returns>
        protected override TypeConverter GetConverter()
            {
            return TypeDescriptor.GetConverter(Source);
            }
        }
    }
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using BinaryStudio.DataProcessing;
using BinaryStudio.DataProcessing.Annotations;
using Image = System.Windows.Controls.Image;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    public class GridEntry : DependencyObject, INotifyPropertyChanged, IEnumerable<GridEntry>, ITypeDescriptorContext
        {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Boolean notify = true;
        private class DisablePropertyChanged : IDisposable
            {
            private GridEntry source;
            public DisablePropertyChanged(GridEntry source) {
                this.source = source;
                source.notify = false;
                }

            public void Dispose() {
                if (source != null) {
                    source.notify = true;
                    source = null;
                    }
                }
            }

        public GridEntry(PropertyDescriptor descriptor, Object component, Int32 level, PropertyGridControl owner) {
            if (descriptor == null) { throw new ArgumentNullException(nameof(descriptor)); }
            if (component == null) { throw new ArgumentNullException(nameof(component)); }
            if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
            PropertyDescriptor = descriptor;
            Name = descriptor.Name;
            Value = descriptor.GetValue(component);
            Instance = component;
            Owner = owner;
            Level = level;
            IsExpanded = owner.IsDefaultExpanded;
            }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public Int32 Level { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public PropertyGridControl Owner { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public TypeConverter Converter { get;private set; }
        public String Name { get; }

        #region P:Margin:Thickness
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Thickness Margin { get {
            return new Thickness(
                Level*16,
                3,
                5,0);
            }}
        #endregion
        #region P:PropertyDescriptor:PropertyDescriptor
        private static readonly DependencyPropertyKey PropertyDescriptorPropertyKey = DependencyProperty.RegisterReadOnly("PropertyDescriptor", typeof(PropertyDescriptor), typeof(GridEntry), new PropertyMetadata(default(PropertyDescriptor), OnPropertyDescriptorChanged));
        private static void OnPropertyDescriptorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((GridEntry)sender).OnPropertyDescriptorChanged();
            }

        private void OnPropertyDescriptorChanged() {
            if (PropertyDescriptor != null) {
                IsReadOnly = PropertyDescriptor.IsReadOnly;
                IsCompositeDescriptor = PropertyDescriptor is CompositePropertyDescriptor;
                DisplayName = PropertyDescriptor.DisplayName;
                }
            TypeEditor = GetEditor(PropertyDescriptor, Value);
            Converter = GetConverter(PropertyDescriptor, Value);
            }

        public static readonly DependencyProperty PropertyDescriptorProperty = PropertyDescriptorPropertyKey.DependencyProperty;
        public PropertyDescriptor PropertyDescriptor {
            get { return (PropertyDescriptor)GetValue(PropertyDescriptorProperty); }
            private set { SetValue(PropertyDescriptorPropertyKey, value); }
            }
        #endregion
        #region P:DisplayName:String
        private static readonly DependencyPropertyKey DisplayNamePropertyKey = DependencyProperty.RegisterReadOnly("DisplayName", typeof(String), typeof(GridEntry), new PropertyMetadata(default(String)));
        public static readonly DependencyProperty DisplayNameProperty = DisplayNamePropertyKey.DependencyProperty;
        public String DisplayName {
            get { return (String)GetValue(DisplayNameProperty); }
            private set { SetValue(DisplayNamePropertyKey, value); }
            }
        #endregion
        #region P:Value:Object
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(GridEntry), new PropertyMetadata(default(Object), OnValueChanged));
        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as GridEntry);
            if (source != null) {
                source.OnValueChanged();
                }
            }

        private void OnValueChanged() {
            var value = Value;
            TypeEditor = GetEditor(PropertyDescriptor, value);
            var conveter = Converter = GetConverter(PropertyDescriptor, value);
            if (conveter.CanConvertTo(this, typeof(String))) {
                GridValue = conveter.ConvertToString(this, value);
                }
            else
                {
                GridValue = (value != null)
                    ? value.ToString()
                    : String.Empty;
                }
            }

        public Object Value {
            get { return (Object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
            }
        #endregion

        //#region P:Value:Object
        //public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(GridEntry), new PropertyMetadata(default(Object), OnValueChanged));
        //private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
        //    ((GridEntry)sender).OnValueChanged();
        //    }

        //private void OnValueChanged() {
        //    var value = Value;
        //    if ((value != null) && (PropertyDescriptor != null)) {
        //        if (IsPaintValueSupported && (TypeEditor != null)) {
        //            var bitmap = new Bitmap(20,14);
        //            using (var graphics = Graphics.FromImage(bitmap)) {
        //                TypeEditor.PaintValue(value, graphics, new Rectangle(0,0,20,14));
        //                }
        //            PaintValueImage = new Image {Source = ToBitmapSource(bitmap)};
        //            }
        //        else
        //            {
        //            PaintValueImage = null;
        //            }
        //        try
        //            {
        //            var r = (value != null)
        //                ? value.ToString()
        //                : String.Empty;
        //            if (!PropertyDescriptor.IsReadOnly) {
        //                var converter = PropertyDescriptor.Converter;
        //                if (converter != null) {
        //                    var type = value.GetType();
        //                    if (converter.CanConvertFrom(this, type)) {
        //                        value = converter.ConvertFrom(this, CultureInfo.CurrentUICulture, value);
        //                        }
        //                    }
        //                PropertyDescriptor.SetValue(Instance, value);
        //                }
        //            if (PropertyDescriptor != null) {
        //                var converter = GetConverter(PropertyDescriptor, value);
        //                if (converter != null) {
        //                    if (converter.CanConvertTo(this, typeof(String))) {
        //                        r = converter.ConvertToString(this, value);
        //                        }
        //                    }
        //                }
        //            GridValue = r;
        //            }
        //        catch (Exception e)
        //            {
        //            var r = (MessageBox.Show(e.Message, "Error", MessageBoxButton.OKCancel));
        //            if (r == MessageBoxResult.Cancel) {
        //                ResetValue();
        //                return;
        //                }
        //            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(()=> {
        //                Owner.Edit(this);
        //                }));
        //            }
        //        }
        //    else
        //        {
        //        Style = GridEntryStyle.TextBox;
        //        PaintValueImage = null;
        //        PropertyDescriptor.SetValue(Instance, null);
        //        }
        //    }

        //private void ResetValue() {
        //    Value = (PropertyDescriptor != null)
        //        ? PropertyDescriptor.GetValue(Instance)
        //        : null;
        //    }

        //public Object Value {
        //    get { return (Object)GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //    }
        //#endregion
        #region P:IsCompositeDescriptor:Boolean
        private static readonly DependencyPropertyKey IsCompositeDescriptorPropertyKey = DependencyProperty.RegisterReadOnly("IsCompositeDescriptor", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsCompositeDescriptorProperty = IsCompositeDescriptorPropertyKey.DependencyProperty;
        public Boolean IsCompositeDescriptor {
            get { return (Boolean)GetValue(IsCompositeDescriptorProperty); }
            private set { SetValue(IsCompositeDescriptorPropertyKey, value); }
            }
        #endregion
        #region P:IsPaintValueSupported:Boolean
        private static readonly DependencyPropertyKey IsPaintValueSupportedPropertyKey = DependencyProperty.RegisterReadOnly("IsPaintValueSupported", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsPaintValueSupportedProperty = IsPaintValueSupportedPropertyKey.DependencyProperty;
        public Boolean IsPaintValueSupported {
            get { return (Boolean)GetValue(IsPaintValueSupportedProperty); }
            private set { SetValue(IsPaintValueSupportedPropertyKey, value); }
            }
        #endregion
        #region P:IsReadOnly:Boolean
        private static readonly DependencyPropertyKey IsReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly("IsReadOnly", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsReadOnlyProperty = IsReadOnlyPropertyKey.DependencyProperty;
        public Boolean IsReadOnly {
            get { return (Boolean)GetValue(IsReadOnlyProperty); }
            private set { SetValue(IsReadOnlyPropertyKey, value); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean), OnIsSelectedChanged));
        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (GridEntry)sender;
            source.OnIsSelectedChanged();
            }

        private void OnIsSelectedChanged() {
            if (IsSelected) {
                Owner.SelectedPropertyDescriptor = PropertyDescriptor;
                Owner.SelectedValue = Value;
                }
            }

        public Boolean IsSelected {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion
        #region P:IsExpanded:Boolean
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        public Boolean IsExpanded
            {
            get { return (Boolean)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
            }
        #endregion
        #region P:PaintValueImage:Image
        private static readonly DependencyPropertyKey PaintValueImagePropertyKey = DependencyProperty.RegisterReadOnly("PaintValueImage", typeof(Image), typeof(GridEntry), new PropertyMetadata(default(Image)));
        public static readonly DependencyProperty PaintValueImageProperty = PaintValueImagePropertyKey.DependencyProperty;
        public Image PaintValueImage {
            get { return (Image)GetValue(PaintValueImageProperty); }
            private set { SetValue(PaintValueImagePropertyKey, value); }
            }
        #endregion
        #region P:Style:GridEntryStyle
        private static readonly DependencyPropertyKey StylePropertyKey = DependencyProperty.RegisterReadOnly("Style", typeof(GridEntryStyle), typeof(GridEntry), new PropertyMetadata(default(GridEntryStyle)));
        public static readonly DependencyProperty StyleProperty = StylePropertyKey.DependencyProperty;
        public GridEntryStyle Style {
            get { return (GridEntryStyle)GetValue(StyleProperty); }
            private set { SetValue(StylePropertyKey, value); }
            }
        #endregion
        #region P:StandardValues:Object[]
        private static readonly DependencyPropertyKey StandardValuesPropertyKey = DependencyProperty.RegisterReadOnly("StandardValues", typeof(Object[]), typeof(GridEntry), new PropertyMetadata(default(Object[])));
        public static readonly DependencyProperty StandardValuesProperty = StandardValuesPropertyKey.DependencyProperty;
        public Object[] StandardValues {
            get { return (Object[])GetValue(StandardValuesProperty); }
            private set { SetValue(StandardValuesPropertyKey, value); }
            }
        #endregion
        #region P:TypeEditor:UITypeEditor
        private static readonly DependencyPropertyKey TypeEditorPropertyKey = DependencyProperty.RegisterReadOnly("TypeEditor", typeof(UITypeEditor), typeof(GridEntry), new PropertyMetadata(default(UITypeEditor), OnTypeEditorChanged));
        public static readonly DependencyProperty TypeEditorProperty = TypeEditorPropertyKey.DependencyProperty;
        private static void OnTypeEditorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as GridEntry);
            if (source != null) {
                source.OnTypeEditorChanged();
                }
            }

        private void OnTypeEditorChanged() {
            var editor = TypeEditor;
            var style = GridEntryStyle.TextBox;
            if (editor != null) {
                var ui = editor.GetEditStyle(this);
                if (ui != UITypeEditorEditStyle.None) {
                    style = (ui == UITypeEditorEditStyle.DropDown)
                        ? GridEntryStyle.DropDown
                        : GridEntryStyle.Modal;
                    }
                IsPaintValueSupported = editor.GetPaintValueSupported(this);
                }
            else
                {
                IsPaintValueSupported = false;
                }
            Style = style;
            }

        public UITypeEditor TypeEditor {
            get { return (UITypeEditor)GetValue(TypeEditorProperty); }
            private set { SetValue(TypeEditorPropertyKey, value); }
            }
        #endregion
        #region P:GridValue:String
        public static readonly DependencyProperty GridValueProperty = DependencyProperty.Register("GridValue", typeof(String), typeof(GridEntry), new PropertyMetadata(default(String), OnGridValueChanged));
        private static void OnGridValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as GridEntry);
            if (source != null) {
                source.OnGridValueChanged();
                }
            }

        private void OnGridValueChanged()
            {
            }

        public String GridValue {
            get { return (String)GetValue(GridValueProperty); }
            set { SetValue(GridValueProperty, value); }
            }
        #endregion

        #region M:GetConverter(PropertyDescriptor,Object):TypeConverter
        private static TypeConverter GetConverter(PropertyDescriptor descriptor, Object value) {
            if (descriptor != null) {
                var attribute = descriptor.Attributes.OfType<TypeConverterAttribute>().FirstOrDefault();
                if ((attribute != null) || (value == null)) {
                    return descriptor.Converter;
                    }
                }
            return TypeDescriptor.GetConverter(value);
            }
        #endregion
        #region M:GetEditor(PropertyDescriptor,Object):UITypeEditor
        private static UITypeEditor GetEditor(PropertyDescriptor descriptor, Object value) {
            Object r = null;
            var type = typeof(UITypeEditor);
            if (descriptor != null) {
                var attribute = descriptor.Attributes.OfType<EditorAttribute>().FirstOrDefault();
                if ((attribute != null) || (value == null)) {
                    r = descriptor.GetEditor(type);
                    }
                if ((r == null) && (value == null))
                    {
                    r = TypeDescriptor.GetEditor(descriptor.PropertyType, type);
                    }
                }
            if ((r == null) && (value != null)) {
                var provider = value as ICustomTypeDescriptor;
                if (provider != null) { return (UITypeEditor)provider.GetEditor(type); }
                r = TypeDescriptor.GetEditor(value, type);
                }
            return r as UITypeEditor;
            }
        #endregion

        //public Boolean OnComponentChanging()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnComponentChanged()
        //{
        //    throw new NotImplementedException();
        //}

        //public IContainer Container { get; }
        //public Object Instance {get; private set;}
        //public PropertyDescriptor PropertyDescriptor { get; }
        //public PropertyGridControl Owner { get;set; }
        //#region P:DisplayName:String
        //private static readonly DependencyPropertyKey DisplayNamePropertyKey = DependencyProperty.RegisterReadOnly("DisplayName", typeof(String), typeof(GridEntry), new PropertyMetadata(null));
        //public static readonly DependencyProperty DisplayNameProperty= DisplayNamePropertyKey.DependencyProperty;
        //public String DisplayName {
        //    get { return (String)GetValue(DisplayNameProperty); }
        //    private set { SetValue(DisplayNamePropertyKey, value); }
        //    }
        //#endregion
        //#region P:FontSize:Double
        //private static readonly DependencyPropertyKey FontSizePropertyKey = DependencyProperty.RegisterReadOnly("FontSize", typeof(Double), typeof(GridEntry), new PropertyMetadata(default(Double)));
        //public static readonly DependencyProperty FontSizeProperty= FontSizePropertyKey.DependencyProperty;
        //public Double FontSize {
        //    get { return (Double)GetValue(FontSizeProperty); }
        //    private set { SetValue(FontSizePropertyKey, value); }
        //    }
        //#endregion
        //#region P:IsSelected:Boolean
        //public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean),typeof(GridEntry),new FrameworkPropertyMetadata(false));
        //public Boolean IsSelected {
        //    get { return (Boolean)GetValue(IsSelectedProperty); }
        //    set { SetValue(IsSelectedProperty, value); }
        //    }
        //#endregion
        //#region P:IsAttached:Boolean
        //private static readonly DependencyPropertyKey IsAttachedPropertyKey = DependencyProperty.RegisterReadOnly("IsAttached", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        //public static readonly DependencyProperty IsAttachedProperty= IsAttachedPropertyKey.DependencyProperty;
        //public Boolean IsAttached {
        //    get { return (Boolean)GetValue(IsAttachedProperty); }
        //    private set { SetValue(IsAttachedPropertyKey, value); }
        //    }
        //#endregion
        //#region P:IsIndexed:Boolean
        //private static readonly DependencyPropertyKey IsIndexedPropertyKey = DependencyProperty.RegisterReadOnly("IsIndexed", typeof(Boolean), typeof(GridEntry), new PropertyMetadata(default(Boolean)));
        //public static readonly DependencyProperty IsIndexedProperty= IsIndexedPropertyKey.DependencyProperty;
        //public Boolean IsIndexed {
        //    get { return (Boolean)GetValue(IsIndexedProperty); }
        //    private set { SetValue(IsIndexedPropertyKey, value); }
        //    }
        //#endregion
        //#region P:IsExpanded:Boolean
        //public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean),typeof(GridEntry),new FrameworkPropertyMetadata(false));
        //public Boolean IsExpanded {
        //    get { return (Boolean)GetValue(IsExpandedProperty); }
        //    set { SetValue(IsExpandedProperty, value); }
        //    }
        //#endregion
        //#region P:IndexPlaceholderSize:Boolean
        //public static readonly DependencyProperty IndexPlaceholderSizeProperty = DependencyProperty.Register("IndexPlaceholderSize", typeof(Size),typeof(GridEntry),new FrameworkPropertyMetadata(default(Size)));
        //public Size IndexPlaceholderSize {
        //    get { return (Size)GetValue(IndexPlaceholderSizeProperty); }
        //    set { SetValue(IndexPlaceholderSizeProperty, value); }
        //    }
        //#endregion
        //#region P:Margin:Thickness
        //public Thickness Margin { get {
        //    return new Thickness(
        //        Level*16,
        //        0,
        //        5,0);
        //    }}
        //#endregion
        //#region P:Value:Object
        //public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(GridEntry), new PropertyMetadata(default(Object), OnValueChanged));
        //private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //    var source = d as GridEntry;
        //    if (source != null) {
        //        source.OnValueChanged();
        //        }
        //    }

        //private void OnValueChanged() {
        //    if (PropertyDescriptor != null) {
        //        //PropertyDescriptor.SetValue(Instance, Value);
        //        }
        //    }

        //public Object Value {
        //    get { return (Object)GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //    }
        //#endregion
        //protected internal virtual IEnumerable<GridEntry> Entries { get {
        //    if (PropertyDescriptor != null) {
        //        var value = Value;
        //        return GetEntries(value, Level + 1, Owner);
        //        //var converter = PropertyDescriptor.Converter;
        //        //var value = Value;
        //        //if ((value != null) && (converter != null)) {
        //        //    if (converter.GetPropertiesSupported(this)) {
        //        //        return converter.GetProperties(this, value).OfType<PropertyDescriptor>().
        //        //            Where(i => i.IsBrowsable).
        //        //            OrderBy(i => i.DisplayName).
        //        //            Select(i => new GridEntry(i, value, Level + 1, Owner));
        //        //        }
        //        //    }
        //        }
        //    return new GridEntry[0];
        //    }
        //    set { }
        //    }

        //private static readonly Type[] NonExpanableTypes = {
        //    typeof(String),
        //    typeof(Color),
        //    typeof(SolidColorBrush)
        //    };

        //protected static internal IEnumerable<GridEntry> GetEntries(Object source, Int32 level, PropertyGridControl owner) {
        //    if (source != null) {
        //        if (!NonExpanableTypes.Contains(source.GetType())) {
        //            if (source is IList) { return GetEntries((IList)source, level, owner); }
        //            var descriptors = TypeDescriptor.GetProperties(source).OfType<PropertyDescriptor>().ToArray();
        //            var services = new SortedDictionary<String, IList<PropertyDescriptor>>();
        //            var r = new List<PropertyDescriptor>();
        //            foreach (var descriptor in descriptors.Where(i => i.IsBrowsable)) {
        //                var index = descriptor.DisplayName.IndexOf(".", StringComparison.OrdinalIgnoreCase);
        //                if (index != -1) {
        //                    var service = descriptor.DisplayName.Substring(0, index);
        //                    IList<PropertyDescriptor> list;
        //                    if (!services.TryGetValue(service, out list)) { services.Add(service, list = new List<PropertyDescriptor>()); }
        //                    list.Add(descriptor);
        //                    }
        //                else
        //                    {
        //                    r.Add(descriptor);
        //                    }
        //                }
        //            foreach (var service in services) {
        //                var descriptor = new CompositePropertyDescriptor(
        //                    source,
        //                    service.Key, service.Value);
        //                r.Add(descriptor);
        //                }
        //            return r.OrderBy(i => i.DisplayName).Select(i => new GridEntry(i, source, level, owner));
        //            }
        //        }
        //    return new GridEntry[0];
        //    }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="level"></param>
        ///// <param name="owner"></param>
        ///// <returns></returns>
        //private static IEnumerable<GridEntry> GetEntries(IList source, Int32 level, PropertyGridControl owner) {
        //    if (source != null) {
        //        var c = source.Count;
        //        var f = new FormattedText(new String('X', c.ToString().Length),
        //            CultureInfo.InvariantCulture, owner.FlowDirection,
        //            new Typeface(owner.FontFamily, owner.FontStyle, owner.FontWeight, owner.FontStretch),
        //            owner.FontSize*0.8, null);
        //        for (var i = 0; i < c; ++i) {
        //            yield return new GridEntry(i, source, level + 1, owner, new Size(f.Width,f.Height));
        //            }
        //        }
        //    }

        protected IEnumerable<GridEntry> Entries {
            get {
                if (PropertyDescriptor != null) {
                    var value = Value;
                    if (value == null) { yield break; }
                    foreach (var entry in GetEntries(this, value, Level + 1, Owner)) {
                        yield return entry;
                        }
                    }
                }
            }

        #region I:IEnumerable<GridEntry>
        IEnumerator<GridEntry> IEnumerable<GridEntry>.GetEnumerator() { return Entries.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return Entries.GetEnumerator(); }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        //private class ArrayPropertyDescriptor : PropertyDescriptor
        //    {
        //    public ArrayPropertyDescriptor(Int32 index, Type propertyType)
        //        : base(index.ToString(), new Attribute[0])
        //        {
        //        Index = index;
        //        PropertyType = propertyType;
        //        }

        //    #region M:CanResetValue(Object):Boolean
        //    public override Boolean CanResetValue(Object component) {
        //        var defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
        //        return (defaultValueAttribute != null) && defaultValueAttribute.Value.Equals(GetValue(component));
        //        }
        //    #endregion
        //    #region M:GetValue(Object):Object
        //    public override Object GetValue(Object component) {
        //        var source = component as IList;
        //        if (source != null) {
        //            if (Index < source.Count) {
        //                return source[Index];
        //                }
        //            }
        //        return null;
        //        }
        //    #endregion
        //    #region M:ResetValue(Object)
        //    public override void ResetValue(Object component) {
        //        var defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
        //        if (defaultValueAttribute != null) {
        //            SetValue(component, defaultValueAttribute.Value);
        //            }
        //        }
        //    #endregion
        //    #region M:SetValue(Object,Object)
        //    public override void SetValue(Object component, Object value) {
        //        var source = component as IList;
        //        if (source != null) {
        //            if (Index < source.Count) {
        //                source[Index] = value;
        //                OnValueChanged(source, EventArgs.Empty);
        //                }
        //            }
        //        }
        //    #endregion
        //    #region M:ShouldSerializeValue(Object):Boolean
        //    public override Boolean ShouldSerializeValue(Object component) {
        //        return false;
        //        }
        //    #endregion

        //    public override Type ComponentType { get; }
        //    public override Boolean IsReadOnly { get; }
        //    public override Type PropertyType { get; }
        //    private Int32 Index {get; }
        //    }

        //public Object GetService(Type serviceType)
        //{
        //    throw new NotImplementedException();
        //}

        private static IEnumerable<T> OfType<T>(IEnumerable<T> source) {
            return (source != null)
                ? source.OfType<T>()
                : new T[0];
            }

        private static IEnumerable<T> OfType<T>(IEnumerable source) {
            return (source != null)
                ? source.OfType<T>()
                : new T[0];
            }

        public static IEnumerable<GridEntry> GetEntries(ITypeDescriptorContext context, Object source, Int32 level, PropertyGridControl owner) {
            if (source != null) {
                var type = source.GetType();
                if ((type == typeof(String) || type == typeof(DateTime))) { yield break; }
                var converter = ((context != null) && (context.PropertyDescriptor != null))
                    ? GetConverter(context.PropertyDescriptor, source)
                    : TypeDescriptor.GetConverter(source);
                var descriptors = (converter != null) && (context != null)
                    ? converter.GetPropertiesSupported(context)
                        ? OfType<PropertyDescriptor>(converter.GetProperties(context, source, new Attribute[]{ new BrowsableAttribute(true) })).ToArray()
                        : new PropertyDescriptor[0]
                    : OfType<PropertyDescriptor>(TypeDescriptor.GetProperties(source, new Attribute[]{ new BrowsableAttribute(true) })).ToArray();
                foreach (var e in descriptors.Select(i => new GridEntry(i, source, level, owner))) {
                    yield return e;
                    }
                //foreach (var e in descriptors.OrderBy(i => i.DisplayName).Select(i => new GridEntry(i, source, level, owner))) {
                //    yield return e;
                //    }
            }
            yield break;
            }

        public Object GetService(Type serviceType) {
            return null;
            }

        public Boolean OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public IContainer Container { get; }
        public Object Instance { get; }

        private static Object[] ToArray(IEnumerable source) {
            return (source != null)
                ? source.Cast<Object>().ToArray()
                : new Object[0];
            }

        [DllImport("gdi32")] static extern Int32 DeleteObject(IntPtr o);
        private static BitmapSource ToBitmapSource(Bitmap source) {
            var ip = source.GetHbitmap();
            BitmapSource r = null;
            try
                {
                r = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
                }
            finally
                {
                DeleteObject(ip);
                }
            return r;
            }
        }
    }
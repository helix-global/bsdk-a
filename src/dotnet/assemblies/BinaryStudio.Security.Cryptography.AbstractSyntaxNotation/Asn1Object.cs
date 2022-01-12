using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [JsonConverter(typeof(Converter))]
    [TypeConverter(typeof(Asn1ObjectConverter))]
    public abstract class Asn1Object : IList<Asn1Object>, IEquatable<Asn1Object>,
        IJsonSerializable, IXmlSerializable, IAsn1Object, IServiceProvider,
        ICustomTypeDescriptor
        {
        [Flags]
        protected internal enum ObjectState : byte
            {
            None = 0,
            ExplicitConstructed = 1,
            ImplicitConstructed = 2,
            Failed = 4,
            Indefinite = 8,
            Decoded = 16,
            Building = 32,
            SealedLength = 64,
            SealedSize = 128
            }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private ObjectState state;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Int64 offset;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Int64 size;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected Int64 length;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected internal ReadOnlyMappingStream content;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly List<Asn1Object> sequence = new List<Asn1Object>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] SByte IAsn1Object.Type { get { return -1; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected internal virtual Boolean IsDecoded { get { return state.HasFlag(ObjectState.Decoded); }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)][Browsable(false)] public virtual Boolean IsFailed  { get { return state.HasFlag(ObjectState.Failed);  }}
        [Browsable(false)] public virtual Boolean IsExplicitConstructed { get { return state.HasFlag(ObjectState.ExplicitConstructed); }}
        [Browsable(false)] public virtual Boolean IsImplicitConstructed { get { return state.HasFlag(ObjectState.ImplicitConstructed); }}
        [Browsable(false)] public virtual Boolean IsIndefiniteLength    { get { return state.HasFlag(ObjectState.Indefinite); }}
        public virtual ReadOnlyMappingStream Content { get { return content; }}
        public virtual Int64 Offset { get{ return offset; }}

        #region M:Body:Byte[]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public virtual Byte[] Body { get {
            using (var target = new MemoryStream())
                {
                Write(target);
                return target.ToArray();
                }
            }}
        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal virtual ObjectState State {
            get { return state; }
            set
                {
                if (state != value)
                    {
                    state = value;
                    }
                }
            }

        #region M:IServiceProvider.GetService(Type):Object
        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="service">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
        /// <filterpriority>2</filterpriority>
        public virtual Object GetService(Type service)
            {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            if (service == GetType()) { return this; }
            if (service == typeof(Asn1Object)) { return this; }
            return null;
            }
        #endregion
        #region M:IEnumerable<Asn1Object>.GetEnumerator:IEnumerator<Asn1Object>
        /**
         * <summary>Returns an enumerator that iterates through the collection.</summary>
         * <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
         * <filterpriority>1</filterpriority>
         * */
        public virtual IEnumerator<Asn1Object> GetEnumerator() { return sequence.GetEnumerator(); }
        #endregion
        #region M:IEnumerable.GetEnumerator:IEnumerator
        /**
         * <summary>Returns an enumerator that iterates through a collection.</summary>
         * <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
         * <filterpriority>2</filterpriority>
         * */
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        #endregion
        #region M:ICollection<Asn1Object>.Add(Asn1Object)
        protected void Add(Asn1Object item)
            {
            if (IsReadOnly) { throw new InvalidOperationException(); }
            sequence.Add(item);
            }

        /**
         * <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
         * <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
         * <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
         * */
        void ICollection<Asn1Object>.Add(Asn1Object item)
            {
            Add(item);
            }
        #endregion
        #region M:ICollection<Asn1Object>.Clear
        protected void Clear() {
            if (IsReadOnly) { throw new InvalidOperationException(); }
            sequence.Clear();
            }

        /**
         * <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
         * <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
         * */
        void ICollection<Asn1Object>.Clear()
            {
            Clear();
            }
        #endregion
        #region M:ICollection<Asn1Object>.Contains(IAsn1Object):Boolean
        /**
         * <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.</summary>
         * <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
         * <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.</returns>
         * */
        Boolean ICollection<Asn1Object>.Contains(Asn1Object item)
            {
            return sequence.Contains(item);
            }
        #endregion
        #region M:ICollection<Asn1Object>.CopyTo(IAsn1Object[],Int32)
        /**
         * <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="Array"/>, starting at a particular <see cref="Array"/> index.</summary>
         * <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="Array"/> must have zero-based indexing.</param>
         * <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
         * <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
         * <exception cref="ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
         * */
        void ICollection<Asn1Object>.CopyTo(Asn1Object[] array, Int32 arrayIndex)
            {
            sequence.CopyTo(array, arrayIndex);
            }
        #endregion
        #region M:ICollection<Asn1Object>.Remove(IAsn1Object):Boolean
        /**
         * <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
         * <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
         * <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
         * <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
         * */
        Boolean ICollection<Asn1Object>.Remove(Asn1Object item)
            {
            if (IsReadOnly) { throw new NotSupportedException(); }
            return sequence.Remove(item);
            }
        #endregion
        #region P:ICollection<Asn1Object>.Count:Int32
        /**
         * <summary>Gets the number of elements in the collection.</summary>
         * <returns>The number of elements in the collection.</returns>
         * */
        public virtual Int32 Count
            {
            get { return sequence.Count; }
            }
        #endregion
        #region P:ICollection<Asn1Object>.IsReadOnly:Boolean
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected Boolean IsReadOnly { get;set; }

        /**
         * <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</summary>
         * <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
         * */
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] Boolean ICollection<Asn1Object>.IsReadOnly { get {
            return IsReadOnly;
            }}
        #endregion
        #region M:IList<Asn1Object>.IndexOf(IAsn1Object):Int32
        /**
         * <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.</summary>
         * <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
         * <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
         * */
        Int32 IList<Asn1Object>.IndexOf(Asn1Object item)
            {
            return sequence.IndexOf(item);
            }
        #endregion
        #region M:IList<Asn1Object>.Insert(Int32,IAsn1Object)
        /**
         * <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.</summary>
         * <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
         * <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
         * <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
         * */
        void IList<Asn1Object>.Insert(Int32 index, Asn1Object item)
            {
            if (IsReadOnly) { throw new NotSupportedException(); }
            sequence.Insert(index, item);
            }
        #endregion
        #region M:IList<Asn1Object>.RemoveAt(Int32)
        /**
         * <summary>Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.</summary>
         * <param name="index">The zero-based index of the item to remove.</param>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
         * <exception cref="NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1" /> is read-only.</exception>
         * */
        void IList<Asn1Object>.RemoveAt(Int32 index)
            {
            if (IsReadOnly) { throw new NotSupportedException(); }
            sequence.RemoveAt(index);
            state &= ~ObjectState.SealedSize;
            state &= ~ObjectState.SealedLength;
            }
        #endregion
        #region P:IList<Asn1Object>.this[Int32]:IAsn1Object
        /**
         * <summary>Gets or sets the element at the specified index.</summary>
         * <param name="index">The zero-based index of the element to get or set.</param>
         * <returns>The element at the specified index.</returns>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
         * <exception cref="NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
         * */
        public virtual Asn1Object this[Int32 index]
            {
            get { return sequence[index]; }
            set
                {
                if (IsReadOnly) { throw new NotSupportedException(); }
                sequence[index] = value;
                state &= ~ObjectState.SealedSize;
                state &= ~ObjectState.SealedLength;
                }
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        protected virtual AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(GetType()); }
        /**
         * <summary>Returns a collection of custom attributes for this instance of a component.</summary>
         * <returns>An <see cref="AttributeCollection"/> containing the attributes for this object.</returns>
         * */
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            return GetAttributes();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetClassName:String
        /**
         * <summary>Returns the class name of this instance of a component.</summary>
         * <returns>The class name of the object, or null if the class does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetClassName()
            {
            return TypeDescriptor.GetClassName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetComponentName:String
        /**
         * <summary>Returns the name of this instance of a component.</summary>
         * <returns>The name of the object, or null if the object does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetComponentName()
            {
            return TypeDescriptor.GetComponentName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        /**
         * <summary>Returns a type converter for this instance of a component.</summary>
         * <returns>A <see cref="TypeConverter"/> that is the converter for this object, or null if there is no <see cref="TypeConverter"/> for this object.</returns>
         * */
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            var r = TypeDescriptor.GetConverter(GetType());
            return r;
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultEvent:EventDescriptor
        /**
         * <summary>Returns the default event for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.</returns>
         * */
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            return TypeDescriptor.GetDefaultEvent(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        /**
         * <summary>Returns the default property for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.</returns>
         * */
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            return TypeDescriptor.GetDefaultProperty(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        /**
         * <summary>Returns an editor of the specified type for this instance of a component.</summary>
         * <param name="editortype">A <see cref="Type"/> that represents the editor for this object. </param>
         * <returns>An <see cref="Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.</returns>
         * */
        Object ICustomTypeDescriptor.GetEditor(Type editortype)
            {
            return TypeDescriptor.GetEditor(GetType(), editortype);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents:EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            return TypeDescriptor.GetEvents(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component using the specified attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the filtered events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            return TypeDescriptor.GetEvents(GetType(), attributes);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        protected virtual IEnumerable<PropertyDescriptor> GetProperties()
            {
            return TypeDescriptor.GetProperties(GetType()).OfType<PropertyDescriptor>();
            }
        /**
         * <summary>Returns the properties for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
            return new PropertyDescriptorCollection(GetProperties().ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        protected virtual IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes)
            {
            return TypeDescriptor.GetProperties(GetType()).OfType<PropertyDescriptor>();
            }
        /**
         * <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
            return new PropertyDescriptorCollection(GetProperties(attributes).ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor):Object
        /**
         * <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
         * <param name="descriptor">A <see cref="PropertyDescriptor"/> that represents the property whose owner is to be found. </param>
         * <returns>An <see cref="Object"/> that represents the owner of the specified property.</returns>
         * */
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return GetPropertyOwner(descriptor);
            }

        protected virtual Object GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return this;
            }
        #endregion
        #region M:Equals(Asn1Object,Asn1Object):Boolean
        public static Boolean Equals(Asn1Object x, Asn1Object y) {
            if (ReferenceEquals(x, y))    { return true;  }
            if (ReferenceEquals(x, null)) { return false; }
            return x.Equals(y);
            }
        #endregion
        #region M:Equals(IList<Asn1Object>,IList<Asn1Object>):Boolean
        public static Boolean Equals(IList<Asn1Object> x, IList<Asn1Object> y) {
            if (ReferenceEquals(x, y))    { return true;  }
            if (ReferenceEquals(x, null)) { return false; }
            if (ReferenceEquals(y, null)) { return false; }
            var c = x.Count;
            if (y.Count == c) {
                for (var i = 0; i < c; ++i) {
                    if (!Equals(x[i], y[i])) { return false; }
                    }
                return true;
                }
            return false;
            }
        #endregion
        #region M:Equals(Asn1Object):Boolean
        public virtual Boolean Equals(Asn1Object other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (Class == other.Class);
            }
        #endregion
        #region M:Equals(Object):Boolean
        /**
         * <summary>Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.</summary>
         * <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.</returns>
         * <param name="other">The object to compare with the current object.</param>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (other is Asn1Object r) && Equals(r);
            }
        #endregion
        #region M:GetHashCode:Int32
        /**
         * <summary>Serves as a hash function for a particular type.</summary>
         * <returns>A hash code for the current <see cref="Object"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            //return HashCodeCombiner.GetHashCode(Class, Type);
            return base.GetHashCode();
            }
        #endregion
        #region M:ToString:String
        /**
         * <summary>Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</summary>
         * <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
         */
        public override String ToString()
            {
            return Count > 0
                ? $"Count = {Count}"
                : $"Size  = {Length}";
            }
        #endregion

        /// <summary>
        /// ASN.1 object class.
        /// </summary>
        public abstract Asn1ObjectClass Class { get; }
        public virtual Int64 Length { get {
            if (state.HasFlag(ObjectState.SealedLength)) { return length; }
            var c = Count;
            var r = 0L;
            if (c > 0)
                {
                foreach (var i in this)
                    {
                    r += i.Size;
                    }
                }
            else
                {
                r = content.Length;
                }
            length = r;
            state |= ObjectState.SealedLength;
            return length;
            }}

        public virtual Int64 Size   { get {
            if (state.HasFlag(ObjectState.SealedSize)) { return size; }
            //var r = Header.Length + Length;
            //state |= State.SealedSize;
            return size;
            }}

        protected Asn1Object(ReadOnlyMappingStream source, Int64 forceoffset)
            {
            state |= ObjectState.SealedLength | ObjectState.SealedSize;
            offset = source.Position - 1;
            length = DecodeLength(source);
                 if (length == -1) { state |= ObjectState.Failed; }
            else if (length == -2)
                {
                state |= ObjectState.Indefinite;
                size = 2;
                /* just link */
                content = source;
                offset += forceoffset;
            }
            else
                {
                size = source.Position - offset + length;
                if (offset + size > source.Length) {
                    state |= ObjectState.Failed;
                    return;
                    }
                offset += forceoffset;
                content = source.Clone(length);
                source.Seek(length, SeekOrigin.Current);
                }
            }

        internal Asn1Object()
            {
            }

        #region M:Decode(IEnumerable<Asn1Object>):Boolean
        private static Boolean Decode(IEnumerable<Asn1Object> items)
            {
            #if TRACE
            using (TraceManager.Instance.Trace())
            #endif
                {
                #if FEATURE_MULTI_THREAD_PROCESSING
                var r = new List<Task<Boolean>>();
                foreach (var item in items)
                    {
                    r.Add(Task.Factory.StartNew(item.Decode));
                    }
                Task.WaitAll(r.OfType<Task>().ToArray());
                return r.All(i => i.Result);
                #else
                return items.All(item => item.Decode());
                #endif
                }
            }
        #endregion
        #region M:Decode:Boolean
        protected internal virtual Boolean Decode()
            {
            #if TRACE
            using (TraceManager.Instance.Trace())
            #endif
                {
                if (IsDecoded) { return true;  }
                if (IsFailed)  { return false; }
                try
                    {
                    if (IsIndefiniteLength)
                        {
                        var r = new List<Asn1Object>();
                        var sz = size;
                        var ln = 0L;
                        var flag = true;
                        for (;;)
                            {
                            var i = ReadNext(content, 0);
                            if (i == null)
                                {
                                flag = false;
                                break;
                                }
                            i.offset += offset;
                            if (i is Asn1EndOfContent)
                                {
                                sz += i.Size;
                                r.Add(i);
                                flag = true;
                                break;
                                }
                            sz += i.Size;
                            ln += i.Size;
                            r.Add(i);
                            }
                        if (flag)
                            {
                            size   = sz;
                            length = ln;
                            content = content.Clone(offset, length);
                            if (Decode(r))
                                {
                                sequence.AddRange(r);
                                state |= ObjectState.Decoded;
                                if (!IsExplicitConstructed)
                                    {
                                    state |= ObjectState.ImplicitConstructed;
                                    }
                                return true;
                                }
                            }
                        if (!IsExplicitConstructed)
                            {
                            state |= ObjectState.Decoded;
                            return true;
                            }
                        }
                    else
                        {
                        var r = new List<Asn1Object>();
                        var forceoffset = offset + size - length;
                        var flag = true;
                        var sz = length;
                        while (sz != 0)
                            {
                            var i = ReadNext(content, forceoffset);
                            if ((i == null) || (i is Asn1EndOfContent))
                                {
                                flag = false;
                                break;
                                }
                            r.Add(i);
                            sz -= i.Size;
                            if (sz < 0)
                                {
                                flag = false;
                                break;
                                }
                            }
                        if (flag)
                            {
                            if (Decode(r))
                                {
                                sequence.AddRange(r);
                                state |= ObjectState.Decoded;
                                if (!IsExplicitConstructed)
                                    {
                                    state |= ObjectState.ImplicitConstructed;
                                    }
                                return true;
                                }
                            }
                        if (!IsExplicitConstructed)
                            {
                            state |= ObjectState.Decoded;
                            return true;
                            }
                        }
                    state |= ObjectState.Failed;
                    return false;
                    }
                catch
                    {
                    state |= ObjectState.Failed;
                    return false;
                    }
                }
            }
        #endregion
        #region M:DecodeLength(Stream):Int64
        internal static Int64 DecodeLength(Stream source) {
            var r = (Int64)source.ReadByte();
            if (r == 0) { return 0; }
            if ((r & 0x80) == 0x80) {
                var c = r & 0x7f;
                if (c == 0) { return -2; }
                if (c > 7)  { return -1; }
                r = 0L;
                for (var i = 0; i < c; ++i) {
                    r <<= 8;
                    r |= (Int64)source.ReadByte();
                    }
                }
            return r;
            }
        #endregion

        /// <summary>
        /// Reads next ASN.1 object from specified read-only stream and with specified offset.
        /// </summary>
        /// <param name="source">Read only source stream.</param>
        /// <param name="forceoffset">Offset to pass to newly created object.</param>
        /// <returns>Returns next ASN.1 object.</returns>
        protected static Asn1Object ReadNext(ReadOnlyMappingStream source, Int64 forceoffset)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Asn1Object o = null;
            if (source.Position == source.Length) { return null; }
            var p = source.Position;
            var r = source.ReadByte();
            var c = (Asn1ObjectClass)((r & 0xc0) >> 6);
            var state = ((r & 0x20) == 0x20) ? ObjectState.ExplicitConstructed : ObjectState.None;
            switch (c)
                {
                case Asn1ObjectClass.Universal:
                    {
                    switch ((Asn1ObjectType)(r & 0x1f))
                        {
                        case Asn1ObjectType.EndOfContent:               { o = new Asn1EndOfContent            (source, forceoffset); } break;
                        case Asn1ObjectType.Boolean:                    { o = new Asn1Boolean                 (source, forceoffset); } break;
                        case Asn1ObjectType.Integer:                    { o = new Asn1Integer                 (source, forceoffset); } break;
                        case Asn1ObjectType.BitString:                  { o = new Asn1BitString               (source, forceoffset); } break;
                        case Asn1ObjectType.OctetString:                { o = new Asn1OctetString             (source, forceoffset); } break;
                        case Asn1ObjectType.Null:                       { o = new Asn1Null                    (source, forceoffset); } break;
                        case Asn1ObjectType.ObjectIdentifier:           { o = new Asn1ObjectIdentifier        (source, forceoffset); } break;
                        case Asn1ObjectType.ObjectDescriptor:           { o = new Asn1ObjectDescriptor        (source, forceoffset); } break;
                        case Asn1ObjectType.External:                   { o = new Asn1External                (source, forceoffset); } break;
                        case Asn1ObjectType.Real:                       { o = new Asn1Real                    (source, forceoffset); } break;
                        case Asn1ObjectType.Enum:                       { o = new Asn1Enum                    (source, forceoffset); } break;
                        case Asn1ObjectType.EmbeddedPDV:                { o = new Asn1EmbeddedPDV             (source, forceoffset); } break;
                        case Asn1ObjectType.RelativeObjectIdentifier:   { o = new Asn1RelativeObjectIdentifier(source, forceoffset); } break;
                        case Asn1ObjectType.Sequence:                   { o = new Asn1Sequence                (source, forceoffset); } break;
                        case Asn1ObjectType.Set:                        { o = new Asn1Set                     (source, forceoffset); } break;
                        case Asn1ObjectType.Utf8String:                 { o = new Asn1Utf8String              (source, forceoffset); } break;
                        case Asn1ObjectType.NumericString:              { o = new Asn1NumericString           (source, forceoffset); } break;
                        case Asn1ObjectType.PrintableString:            { o = new Asn1PrintableString         (source, forceoffset); } break;
                        case Asn1ObjectType.TeletexString:              { o = new Asn1TeletexString           (source, forceoffset); } break;
                        case Asn1ObjectType.VideotexString:             { o = new Asn1VideotexString          (source, forceoffset); } break;
                        case Asn1ObjectType.IA5String:                  { o = new Asn1IA5String               (source, forceoffset); } break;
                        case Asn1ObjectType.UtcTime:                    { o = new Asn1UtcTime                 (source, forceoffset); } break;
                        case Asn1ObjectType.GeneralTime:                { o = new Asn1GeneralTime             (source, forceoffset); } break;
                        case Asn1ObjectType.GraphicString:              { o = new Asn1GraphicString           (source, forceoffset); } break;
                        case Asn1ObjectType.VisibleString:              { o = new Asn1VisibleString           (source, forceoffset); } break;
                        case Asn1ObjectType.GeneralString:              { o = new Asn1GeneralString           (source, forceoffset); } break;
                        case Asn1ObjectType.UniversalString:            { o = new Asn1UniversalString         (source, forceoffset); } break;
                        case Asn1ObjectType.UnicodeString:              { o = new Asn1UnicodeString           (source, forceoffset); } break;
                        }
                    }
                    break;
                case Asn1ObjectClass.Application:     { o = new Asn1ApplicationObject    (source, forceoffset, (SByte)(r & 0x1f)); } break;
                case Asn1ObjectClass.ContextSpecific: { o = new Asn1ContextSpecificObject(source, forceoffset, (SByte)(r & 0x1f)); } break;
                case Asn1ObjectClass.Private:         { o = new Asn1PrivateObject        (source, forceoffset, (SByte)(r & 0x1f)); } break;
                }
            if ((o == null) || (o.IsFailed))
                {
                source.Seek(p, SeekOrigin.Begin);
                return null;
                }
            o.state |= state;
            if (o.IsIndefiniteLength) {
                if (!o.Decode()) {
                    source.Seek(p, SeekOrigin.Begin);
                    return null;
                    }
                }
            return o;
            }

        #region M:Find(Func<Asn1Object,Boolean>):IEnumerable<Asn1Object>
        public virtual IEnumerable<Asn1Object> Find(Func<Asn1Object, Boolean> predicate) {
            if (predicate != null) {
                foreach (var i in sequence) {
                    if (predicate(i)) {
                        yield return i;
                        }
                    }
                }
            }
        #endregion
        #region M:FindAll(Func<Asn1Object,Boolean>):IEnumerable<Asn1Object>
        public virtual IEnumerable<Asn1Object> FindAll(Func<Asn1Object, Boolean> predicate) {
            if (predicate != null) {
                if (predicate(this)) {
                    yield return this;
                    }
                }
            else
                {
                yield return this;
                }
            foreach (var i in sequence) {
                foreach (var o in i.FindAll(predicate))
                    {
                    yield return o;
                    }
                }
            }
        #endregion

        private class ReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public ReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterReadLock();
                }

            public void Dispose()
                {
                o.ExitReadLock();
                o = null;
                }
            }

        private class UpgradeableReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public UpgradeableReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterUpgradeableReadLock();
                }

            public void Dispose()
                {
                o.ExitUpgradeableReadLock();
                o = null;
                }
            }

        private class WriteLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public WriteLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterWriteLock();
                }

            public void Dispose()
                {
                o.ExitWriteLock();
                o = null;
                }
            }

        protected internal static IDisposable ReadLock(ReaderWriterLockSlim o)            { return new ReadLockScope(o);            }
        protected internal static IDisposable WriteLock(ReaderWriterLockSlim o)           { return new WriteLockScope(o);           }
        protected internal static IDisposable UpgradeableReadLock(ReaderWriterLockSlim o) { return new UpgradeableReadLockScope(o); }

        private class Converter : JsonConverter
            {
            #region M:WriteJson(JsonWriter,Object,JsonSerializer)
            /// <summary>Writes the JSON representation of the object.</summary>
            /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
                {
                if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
                var o = value as Asn1Object;
                if (o != null) {
                    o.WriteJson(writer, serializer);
                    }
                }
            #endregion
            #region M:ReadJson(JsonReader,Type,Object,JsonSerializer)
            /// <summary>Reads the JSON representation of the object.</summary>
            /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
                {
                throw new NotImplementedException();
                }
            #endregion
            #region M:CanConvert(Type):Boolean
            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
            /// </returns>
            public override Boolean CanConvert(Type objectType)
                {
                throw new NotImplementedException();
                }
            #endregion
            }

        #region M:WriteValue(JsonWriter,JsonSerializer,String,Object)
        protected static void WriteValue(JsonWriter writer, JsonSerializer serializer, String name, Object value) {
            if (value != null) {
                writer.WritePropertyName(name);
                if (value is IJsonSerializable i)
                    {
                    i.WriteJson(writer, serializer);
                    }
                else
                    {
                    writer.WriteValue(value);
                    }
                }
            }
        #endregion
        #region M:IJsonSerializable.WriteJson(JsonWriter,JsonSerializer)
        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartObject();
            WriteJsonOverride(writer, serializer);
            writer.WriteEndObject();
            }
        #endregion

        protected virtual void WriteJsonOverride(JsonWriter writer, JsonSerializer serializer)
            {
            WriteValue(writer, serializer, nameof(Class), Class.ToString());
            WriteValue(writer, serializer, "Type",
                (Class == Asn1ObjectClass.Universal)       ? ((Asn1UniversalObject)this).Type.ToString()       :
                (Class == Asn1ObjectClass.Application)     ? ((Asn1ApplicationObject)this).Type.ToString()     :
                (Class == Asn1ObjectClass.ContextSpecific) ? ((Asn1ContextSpecificObject)this).Type.ToString() : ((Asn1PrivateObject)this).Type.ToString());
            if (Offset >= 0) { WriteValue(writer, serializer, nameof(Offset), Offset); }
            var c = Count;
            if (c > 0) {
                writer.WritePropertyName("(Self)");
                writer.WriteStartArray();
                foreach (var item in this) {
                    item.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            else
                {
                var array = Content.ToArray();
                writer.WritePropertyName(nameof(Content));
                if (array.Length > 50)
                    {
                    writer.WriteRaw(" \"");
                    foreach (var value in Convert.ToBase64String(Content.ToArray(), Base64FormattingOptions.InsertLineBreaks).Split('\n')) {
                        //writer.WriteIndent();
                        writer.WriteRaw($"         {value}");
                        }
                    writer.WriteRawValue("\"");
                    }
                else if (array.Length > 0)
                    {
                    writer.WriteValue(array);
                    }
                }
            }


        #region M:IXmlSerializable.GetSchema:XmlSchema
        XmlSchema IXmlSerializable.GetSchema()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IXmlSerializable.ReadXml(XmlReader)
        public virtual void ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IXmlSerializable.WriteXml(XmlWriter)
        public virtual void WriteXml(XmlWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartElement("Object");
            writer.WriteAttributeString(nameof(Class), Class.ToString());
            if (Offset >= 0) { writer.WriteAttributeString(nameof(Offset), Offset.ToString()); }
            writer.WriteAttributeString("Type",
                (Class == Asn1ObjectClass.Universal)       ? ((Asn1UniversalObject)this).Type.ToString()       :
                (Class == Asn1ObjectClass.Application)     ? ((Asn1ApplicationObject)this).Type.ToString()     :
                (Class == Asn1ObjectClass.ContextSpecific) ? ((Asn1ContextSpecificObject)this).Type.ToString() : ((Asn1PrivateObject)this).Type.ToString());
            var c = Count;
            if (c > 0)
                {
                foreach (var item in this)
                    {
                    item.WriteXml(writer);
                    }
                }
            else
                {
                
                }
            writer.WriteEndElement();
            writer.Flush();
            }
        #endregion

        /// <summary>
        /// Loads ASN.1 structure from specified read-only stream and by specified flags.
        /// </summary>
        /// <param name="source">Read only stream.</param>
        /// <param name="flags">Load flags.</param>
        /// <returns>Returns sequence of ASN.1 objects.</returns>
        public static IEnumerable<Asn1Object> Load(ReadOnlyMappingStream source, Asn1ReadFlags flags = 0) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.CanSeek) {
                var sz = source.Length;
                if (flags.HasFlag(Asn1ReadFlags.IgnoreLeadLineEnding)) {
                    if (sz > 1) {
                        var p = source.Position;
                        var c = source.ReadByte();
                        if (c == '\r') {
                            c = source.ReadByte();
                            if (c != '\n') {
                                source.Seek(p + 1, SeekOrigin.Begin);
                                }
                            }
                        else
                            {
                            source.Seek(p, SeekOrigin.Begin);
                            }
                        }
                    }
                }
            for (;;)
                {
                var i = ReadNext(source, source.Position);
                if (i == null) { break; }
                i.Decode();
                yield return i;
                }
            }

        /// <summary>
        /// Loads ASN.1 structure from specified <paramref name="filename"/> and by specified <paramref name="flags"/>.
        /// </summary>
        /// <param name="filename">File name where ASN.1 is located.</param>
        /// <param name="flags">Load flags.</param>
        /// <returns>Returns sequence of ASN.1 objects.</returns>
        public static IEnumerable<Asn1Object> Load(String filename, Asn1ReadFlags flags = 0) {
            return Load(new ReadOnlyFileMappingStream(filename), flags);
            }

        /// <summary>
        /// Loads ASN.1 structure from specified read-only stream and by specified flags.
        /// </summary>
        /// <param name="source">Read only stream.</param>
        /// <param name="flags">Load flags.</param>
        /// <returns>Returns sequence of ASN.1 objects.</returns>
        public static IEnumerable<Asn1Object> Load(Stream source, Asn1ReadFlags flags = 0) {
            if (source == null)  { throw new ArgumentNullException(nameof(source));       }
            if (!source.CanRead) { throw new ArgumentOutOfRangeException(nameof(source)); }
            return Load(new ReadOnlyStream(source), flags);
            }

        public virtual void Write(Stream target) {
            WriteHeader(target);
            WriteContent(target);
            }
        protected virtual void WriteContent(Stream target) {
            Content.Seek(0, SeekOrigin.Begin);
            Content.CopyTo(target);
            if (IsIndefiniteLength)
                {
                target.WriteByte(0);
                target.WriteByte(0);
                target.WriteByte(0);
                target.WriteByte(0);
                }
            }

        protected virtual void WriteHeader(Stream target) {
            WriteHeader(target, IsExplicitConstructed, Class, ((IAsn1Object)this).Type,
                IsIndefiniteLength
                 ? - 1
                 : Length);
            }

        protected static void WriteHeader(Stream target, Boolean constructed, Asn1ObjectClass @class, SByte type, Int64? length)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var r = constructed ? 0x20 : 0x00;
            r |= ((Byte)@class) << 6;
            r |= (Byte)type;
            target.WriteByte((Byte)r);
            if (length < 0)     {
                target.WriteByte((Byte)0x80);
                return;
                }
            if (length < 0x80) { target.WriteByte((Byte)length); }
            else
                {
                var n = new List<Byte>();
                while (length > 0) {
                    n.Add((Byte)(length & 0xFF));
                    length >>= 8;
                    }
                var c = n.Count;
                target.WriteByte((Byte)(c | 0x80));
                for (var i = c - 1;i >= 0; i--) {
                    target.WriteByte(n[i]);
                    }
                }
            }

        #region M:IsNullOrEmpty<T>(ICollection<T>):Boolean
        protected static Boolean IsNullOrEmpty<T>(ICollection<T> source)
            {
            return (source == null) || (source.Count == 0);
            }
        #endregion

        protected static IEnumerable<Byte[]> HeaderSequence(Boolean explct, Asn1ObjectClass forceclass, Asn1ObjectType forcetype, Int64 length) {
            return HeaderSequence(
                explct,forceclass,
                (Byte)forcetype,
                length);
            }

        protected static IEnumerable<Byte[]> HeaderSequence(Boolean explct, Asn1ObjectClass forceclass, Byte forcetype, Int64 length) {
            var c = explct ? 0x20 : 0x00;
            c |= ((Byte)forceclass) << 6;
            c |= ((Byte)forcetype);
            yield return new []{ (Byte)c };
            if (length < 0) {
                yield return new[]{ (Byte)0x80 };
                yield break;
                }
            if (length < 0x80) {
                yield return new[]{ (Byte)length };
                yield break;
                }
            var r = new List<Byte>();
            while (length > 0) {
                r.Add((Byte)(length & 0xff));
                length >>= 8;
                }
            r.Reverse();
            yield return new[]{ (Byte)(r.Count | 0x80) };
            yield return r.ToArray();
            }

        protected internal virtual IEnumerable<Byte[]> ContentSequence { get {
            using (var r = new MemoryStream()) {
                Content.Seek(0, SeekOrigin.Begin);
                Content.CopyTo(r);
                yield return r.ToArray();
                }
            if (IsIndefiniteLength) { yield return new Byte[4]; }
            }}
        }
    }

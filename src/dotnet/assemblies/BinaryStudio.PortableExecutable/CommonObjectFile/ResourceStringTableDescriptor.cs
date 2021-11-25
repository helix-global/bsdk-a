using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable
    {
    internal class ResourceStringTableDescriptor : ResourceDescriptor, IDictionary<UInt32, String>
        {
        private readonly IDictionary<UInt32, String> values;

        internal ResourceStringTableDescriptor(ResourceDescriptor owner, ResourceIdentifier identifier)
            :base(owner, identifier)
            {
            if (owner.Identifier.Identifier != (Int32)IMAGE_RESOURCE_TYPE.RT_STRING) { throw new ArgumentOutOfRangeException(nameof(owner)); }
            values = new SortedDictionary<UInt32, String>();
            }

        internal ResourceStringTableDescriptor(ResourceDescriptor owner, ResourceIdentifier identifier, Byte[] source)
            :base(owner, identifier, source)
            {
            if (owner.Identifier.Identifier == null) { throw new ArgumentOutOfRangeException(nameof(owner)); }
            values = new SortedDictionary<UInt32, String>();
            unsafe
                {
                var id = (UInt32)((owner.Identifier.Identifier.Value - 1) << 4);
                fixed (Byte* r = source) {
                    var src = (UInt16*)r;
                    /* String Table is broken up into 16 string segments. */
                    for (var i = 0; i < 16; i++) {
                        var sz = *src++;
                        if (sz > 0) {
                            values.Add((UInt32)i | id, Encoding.Unicode.GetString((Byte*)src, sz*2));
                            src += sz;
                            }
                        }
                    }
                }
            }

        #region M:IEnumerable<KeyValuePair<UInt32,String>>.GetEnumerator:IEnumerator<KeyValuePair<UInt32,String>>
        IEnumerator<KeyValuePair<UInt32, String>> IEnumerable<KeyValuePair<UInt32, String>>.GetEnumerator() { return values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)values).GetEnumerator(); }
        #endregion
        #region M:ICollection<KeyValuePair<UInt32,String>>.Add(KeyValuePair<UInt32,String>)
        void ICollection<KeyValuePair<UInt32, String>>.Add(KeyValuePair<UInt32, String> item) {
            throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection);
            }
        #endregion
        #region M:ICollection<KeyValuePair<UInt32,String>>.Clear
        void ICollection<KeyValuePair<UInt32, String>>.Clear() {
            throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection);
            }
        #endregion
        #region M:ICollection<KeyValuePair<UInt32,String>>.Contains(KeyValuePair<UInt32,String>):Boolean
        Boolean ICollection<KeyValuePair<UInt32, String>>.Contains(KeyValuePair<UInt32, String> item) {
            return values.Contains(item);
            }
        #endregion
        #region M:ICollection<KeyValuePair<UInt32,String>>.CopyTo(KeyValuePair<UInt32,String>[],Int32)
        void ICollection<KeyValuePair<UInt32, String>>.CopyTo(KeyValuePair<UInt32, String>[] array, Int32 arrayIndex) {
            values.CopyTo(array, arrayIndex);
            }
        #endregion
        #region M:ICollection<KeyValuePair<UInt32,String>>.Remove(KeyValuePair<UInt32,String>):Boolean
        Boolean ICollection<KeyValuePair<UInt32, String>>.Remove(KeyValuePair<UInt32, String> item) {
            throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection);
            }
        #endregion
        #region M:IDictionary<UInt32,String>.ContainsKey(UInt32):Boolean
        Boolean IDictionary<UInt32, String>.ContainsKey(UInt32 key) {
            return values.ContainsKey(key);
            }
        #endregion
        #region M:IDictionary<UInt32,String>.Add(UInt32,String)
        void IDictionary<UInt32, String>.Add(UInt32 key, String value) {
            throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection);
            }
        #endregion
        #region M:IDictionary<UInt32, String>.Remove(UInt32):Boolean
        Boolean IDictionary<UInt32, String>.Remove(UInt32 key) {
            throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection);
            }
        #endregion
        #region M:IDictionary<UInt32,String>.TryGetValue(UInt32,out String):Boolean
        Boolean IDictionary<UInt32, String>.TryGetValue(UInt32 key, out String value) {
            return values.TryGetValue(key, out value);
            }
        #endregion
        #region P:ICollection<KeyValuePair<UInt32,String>>.Count:Int32
        Int32 ICollection<KeyValuePair<UInt32, String>>.Count {
            get { return values.Count; }
            }
        #endregion
        #region P:ICollection<KeyValuePair<UInt32,String>>.IsReadOnly:Boolean
        Boolean ICollection<KeyValuePair<UInt32, String>>.IsReadOnly {
            get { return true; }
            }
        #endregion
        #region P:IDictionary<UInt32,String>.this[UInt32]:String
        String IDictionary<UInt32, String>.this[UInt32 key] {
            get { return values[key]; }
            set { throw new NotSupportedException(Properties.Resources.NotSupported_ReadOnlyCollection); }
            }
        #endregion
        #region P:IDictionary<UInt32,String>.Keys:ICollection<UInt32>
        ICollection<UInt32> IDictionary<UInt32, String>.Keys {
            get { return values.Keys; }
            }
        #endregion
        #region P:IDictionary<UInt32,String>.Values:ICollection<String>
        ICollection<String> IDictionary<UInt32, String>.Values {
            get { return values.Values; }
            }
        #endregion

        internal void Merge(ResourceStringTableDescriptor descriptor) {
            if (descriptor == null) { throw new ArgumentNullException(nameof(descriptor)); }
            if (Source == null) { Source = new List<Byte[]>(); }
            Source.Add(descriptor.Source[0]);
            foreach (var item in descriptor) {
                values.Add(item.Key, item.Value);
                }
            }
        }
    }
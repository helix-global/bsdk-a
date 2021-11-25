using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    internal class ResourceMessageTableDescriptor : ResourceDescriptor, IDictionary<UInt32, String>
        {
        private readonly IDictionary<UInt32, String> values;

        private enum MESSAGE_RESOURCE_ENTRY_ENCODING : ushort
            {
            ANSI,
            UNICODE
            }

        /// <summary>
        /// The message-table entry descriptor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MESSAGE_RESOURCE_BLOCK
            {
            public readonly UInt32 LowId;                           /* First message identifier.    */
            public readonly UInt32 HighId;                          /* Last message identifier.     */
            public readonly UInt32 OffsetToEntries;                 /* Offset of the first message. */
            }

        /// <summary>
        /// The message-table string entry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct MESSAGE_RESOURCE_ENTRY
            {
            public readonly UInt16 Length;                          /* Size.  */
            public readonly MESSAGE_RESOURCE_ENTRY_ENCODING Flags;  /* Flags. */
            }

        internal ResourceMessageTableDescriptor(ResourceDescriptor owner, ResourceIdentifier identifier, Byte[] source)
            :base(owner, identifier, source)
            {
            values = new SortedDictionary<UInt32, String>();
            unsafe
                {
                fixed (Byte* r = source) {
                    if (*(Int32*)r > 0) {
                        var src = (MESSAGE_RESOURCE_BLOCK*)(r + 4);
                        for (var i = 0; i < *(Int32*)r; i++) {
                            var blocks = (MESSAGE_RESOURCE_ENTRY*)(r + src->OffsetToEntries);
                            for (var j = src->LowId; j <= src->HighId; j++) {
                                var sz = blocks->Length - 4;
                                var encoding = (blocks->Flags == MESSAGE_RESOURCE_ENTRY_ENCODING.UNICODE)
                                        ? Encoding.Unicode
                                        : Encoding.ASCII;
                                blocks++;
                                values.Add(j, encoding.GetString((Byte*)blocks, sz).Trim('\0').Trim('\r','\n'));
                                blocks = (MESSAGE_RESOURCE_ENTRY*)(((Byte*)blocks) + sz);
                                }
                            src++;
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
        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
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
        }
    }
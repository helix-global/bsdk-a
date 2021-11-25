using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal abstract class MSFTMetadataTableDescriptor : IDisposable
        {
        /// <summary>
        /// Segments in the type lib file.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
        protected struct SEGMENT
            {
            [FieldOffset( 0)] public  readonly Int32 Offset;
            [FieldOffset( 4)] public  readonly Int32 Length;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 84)]
        protected struct HEADER
            {
            [FieldOffset(32)] public readonly UInt32 TypeInfoCount;
            [FieldOffset(48)] public readonly UInt32 NameTableCount;
            }

        internal unsafe Byte* Source { get;private set; }
        protected unsafe HEADER* Header { get;private set; }
        protected Int64 Size { get;private set; }
        protected MetadataScope Scope { get;private set; }
        protected Boolean IsDisposed { get;private set; }

        protected unsafe MSFTMetadataTableDescriptor(MetadataScope scope, Byte* source, Int32 index) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            Header  = (HEADER*)source;
            var segment = (SEGMENT*)(((UInt32*)(Header + 1)) + (Int64)(Header->TypeInfoCount)) + index;
            Source = source + segment->Offset;
            Size = segment->Length;
            Scope = scope;
            }

        protected virtual unsafe void Dispose(Boolean disposing) {
            IsDisposed = true;
            if (disposing) {
                Size = 0;
                Source = null;
                Scope  = null;
                Header = null;
                }
            }

        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~MSFTMetadataTableDescriptor() {
            Dispose(false);
            }
        #endregion

        }

    internal abstract class MSFTMetadataTableDescriptor<T> : MSFTMetadataTableDescriptor
        {
        public abstract T this[Int32 i] { get; }

        protected readonly IDictionary<Int32, T> cache = new Dictionary<Int32, T>();
        protected unsafe MSFTMetadataTableDescriptor(MetadataScope scope, Byte* source, Int32 index)
            : base(scope, source, index)
            {
            }

        protected override void Dispose(Boolean disposing)
            {
            cache.Clear();
            base.Dispose(disposing);
            }

        public override String ToString()
            {
            return $"Count = {cache.Count}";
            }
        }
    }
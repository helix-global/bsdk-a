using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1LinkObject<T> : Asn1Object
        where T: Asn1Object
        {
        private T U;
        [Browsable(false)] public virtual T UnderlyingObject { get { return U; }}
        [Browsable(false)] public sealed override Asn1ObjectClass Class { get { return UnderlyingObject.Class; }}
        [Browsable(false)] public sealed override Int64 Offset { get { return UnderlyingObject.Offset; }}
        [Browsable(false)] public sealed override Int64 Length { get { return UnderlyingObject.Length; }}
        [Browsable(false)] public sealed override Int64 Size   { get { return UnderlyingObject.Size;   }}
        [Browsable(false)] public sealed override Int32 Count  { get { return UnderlyingObject.Count;  }}
        [Browsable(false)] public sealed override ReadOnlyMappingStream Content { get { return UnderlyingObject?.Content; }}
        [Browsable(false)] protected internal sealed override IEnumerable<Byte[]> ContentSequence { get { return UnderlyingObject.ContentSequence; }}

        protected Asn1LinkObject(T source)
            {
            if (ReferenceEquals(source, null)) { throw new ArgumentNullException(nameof(source)); }
            U = source;
            State = source.State | ObjectState.DisposeUnderlyingObject;
            }

        public override void WriteTo(Stream target)
            {
            UnderlyingObject.WriteTo(target);
            }

        #region P:IList<Asn1Object>.this[Int32]:Asn1Object
        /**
         * <summary>Gets or sets the element at the specified index.</summary>
         * <param name="index">The zero-based index of the element to get or set.</param>
         * <returns>The element at the specified index.</returns>
         * <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
         * <exception cref="NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
         * */
        public override Asn1Object this[Int32 index]
            {
            get { return UnderlyingObject[index];  }
            set { UnderlyingObject[index] = value; }
            }
        #endregion

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (State.HasFlag(ObjectState.DisposeUnderlyingObject)) { Dispose(ref U); }
            base.Dispose(disposing);
            }
        }

    public abstract class Asn1LinkObject : Asn1LinkObject<Asn1Object>
        {
        protected Asn1LinkObject(Asn1Object source)
            :base(source)
            {
            }
        }
    }

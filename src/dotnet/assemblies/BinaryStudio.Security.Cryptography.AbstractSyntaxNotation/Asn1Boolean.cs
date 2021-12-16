using System;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents a <see langword="BOOLEAN"/> type.
    /// </summary>
    /// <x:block xmlns:x="http://xmldoc.schemas.helix.global" x:lang="ru-RU">
    ///   <summary>
    ///   Передставляет собой описание типа <see langword="BOOLEAN"/>.
    ///   </summary>
    /// </x:block>
    public sealed class Asn1Boolean : Asn1UniversalObject
        {
        public Boolean Value { get;private set; }
        internal Asn1Boolean(ReadOnlyMappingStream source, Int64 forceoffset)
            : base(source, forceoffset)
            {
            }

        /// <summary>
        /// ASN.1 universal type. Always returns <see cref="Asn1ObjectType.Boolean"/>.
        /// </summary>
        public override Asn1ObjectType Type { get { return Asn1ObjectType.Boolean; }}
        public static implicit operator Boolean(Asn1Boolean source) {
            return source.Value;
            }

        protected internal  override Boolean Decode()
            {
            Value = Content.ReadByte() != 0;
            return true;
            }
        }
    }

using System;
using System.Diagnostics;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public abstract class CmsObject : Asn1LinkObject
        {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Length { get { return base.Length; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Offset { get { return base.Offset; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Size   { get { return base.Size;   }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int32 Count  { get { return base.Count;  }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected internal override Boolean IsDecoded { get { return base.IsDecoded; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsFailed  { get { return base.IsFailed;  }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsExplicitConstructed { get { return base.IsExplicitConstructed; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsImplicitConstructed { get { return base.IsImplicitConstructed; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsIndefiniteLength    { get { return base.IsIndefiniteLength;    }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1Object UnderlyingObject { get { return base.UnderlyingObject; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1ObjectClass Class { get { return base.Class; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override ReadOnlyMappingStream Content { get { return base.Content; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Byte[] Body { get { return base.Body; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal override ObjectState State
            {
            get { return base.State; }
            set { base.State = value; }
            }

        public CmsObject(Asn1Object source)
            : base(source)
            {
            }
        }
    }
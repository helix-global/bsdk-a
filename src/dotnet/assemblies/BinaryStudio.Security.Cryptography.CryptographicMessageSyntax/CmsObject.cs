using System;
using System.ComponentModel;
using System.Diagnostics;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public abstract class CmsObject : Asn1LinkObject
        {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] protected internal override Boolean IsDecoded { get { return base.IsDecoded; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsFailed  { get { return base.IsFailed;  }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsExplicitConstructed { get { return base.IsExplicitConstructed; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsImplicitConstructed { get { return base.IsImplicitConstructed; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsIndefiniteLength    { get { return base.IsIndefiniteLength;    }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1Object UnderlyingObject { get { return base.UnderlyingObject; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)][Browsable(false)] public override Byte[] Body { get { return base.Body; }}
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal override ObjectState State
            {
            get { return base.State; }
            set { base.State = value; }
            }

        protected CmsObject(Asn1Object source)
            : base(source)
            {
            }
        }
    }
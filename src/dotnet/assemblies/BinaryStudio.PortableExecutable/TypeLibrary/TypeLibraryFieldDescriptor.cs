using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    internal abstract class TypeLibraryFieldDescriptor : TypeLibraryMemberDescriptor, ITypeLibraryFieldDescriptor
        {
        protected TypeLibraryFieldDescriptor(ITypeLibraryTypeDescriptor type)
            : base(type)
            {
            }

        public abstract Int32 Id { get; }
        public abstract ITypeLibraryTypeDescriptor FieldType { get; }
        public abstract TypeLibVarFlags Flags { get; }
        public abstract TypeLibraryFieldAttributes Attributes { get; }
        public abstract Boolean IsLiteral { get; }
        public override TypeLibraryMemberTypes MemberType { get { return TypeLibraryMemberTypes.Field; }}
        public abstract Object LiteralValue { get; }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public virtual Boolean Equals(ITypeLibraryFieldDescriptor other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (Id == other.Id)
                && (MemberType == other.MemberType)
                && (String.Equals(Name, other.Name))
                && (Attributes == other.Attributes)
                && (Flags == other.Flags)
                && (Equals(FieldType, other.FieldType));
            }

        private String ToString(ITypeLibraryFixedArrayTypeDescriptor source) {
            return (source != null)
                ? source.Dimension.ToString()
                : String.Empty;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
            if (Flags != 0) {
                var attributes = new List<String>();
                if (Flags.HasFlag(TypeLibVarFlags.FReadOnly))        { attributes.Add("readonly");        }
                if (Flags.HasFlag(TypeLibVarFlags.FSource))          { attributes.Add("source");          }
                if (Flags.HasFlag(TypeLibVarFlags.FBindable))        { attributes.Add("bindable");        }
                if (Flags.HasFlag(TypeLibVarFlags.FRequestEdit))     { attributes.Add("requestedit");     }
                if (Flags.HasFlag(TypeLibVarFlags.FDisplayBind))     { attributes.Add("displaybind");     }
                if (Flags.HasFlag(TypeLibVarFlags.FDefaultBind))     { attributes.Add("defaultbind");     }
                if (Flags.HasFlag(TypeLibVarFlags.FHidden))          { attributes.Add("hidden");          }
                if (Flags.HasFlag(TypeLibVarFlags.FRestricted))      { attributes.Add("restricted");      }
                if (Flags.HasFlag(TypeLibVarFlags.FDefaultCollelem)) { attributes.Add("defaultcollelem"); }
                if (Flags.HasFlag(TypeLibVarFlags.FUiDefault))       { attributes.Add("uidefault");       }
                if (Flags.HasFlag(TypeLibVarFlags.FNonBrowsable))    { attributes.Add("nonbrowsable");    }
                if (Flags.HasFlag(TypeLibVarFlags.FReplaceable))     { attributes.Add("replaceable");     }
                if (Flags.HasFlag(TypeLibVarFlags.FImmediateBind))   { attributes.Add("immediatebind");   }
                if (attributes.Count > 0) {
                    r.AppendFormat("[{0}]", String.Join(",", attributes));
                    }
                }
            if (FieldType != null) {
                if (r.Length > 0) { r.Append(' '); }
                r.Append(FieldType.Name);
                }
            if (!String.IsNullOrWhiteSpace(Name)) {
                if (r.Length > 0) { r.Append(' '); }
                r.Append(Name);
                }
            if (FieldType != null) { r.Append(ToString(FieldType.FixedArrayTypeDescriptor)); }
            if (IsLiteral) { return $"{Name} = {LiteralValue}"; }
            return r.ToString();
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public sealed override Boolean Equals(ITypeLibraryMemberDescriptor other)
            {
            return base.Equals(other)
                && Equals(other as ITypeLibraryFieldDescriptor);
            }

        /**
         * <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
         * <param name="other">The object to compare with the current object. </param>
         * <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Boolean Equals(Object other)
            {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return Equals(other as ITypeLibraryFieldDescriptor);
            }

        /**
         * <summary>Serves as a hash function for a particular type. </summary>
         * <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Int32 GetHashCode()
            {
            return HashCodeCombiner.GetHashCode(
                base.GetHashCode(),
                Id,
                Name,
                FieldType);
            }
        }
    }
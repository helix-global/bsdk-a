using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    using CALLCONV = System.Runtime.InteropServices.ComTypes.CALLCONV;
    internal abstract class TypeLibraryMethodDescriptor : TypeLibraryMemberDescriptor, ITypeLibraryMethodDescriptor
        {
        public abstract TypeLibraryMethodAttributes Attributes { get; }
        public abstract ITypeLibraryTypeDescriptor ReturnType { get; }
        public abstract Int32 Id { get; }
        public abstract TypeLibFuncFlags Flags { get; }
        public abstract CALLCONV CallingConvention { get; }
        public virtual IList<ITypeLibraryParameterDescriptor> Parameters { get { return EmptyArray<ITypeLibraryParameterDescriptor>.Value; }}
        public override TypeLibraryMemberTypes MemberType { get { return TypeLibraryMemberTypes.Method; }}

        protected TypeLibraryMethodDescriptor(ITypeLibraryTypeDescriptor declaringtype)
            :base(declaringtype)
            {
            }

        private static String ToString(TypeLibFuncFlags source) {
            var r = new StringBuilder();
            var attributes = new List<String>();
            if (source.HasFlag(TypeLibFuncFlags.FRestricted))       { attributes.Add("restricted");       }
            if (source.HasFlag(TypeLibFuncFlags.FSource))           { attributes.Add("source");           }
            if (source.HasFlag(TypeLibFuncFlags.FBindable))         { attributes.Add("bindable");         }
            if (source.HasFlag(TypeLibFuncFlags.FRequestEdit))      { attributes.Add("requestedit");      }
            if (source.HasFlag(TypeLibFuncFlags.FDisplayBind))      { attributes.Add("displaybind");      }
            if (source.HasFlag(TypeLibFuncFlags.FDefaultBind))      { attributes.Add("defaultbind");      }
            if (source.HasFlag(TypeLibFuncFlags.FHidden))           { attributes.Add("hidden");           }
            if (source.HasFlag(TypeLibFuncFlags.FUsesGetLastError)) { attributes.Add("usesgetlasterror"); }
            if (source.HasFlag(TypeLibFuncFlags.FDefaultCollelem))  { attributes.Add("defaultcollelem");  }
            if (source.HasFlag(TypeLibFuncFlags.FUiDefault))        { attributes.Add("uidefault");        }
            if (source.HasFlag(TypeLibFuncFlags.FNonBrowsable))     { attributes.Add("nonbrowsable");     }
            if (source.HasFlag(TypeLibFuncFlags.FReplaceable))      { attributes.Add("replaceable");      }
            if (source.HasFlag(TypeLibFuncFlags.FImmediateBind))    { attributes.Add("immediatebind");    }
            if (attributes.Count > 0) { r.AppendFormat("[{0}]", String.Join(",", attributes)); }
            return r.ToString();
            }

        private static String ToString(CALLCONV source) {
            switch (source) {
                case CALLCONV.CC_CDECL:     { return "cdecl";   }
                case CALLCONV.CC_MSCPASCAL: { return "pascal";  }
                case CALLCONV.CC_MACPASCAL: { return "pascal";  }
                case CALLCONV.CC_STDCALL:   { return "stdcall"; }
                case CALLCONV.CC_SYSCALL:   { return "syscall"; }
                case CALLCONV.CC_MPWCDECL:  { return "cdecl";   }
                case CALLCONV.CC_MPWPASCAL: { return "pascal";  }
                }
            return String.Empty;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
            r.Append(ToString(CallingConvention));
            if (r.Length > 0) { r.Append(' '); }
            r.Append(ToString(Flags));
            if (r.Length > 0) { r.Append(' '); }
            r.Append(Name);
            var parameters = Parameters;
            if (parameters.Count > 0) {
                r.Append('(');
                r.Append(String.Join(",", parameters.Select(i => {
                    var builder = new StringBuilder();
                    var attributes = new List<String>();
                    if (i.IsIn)       { attributes.Add("in");       }
                    if (i.IsOut)      { attributes.Add("out");      }
                    if (i.IsRetval)   { attributes.Add("retval");   }
                    if (i.IsOptional) { attributes.Add("optional"); }
                    if (i.IsLcid)     { attributes.Add("lcid");     }
                    if (attributes.Count > 0) { builder.Append($"[{String.Join(",", attributes)}]"); }
                    if (i.ParameterType != null) {
                        if (builder.Length > 0) { builder.Append(' '); }
                        builder.Append(i.ParameterType);
                        }
                    return builder.ToString();
                    })));
                r.Append(')');
                }
            if (ReturnType != null) {
                r.Append(':');
                r.Append(ReturnType);
                }
            return r.ToString();
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public virtual Boolean Equals(ITypeLibraryMethodDescriptor other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true;  }
            return (Id == other.Id)
                && (MemberType == other.MemberType)
                && (String.Equals(Name, other.Name))
                && (Attributes == other.Attributes)
                && (Flags == other.Flags)
                && (Equals(ReturnType, other.ReturnType))
                && (Equals(Parameters, other.Parameters));
            }

        /**
         * <summary>Indicates whether the current object is equal to another object of the same type.</summary>
         * <param name="other">An object to compare with this object.</param>
         * <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
         * */
        public sealed override Boolean Equals(ITypeLibraryMemberDescriptor other)
            {
            return base.Equals(other)
                && Equals(other as ITypeLibraryMethodDescriptor);
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
            return Equals(other as ITypeLibraryMethodDescriptor);
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
                ReturnType);
            }
        }
    }
using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    using PARAMFLAG = System.Runtime.InteropServices.ComTypes.PARAMFLAG;
    internal abstract class TypeLibraryParameterDescriptor : ITypeLibraryParameterDescriptor
        {
        public abstract String Name { get; }
        public abstract PARAMFLAG Flags { get; }
        public virtual Boolean IsIn       { get { return Flags.HasFlag(PARAMFLAG.PARAMFLAG_FIN);     }}
        public virtual Boolean IsLcid     { get { return Flags.HasFlag(PARAMFLAG.PARAMFLAG_FLCID);   }}
        public virtual Boolean IsOptional { get { return Flags.HasFlag(PARAMFLAG.PARAMFLAG_FOPT);    }}
        public virtual Boolean IsOut      { get { return Flags.HasFlag(PARAMFLAG.PARAMFLAG_FOUT);    }}
        public virtual Boolean IsRetval   { get { return Flags.HasFlag(PARAMFLAG.PARAMFLAG_FRETVAL); }}
        public abstract ITypeLibraryTypeDescriptor ParameterType { get; }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            var r = new StringBuilder();
            var attributes = new List<String>();
            if (IsIn)       { attributes.Add("in");       }
            if (IsOut)      { attributes.Add("out");      }
            if (IsRetval)   { attributes.Add("retval");   }
            if (IsOptional) { attributes.Add("optional"); }
            if (IsLcid)     { attributes.Add("lcid");     }
            if (attributes.Count > 0) { r.AppendFormat("[{0}]", String.Join(",", attributes)); }
            if (ParameterType != null) {
                if (r.Length > 0) { r.Append(' '); }
                r.Append(ParameterType);
                }
            if (!String.IsNullOrWhiteSpace(Name)) {
                if (r.Length > 0) { r.Append(' '); }
                r.Append(Name);
                }
            return r.ToString();
            }
        }
    }
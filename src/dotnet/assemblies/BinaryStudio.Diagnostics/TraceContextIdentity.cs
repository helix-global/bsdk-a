using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BinaryStudio.Diagnostics
    {
    public abstract class TraceContextIdentity
        {
        public abstract String ShortName { get; }

        #region M:ToString(MethodBase):String
        protected static String ToString(MethodBase mi)
            {
            var r = new StringBuilder();
            if (mi.DeclaringType != null) {
                r.Append(mi.DeclaringType.Name);
                r.Append(".");
                }
            r.Append(mi.Name);
            var args = mi.GetParameters();
            if (args.Length > 0) {
                r.Append("(");
                #if NET35
                r.Append(String.Join(",", args.Select(i => ToString(i.ParameterType)).ToArray()));
                #else
                r.Append(String.Join(",", args.Select(i => ToString(i.ParameterType))));
                #endif
                r.Append(")");
                }
            return r.ToString();
            }
        #endregion
        #region M:ToString(Type):String
        protected static String ToString(Type type) {
            var r = new StringBuilder();
            #if !NET40 && !NET35
            if (type.IsConstructedGenericType) {
                var j = type.Name.IndexOf("`");
                r.Append(type.Name.Substring(0, j));
                r.Append('<');
                r.Append(String.Join(",", type.GenericTypeArguments.Select(ToString)));
                r.Append('>');
                }
            #else
            if (!type.IsGenericTypeDefinition && type.IsGenericType) {
                var j = type.Name.IndexOf("`");
                r.Append(type.Name.Substring(0, j));
                r.Append('<');
                #if NET35
                r.Append(String.Join(",", type.GetGenericArguments().Select(ToString).ToArray()));
                #else
                r.Append(String.Join(",", type.GetGenericArguments().Select(ToString)));
                #endif
                r.Append('>');
                }
            #endif
            else
                {
                r.Append(type.Name);
                }
            return r.ToString();
            }
        #endregion
        }
    }
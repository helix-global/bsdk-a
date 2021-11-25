#define FEATURE_ITYPELIBVIEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using IMPLTYPEFLAGS = System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS;
using PARAMFLAG = System.Runtime.InteropServices.ComTypes.PARAMFLAG;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryODLSerializer : TypeLibrarySerializer
        {
        private Int32 IndentLevel = 0;

        [Flags]
        private enum Flags
            {
            MethodParameterOnSeparatedLine  = 1,
            MethodAttributesOnSeparatedLine = 2,
            InterfaceOpenBrakeOnSameLine = 4,
            MethodAttributesWhiteSpace = 8,
            MethodCustomAttributesOnSeparatedLine = 16,
            IUnknownStdCall = 32,
            IUnknownNoDispId = 64
            }

        private Flags flags = Flags.MethodParameterOnSeparatedLine
            | Flags.MethodAttributesOnSeparatedLine
            | Flags.InterfaceOpenBrakeOnSameLine
            | Flags.MethodAttributesWhiteSpace
            | Flags.MethodCustomAttributesOnSeparatedLine
            | Flags.IUnknownStdCall
            | Flags.IUnknownNoDispId;

        private String IndentString { get { return new String(' ', IndentLevel*4); }}

        private IEnumerable<ITypeLibraryMethodDescriptor> MergeMethods(IEnumerable<ITypeLibraryMethodDescriptor> x, IEnumerable<ITypeLibraryPropertyDescriptor> y)
            {
            foreach (var i in x) { yield return i; }
            foreach (var i in y) {
                if (i.CanRead)  { yield return i.GetMethod; }
                if (i.CanWrite) { yield return i.SetMethod; }
                }
            }

        public override void Write(ITypeLibraryDescriptor source, TextWriter writer)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteLine("[");
            IndentLevel++;
            var values = new List<String>();
            values.Add($"uuid({source.UniqueIdentifier})");
            if (source.Version    != null) { values.Add($"version({source.Version.Major}.{source.Version.Minor})"); }
            if (source.HelpString != null) { values.Add($"helpstring(\"{source.HelpString}\")"); }
            foreach (var attribute in source.CustomAttributes)
                {
                values.Add(attribute.ToString());
                }
            writer.WriteLine(String.Join($",{Environment.NewLine}", values.Select(i => $"{IndentString}{i}")));
            IndentLevel--;
            writer.WriteLine("]");
            writer.WriteLine("library {0}", source.Name);
            writer.WriteLine("{");
            try
                {
                IndentLevel++;
                foreach (var lib in source.ImportedLibraries)
                    {
                    writer.WriteLine($"{IndentString}importlib(\"{lib.Name}.tlb\");");
                    }
                if (source.ImportedLibraries.Count > 0)
                    {
                    writer.WriteLine();
                    }

                foreach (var type in source.DefinedTypes) {
                    if (type.IsInterface || type.IsDispatch) {
                        writer.WriteLine($"{IndentString}interface {type.Name};");
                        }
                    }
                writer.WriteLine();
                foreach (var type in source.DefinedTypes) {
                    writer.WriteLine();
                    if (type.IsEnum)
                        {
                        writer.WriteLine($"{IndentString}typedef ");
                        }
                    values.Clear();
                    if (type.IsInterface || type.IsDispatch)
                        {
                        values.Add("odl");
                        }
                    if (type.UniqueIdentifier != null) { values.Add($"uuid({type.UniqueIdentifier.Value})"); }
                    if (type.Version != null) { values.Add($"version({type.Version.Major}.{type.Version.Minor})"); }
                    if (type.HelpString != null) { values.Add($"helpstring(\"{type.HelpString}\")"); }
                    if (type.IsInterface || type.IsDispatch) {
                        if (type.Flags.HasFlag(TypeLibTypeFlags.FHidden))        { values.Add("hidden");        }
                        if (type.Flags.HasFlag(TypeLibTypeFlags.FDual))          { values.Add("dual");          }
                        if (type.Flags.HasFlag(TypeLibTypeFlags.FOleAutomation)) { values.Add("oleautomation"); }
                        }
                    if (type.IsClass)
                        {
                        if (!type.Flags.HasFlag(TypeLibTypeFlags.FCanCreate))    { values.Add("noncreatable");  }
                        }
                    foreach (var attribute in type.CustomAttributes) {
                        values.Add(attribute.ToString());
                        }
                    if (values.Count > 0)
                        {
                        writer.WriteLine($"{IndentString}[");
                        IndentLevel++;
                        writer.WriteLine(String.Join($",{Environment.NewLine}", values.Select(i => $"{IndentString}{i}")));
                        #if FEATURE_ITYPELIBVIEWER
                        if (type.IsInterface || type.IsDispatch)
                            {
                            writer.WriteLine();
                            }
                        #endif
                        IndentLevel--;
                        writer.WriteLine($"{IndentString}]");
                        }
                    writer.Write(IndentString);
                         if (type.IsClass)     { writer.Write("coclass"); }
                    else if (type.IsInterface) { writer.Write("interface"); }
                    else if (type.IsDispatch)  { writer.Write("interface"); }
                    else if (type.IsEnum)      { writer.Write("enum"); }
                    else if (type.IsStructure) { writer.Write("struct"); }
                    writer.Write(' ');
                    if (!type.IsEnum)
                        {
                        if (type.IsStructure)
                            {
                            writer.Write($"tag{type.Name}");
                            }
                        else
                            {
                            writer.Write(type.Name);
                            }
                        }
                    
                    if (type.IsInterface || type.IsDispatch) {
                        if (type.BaseType != null) {
                            writer.Write($" : {type.BaseType.Name}");
                            }
                        }
                    if (flags.HasFlag(Flags.InterfaceOpenBrakeOnSameLine))
                        {
                        writer.Write(" {");
                        writer.WriteLine($"{IndentString}");    
                        }
                    else
                        {
                        writer.WriteLine();
                        writer.WriteLine($"{IndentString}{{");    
                        }
                    
                    IndentLevel++;
                    if (type.IsClass) {
                        values.Clear();
                        writer.WriteLine("{0};",String.Join($";{Environment.NewLine}", type.ImplementedInterfaces.Select(i => $"{IndentString}{ToString(i.Flags)}interface {i.Type.Name}")));
                        }
                    if (type.IsInterface || type.IsDispatch) {
                        foreach (var mi in MergeMethods(type.DeclaredMethods, type.DeclaredProperties).OrderBy(i => i.Id)) {
                            writer.Write(IndentString);
                            WriteMethod(writer, mi);
                            writer.WriteLine();
                            }
                        }
                    else if (type.IsEnum) {
                        var i = 0;
                        var c = type.DeclaredFields.Count;
                        foreach (var field in type.DeclaredFields) {
                            writer.Write($"{IndentString}{field.Name} = {field.LiteralValue}");
                            i++;
                            if (i != c) { writer.Write(','); }
                            writer.WriteLine();
                            }
                        }
                    else if (type.IsStructure) {
                        foreach (var field in type.DeclaredFields) {
                            writer.Write($"{IndentString}{BuildParameterTypeName(field.FieldType)} {field.Name};");
                            writer.WriteLine();
                            }
                        }
                    IndentLevel--;
                    if (type.IsEnum || type.IsStructure)
                        {
                        writer.Write($"{IndentString}}} ");
                        writer.WriteLine($"{type.Name};");
                        }
                    else
                        {
                        writer.WriteLine($"{IndentString}}};");
                        }
                    }
                }
            finally
                {
                IndentLevel--;
                }
            writer.WriteLine("}");
            }

        private String BuildMethodParameter(ITypeLibraryParameterDescriptor source)
            {
            var r = new StringBuilder();
            r.Append(ToString(source.Flags));
            r.Append(BuildParameterTypeName(source.ParameterType));
            r.Append(' ');
            r.Append(source.Name);
            return r.ToString();
            }

        private String BuildParameterTypeName(ITypeLibraryTypeDescriptor source)
            {
            switch (source.Name)
                {
                case "bool"                : { return "VARIANT_BOOL";    }
                case "bool*"               : { return "VARIANT_BOOL*";   }
                case "DateTime"            : { return "DATE";            }
                case "DateTime*"           : { return "DATE*";           }
                case "byte"                : { return "unsigned char";   }
                case "byte*"               : { return "unsigned char*";  }
                case "long long"           : { return "int64";           }
                case "long long*"          : { return "int64*";          }
                case "unsigned long long"  : { return "uint64";          }
                case "unsigned long long*" : { return "uint64*";         }
                case "float"               : { return "single";          }
                case "float*"              : { return "single*";         }
                case "string"              : { return "BSTR";            }
                case "string*"             : { return "BSTR*";           }
                case "SAFEARRAY(string)"   : { return "SAFEARRAY(BSTR)"; }
                //case "" : { return ""; }
                //case "" : { return ""; }
                //case "" : { return ""; }
                }
            return source.Name;
            }

        private void WriteMethod(TextWriter writer, ITypeLibraryMethodDescriptor source)
            {
            var separator = ", ";
            var attributes = new List<String>();
            //if (source.Id > 0)
                {
                if (!flags.HasFlag(Flags.IUnknownNoDispId) || !source.DeclaringType.IsInterface)
                    {
                    attributes.Add($"id(0x{source.Id:X8})");    
                    }
                }
            attributes.AddRange(ToArray(source.Attributes));
            if (flags.HasFlag(Flags.MethodCustomAttributesOnSeparatedLine))
                {
                attributes.AddRange(ToArray(source.CustomAttributes).Select(i => $"{Environment.NewLine}{IndentString}    {i}"));    
                }
            else
                {
                attributes.AddRange(ToArray(source.CustomAttributes));    
                }
            writer.Write(ToString(attributes));
            if (flags.HasFlag(Flags.MethodAttributesOnSeparatedLine)) {
                if (attributes.Count > 0) {
                    writer.WriteLine();
                    writer.Write(IndentString);
                    }
                }
            writer.Write(source.ReturnType.Name);
            if (flags.HasFlag(Flags.IUnknownStdCall) && source.DeclaringType.IsInterface)
                {
                writer.Write(' ');
                writer.Write("_stdcall");
                }
            writer.Write(' ');
            writer.Write(source.Name);
            writer.Write('(');
            if (flags.HasFlag(Flags.MethodParameterOnSeparatedLine)) {
                if (source.Parameters.Count > 1) {
                    writer.WriteLine();
                    writer.Write($"{IndentString}      ");
                    }
                separator = $",{Environment.NewLine}{IndentString}      ";    
                }

            writer.Write(String.Join(separator, source.Parameters.Select(BuildMethodParameter)));
            writer.Write(");");
            }

        #region M:ToString(IMPLTYPEFLAGS):String
        private static String ToString(IMPLTYPEFLAGS flags)
            {
            var r = ToArray(flags);
            return (r.Count > 0)
                ? $"[{String.Join(",", r)}] "
                : String.Empty;
            }
        #endregion
        #region M:ToString(PARAMFLAG):String
        private String ToString(PARAMFLAG flags)
            {
            return ToString(ToArray(flags));
            }
        #endregion
        #region M:ToString(ICollection<String>):String
        private String ToString(ICollection<String> value)
            {
            return (value.Count > 0)
                ? flags.HasFlag(Flags.MethodAttributesWhiteSpace)
                    ? $"[{String.Join(", ", value)}] "
                    : $"[{String.Join(",", value)}] "
                : String.Empty;
            }
        #endregion

        #region M:ToArray(IMPLTYPEFLAGS):IList<String>
        private static IList<String> ToArray(IMPLTYPEFLAGS flags)
            {
            var r = new List<String>();
            if (flags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FDEFAULT))    { r.Add("default");    }
            if (flags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FRESTRICTED)) { r.Add("restricted"); }
            if (flags.HasFlag(IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE))     { r.Add("source");     }
            return r;
            }
        #endregion
        #region M:ToArray(PARAMFLAG):IList<String>
        private static IList<String> ToArray(PARAMFLAG flags)
            {
            var r = new List<String>();
            if (flags.HasFlag(PARAMFLAG.PARAMFLAG_FIN))     { r.Add("in");       }
            if (flags.HasFlag(PARAMFLAG.PARAMFLAG_FOUT))    { r.Add("out");      }
            if (flags.HasFlag(PARAMFLAG.PARAMFLAG_FLCID))   { r.Add("lcid");     }
            if (flags.HasFlag(PARAMFLAG.PARAMFLAG_FRETVAL)) { r.Add("retval");   }
            if (flags.HasFlag(PARAMFLAG.PARAMFLAG_FOPT))    { r.Add("optional"); }
            return r;
            }
        #endregion
        #region M:ToArray(TypeLibraryMethodAttributes):IList<String>
        private static IList<String> ToArray(TypeLibraryMethodAttributes flags)
            {
            var r = new List<String>();
            if (flags.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYGET))    { r.Add("propget");    }
            if (flags.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUT))    { r.Add("propput");    }
            if (flags.HasFlag(TypeLibraryMethodAttributes.INVOKE_PROPERTYPUTREF)) { r.Add("propputref"); }
            return r;
            }
        #endregion
        #region M:ToArray(IEnumerable<TypeLibraryCustomAttribute>):IList<String>
        private static IList<String> ToArray(IEnumerable<TypeLibraryCustomAttribute> attributes)
            {
            return attributes.Select(i => i.ToString()).ToArray();
            }
        #endregion
        }
    }
//#define FEATURE_ITYPELIBVIEWER
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
    public class TypeLibraryIDLSerializer : TypeLibrarySerializer
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
        private Boolean IncludeCustomAttribute { get { return false; }}
        private Boolean MethodParameterOnSeparatedLine { get { return true; }}

        private IEnumerable<ITypeLibraryMethodDescriptor> MergeMethods(IEnumerable<ITypeLibraryMethodDescriptor> x, IEnumerable<ITypeLibraryPropertyDescriptor> y)
            {
            foreach (var i in x) { yield return i; }
            foreach (var i in y) {
                if (i.CanRead)  { yield return i.GetMethod; }
                if (i.CanWrite) { yield return i.SetMethod; }
                }
            }
        private IEnumerable<ITypeLibraryMethodDescriptor> MergeMethods(IEnumerable<ITypeLibraryMethodDescriptor> x, IEnumerable<ITypeLibraryMethodDescriptor> y)
            {
            //foreach (var i in x) { yield return i; }
            foreach (var i in y) { yield return i; }
            }

        private static IEnumerable<ITypeLibraryTypeDescriptor> OrderedTypes(IEnumerable<ITypeLibraryTypeDescriptor> types)
            {
            var r = types.ToArray();
            foreach (var i in r.Where(i => i.IsEnum)) { yield return i; }
            foreach (var i in r.Where(i => i.IsStructure )) { yield return i; }
            foreach (var i in r.Where(i => !i.IsStructure && !i.IsEnum)) { yield return i; }
            }

        public override void Write(ITypeLibraryDescriptor source, TextWriter writer)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteLine("[");
            IndentLevel++;
            var values = new List<String>();
            values.Add($"uuid({source.UniqueIdentifier:D})");
            if (source.Version    != null) { values.Add($"version({source.Version.Major}.{source.Version.Minor})"); }
            if (source.HelpString != null) { values.Add($"helpstring(\"{source.HelpString}\")"); }
            if (IncludeCustomAttribute) {
                foreach (var attribute in source.CustomAttributes) {
                    values.Add(attribute.ToString());
                    }
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
                    if (type.IsInterface) {
                        writer.WriteLine($"{IndentString}interface {type.Name};");
                        }
                    else if (type.IsDispatch) {
                        writer.WriteLine($"{IndentString}dispinterface {type.Name};");
                        }
                    }
                writer.WriteLine();
                foreach (var type in OrderedTypes(source.DefinedTypes)) {
                    writer.WriteLine();
                    if (type.IsEnum || type.IsStructure)
                        {
                        writer.WriteLine($"{IndentString}typedef ");
                        }
                    values.Clear();
                    if (type.UniqueIdentifier != null) { values.Add($"uuid({type.UniqueIdentifier.Value:D})"); }
                    if (type.Version != null) { values.Add($"version({type.Version.Major}.{type.Version.Minor})"); }
                    if (type.HelpString != null) { values.Add($"helpstring(\"{type.HelpString}\")"); }
                    if (type.IsInterface || type.IsDispatch) {
                        if (type.Flags.HasFlag(TypeLibTypeFlags.FHidden))        { values.Add("hidden");        }
                        //if (type.Flags.HasFlag(TypeLibTypeFlags.FDual))          { values.Add("dual");          }
                        //if (type.Flags.HasFlag(TypeLibTypeFlags.FOleAutomation)) { values.Add("oleautomation"); }
                        }
                    if (type.IsClass)
                        {
                        if (!type.Flags.HasFlag(TypeLibTypeFlags.FCanCreate))    { values.Add("noncreatable");  }
                        }
                    if (IncludeCustomAttribute) {
                        foreach (var attribute in type.CustomAttributes) {
                            values.Add(attribute.ToString());
                            }
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
                    else if (type.IsDispatch)  { writer.Write("dispinterface"); }
                    else if (type.IsEnum)      { writer.Write("enum"); }
                    else if (type.IsStructure) { writer.Write("struct"); }
                    writer.Write(' ');
                    writer.Write(type.Name);
                    
                    if (type.IsInterface) {
                        if (type.BaseType != null) {
                            writer.Write($" : {type.BaseType.Name}");
                            }
                        }
                    #if FEATURE_ITYPELIBVIEWER
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
                    #else
                    writer.WriteLine();
                    writer.WriteLine($"{IndentString}{{");    
                    #endif
                    
                    IndentLevel++;
                    if (type.IsClass) {
                        values.Clear();
                        writer.WriteLine("{0};",String.Join($";{Environment.NewLine}",
                            type.ImplementedInterfaces.Select(i =>
                                i.Type.IsDispatch
                                    ? $"{IndentString}{ToString(i.Flags)}dispinterface {i.Type.Name}"
                                    : $"{IndentString}{ToString(i.Flags)}interface {i.Type.Name}")));
                        }
                    if (type.IsInterface) {
                        foreach (var mi in MergeMethods(type.DeclaredMethods, type.DeclaredProperties).OrderBy(i => i.Id)) {
                            writer.Write(IndentString);
                            WriteMethod(writer, mi, true);
                            writer.WriteLine();
                            }
                        }
                    if (type.IsDispatch)
                        {
                        writer.WriteLine($"{IndentString}properties:");
                        IndentLevel++;
                        foreach (var pi in type.DeclaredProperties) {
                            writer.Write(IndentString);
                            WriteProperty(writer, pi);
                            writer.WriteLine();
                            }
                        IndentLevel--;
                        writer.WriteLine($"{IndentString}methods:");
                        IndentLevel++;
                        foreach (var mi in type.DeclaredMethods) {
                            writer.Write(IndentString);
                            WriteMethod(writer, mi, false);
                            writer.WriteLine();
                            }
                        IndentLevel--;
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

        private void WriteProperty(TextWriter writer, ITypeLibraryPropertyDescriptor source)
            {
            var attributes = new List<String>();
            attributes.Add($"id(0x{source.Id:X8})");
            attributes.AddRange(ToArray(source.CustomAttributes));
            writer.Write(ToString(attributes));
            writer.Write(BuildParameterTypeName(source.PropertyType));
            writer.Write(" ");
            writer.Write(source.Name);
            writer.Write(";");
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
                case "float"               : { return "float";          }
                case "float*"              : { return "float*";         }
                case "string"              : { return "BSTR";            }
                case "string*"             : { return "BSTR*";           }
                case "SAFEARRAY(string)"   : { return "SAFEARRAY(BSTR)"; }
                //case "" : { return ""; }
                //case "" : { return ""; }
                //case "" : { return ""; }
                }
            return source.Name;
            }

        private void WriteMethod(TextWriter writer, ITypeLibraryMethodDescriptor source, Boolean hasreturn)
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
            #if FEATURE_ITYPELIBVIEWER
            if (flags.HasFlag(Flags.MethodCustomAttributesOnSeparatedLine))
                {
                attributes.AddRange(ToArray(source.CustomAttributes).Select(i => $"{Environment.NewLine}{IndentString}    {i}"));    
                }
            else
                {
                attributes.AddRange(ToArray(source.CustomAttributes));    
                }
            #else
            attributes.AddRange(ToArray(source.CustomAttributes));    
            #endif
            writer.Write(ToString(attributes));
            #if FEATURE_ITYPELIBVIEWER
            if (flags.HasFlag(Flags.MethodAttributesOnSeparatedLine)) {
                if (attributes.Count > 0) {
                    writer.WriteLine();
                    writer.Write(IndentString);
                    }
                }
            #endif
            if (hasreturn || (source.ReturnType.Name != "HRESULT"))
                {
                WriteMethodWithReturnType(writer, source);
                }
            else
                {
                var parameters = source.Parameters;
                var c = parameters.Count;
                if (c == 0) { writer.Write($"void {source.Name}();"); }
                else
                    {
                    var lastpar = parameters[c - 1];
                    var typeref = lastpar.ParameterType;
                    if (lastpar.IsRetval) {
                        if (typeref.IsPointer) {
                            typeref = typeref.UnderlyingType;
                            writer.Write($"{BuildParameterTypeName(typeref)} {source.Name}(");
                            if (c == 2)
                                {
                                writer.Write(BuildMethodParameter(parameters[0]));
                                }
                            else if (c > 2)
                                {
                                if (MethodParameterOnSeparatedLine) {
                                    writer.WriteLine();
                                    writer.Write($"{IndentString}      ");
                                    separator = $",{Environment.NewLine}{IndentString}      ";
                                    }
                                var nparameters = new List<ITypeLibraryParameterDescriptor>();
                                for (var i = 0; i < c - 1; i++) {
                                    nparameters.Add(parameters[i]);
                                    }
                                writer.Write(String.Join(separator, nparameters.Select(BuildMethodParameter)));
                                }
                            writer.Write(");");
                            }
                        else
                            {
                            WriteMethodWithReturnType(writer, source);
                            }
                        }
                    else
                        {
                        writer.Write($"void {source.Name}(");
                        if (MethodParameterOnSeparatedLine) {
                            if (source.Parameters.Count > 1) {
                                writer.WriteLine();
                                writer.Write($"{IndentString}      ");
                                }
                            separator = $",{Environment.NewLine}{IndentString}      ";    
                            }
                        writer.Write(String.Join(separator, source.Parameters.Select(BuildMethodParameter)));
                        writer.Write(");");
                        }
                    }
                }
            }

        private void WriteMethodWithReturnType(TextWriter writer, ITypeLibraryMethodDescriptor source)
            {
            var separator = ", ";
            writer.Write(source.ReturnType.Name);
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
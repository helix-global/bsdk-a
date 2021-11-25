using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BinaryStudio.PortableExecutable
    {
    public class TypeLibraryDelphiSerializer : TypeLibrarySerializer
        {
        private Int32 IndentLevel = 0;
        private Guid FQN = new Guid("0F21F359-AB84-41E8-9A78-36D110E6D2F9");
        private String IndentString { get { return new String(' ', IndentLevel*2); }}
        public override void Write(ITypeLibraryDescriptor source, TextWriter writer)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteLine("unit {0}_TLB;", source.Name);
            writer.WriteLine();
            writer.WriteLine("// ************************************************************************ //");
            writer.WriteLine("// WARNING                                                                    ");
            writer.WriteLine("// -------                                                                    ");
            writer.WriteLine("// The types declared in this file were generated from data read from a       ");
            writer.WriteLine("// Type Library. If this type library is explicitly or indirectly (via        ");
            writer.WriteLine("// another type library referring to this type library) re-imported, or the   ");
            writer.WriteLine("// 'Refresh' command of the Type Library Editor activated while editing the   ");
            writer.WriteLine("// Type Library, the contents of this file will be regenerated and all        ");
            writer.WriteLine("// manual modifications will be lost.                                         ");
            writer.WriteLine("// ************************************************************************ //");
            writer.WriteLine();
            writer.WriteLine("interface");
            writer.WriteLine();
            writer.WriteLine("uses Windows, ActiveX, Classes, Graphics, OleServer, StdVCL, Variants;");
            writer.WriteLine();
            writer.WriteLine("const");
            IndentLevel++;
            writer.WriteLine("{0}{1}MajorVersion = {2};", IndentString, source.Name, source.Version.Major);
            writer.WriteLine("{0}{1}MinorVersion = {2};", IndentString, source.Name, source.Version.Minor);
            IndentLevel--;
            writer.WriteLine();
            var n = source.Name.Length + 6;
            foreach (var type in source.DefinedTypes) {
                     if (type.IsInterface) { n = Math.Max(n, type.Name.Length + 4); }
                else if (type.IsClass)     { n = Math.Max(n, type.Name.Length + 6); }
                }
            IndentLevel++;
            writer.WriteLine("{0}{1}:TGUID = '{2:B}';", IndentString, $"LIBID_{source.Name}", source.UniqueIdentifier);
            foreach (var type in source.DefinedTypes) {
                String name = null;
                     if (type.IsInterface) { name = $"IID_{type.Name}";   }
                else if (type.IsClass)     { name = $"CLASS_{type.Name}"; }
                if (name != null) {
                    writer.WriteLine("{0}{1}{2}: TGUID = '{3:B}';", IndentString, name, new String(' ', n - name.Length), type.UniqueIdentifier.Value);
                    }
                }
            IndentLevel--;
            if (source.DefinedTypes.Any(i => i.IsEnum)) {
                writer.WriteLine();
                writer.WriteLine("(**");
                writer.WriteLine(" * Declaration of Enumerations defined in Type Library");
                writer.WriteLine(" *)");
                writer.WriteLine();
                foreach (var type in source.DefinedTypes.Where(i => i.IsEnum)) {
                    var values = new List<String> {$"enum:{type.Name}"};
                    if (type.UniqueIdentifier != null) { values.Add($"uuid({type.UniqueIdentifier.Value:D})");              }
                    if (type.Version != null)          { values.Add($"version({type.Version.Major}.{type.Version.Minor})"); }
                    if (type.HelpString != null)       { values.Add($"helpstring(\"{type.HelpString}\")");                  }
                    writer.WriteLine("(**");
                    writer.WriteLine(String.Join(Environment.NewLine, values.Select(i => $" * {i}")));
                    writer.WriteLine(" *)");
                    writer.WriteLine("type");
                    IndentLevel++;
                    writer.WriteLine("{0}{1} = TOleEnum;", IndentString, type.Name);
                    IndentLevel--;
                    if (type.DeclaredFields.Count > 0) {
                        writer.WriteLine("const");
                        IndentLevel++;
                        n = type.DeclaredFields.Max(i => i.Name.Length);
                        foreach (var field in type.DeclaredFields) {
                            writer.WriteLine("{0}{1}{2} = ${3:X8};", IndentString,
                                field.Name,
                                new String(' ', n - field.Name.Length),
                                field.LiteralValue);
                            }
                        IndentLevel--;
                        }
                    writer.WriteLine();
                    }
                }
            if (source.DefinedTypes.Any(i => i.IsDispatch || i.IsInterface))
                {
                writer.WriteLine();
                writer.WriteLine("(**");
                writer.WriteLine(" * Forward declaration of types defined in TypeLibrary");
                writer.WriteLine(" *)");
                writer.WriteLine();
                writer.WriteLine("type");
                IndentLevel++;
                n = source.DefinedTypes.Where(i => i.IsDispatch || i.IsInterface).Max(i => i.Name.Length);
                foreach (var type in source.DefinedTypes.Where(i => i.IsDispatch || i.IsInterface).OrderBy(i => i.Name)) {
                    if (type.IsInterface)
                        {
                        writer.WriteLine("{0}{1}{2} = interface;", IndentString, type.Name, new String(' ', n - type.Name.Length));
                        }
                    else
                        {
                        writer.WriteLine("{0}{1}{2} = interface;", IndentString, type.Name, new String(' ', n - type.Name.Length));
                        //writer.WriteLine("{0}{1}Disp = dispinterface;", IndentString, type.Name);
                        }
                    }
                IndentLevel--;
                }

            writer.WriteLine();
            if (source.DefinedTypes.Any(i => i.IsStructure)) {
                writer.WriteLine();
                IndentLevel++;
                foreach (var type in Order(source.DefinedTypes.Where(i => i.IsStructure).OrderBy(i => i.Name))) {
                    writer.WriteLine("{0}(**", IndentString);
                    writer.WriteLine("{0} * Structure: {1}",  IndentString, type.Name);
                    writer.WriteLine("{0} * GUID: {1:B}",  IndentString, type.UniqueIdentifier.GetValueOrDefault());
                    var attribute = type.CustomAttributes.FirstOrDefault(i => i.UniqueIdentifier == FQN);
                    if (attribute != null)
                        {
                        writer.WriteLine("{0} * .NET: {1}",  IndentString, attribute.Value);
                        }
                    writer.WriteLine("{0} *)", IndentString);
                    writer.WriteLine("{0}{1} = packed record", IndentString, MapTypeName(type.Name, attribute?.Value.ToString()));
                    IndentLevel++;
                    foreach (var fi in type.DeclaredFields) {
                        writer.Write("{0}{1}: ", IndentString, MapParameterName(fi.Name));
                        WriteType(writer, fi.FieldType);
                        writer.WriteLine(";");
                        }
                    IndentLevel--;
                    writer.WriteLine("{0}end;", IndentString);
                    writer.WriteLine();
                    }
                IndentLevel--;
                }
            if (source.DefinedTypes.Any(i => i.IsDispatch || i.IsInterface))
                {
                writer.WriteLine();
                IndentLevel++;
                foreach (var type in source.DefinedTypes.Where(i => i.IsDispatch || i.IsInterface).OrderBy(i => i.Name)) {
                    if (type.IsInterface || type.IsDispatch)
                        {
                        writer.WriteLine("{0}(**", IndentString);
                        writer.WriteLine("{0} * Interface: {1}",  IndentString, type.Name);
                        writer.WriteLine("{0} * GUID: {1:B}",  IndentString, type.UniqueIdentifier.GetValueOrDefault());
                        var attribute = type.CustomAttributes.FirstOrDefault(i => i.UniqueIdentifier == FQN);
                        if (attribute != null)
                            {
                            writer.WriteLine("{0} * .NET: {1}",  IndentString, attribute.Value);
                            }
                        writer.WriteLine("{0} *)", IndentString);
                        writer.WriteLine("{0}{1} = interface({2})", IndentString, type.Name, type.BaseType.Name);
                        IndentLevel++;
                        writer.WriteLine("{0}['{1:B}']", IndentString, type.UniqueIdentifier.GetValueOrDefault());
                        foreach (var pi in type.DeclaredProperties) { WriteMethod(writer, pi); }
                        foreach (var mi in type.DeclaredMethods)    { WriteMethod(writer, mi, String.Empty); }
                        foreach (var pi in type.DeclaredProperties) { WriteProperty(writer, pi); }
                        IndentLevel--;
                        writer.WriteLine("{0}end;", IndentString);
                        writer.WriteLine();
                        }
                    else
                        {
                        }
                    }
                IndentLevel--;
                }
            writer.WriteLine();
            writer.WriteLine("implementation");
            writer.WriteLine();
            writer.WriteLine("uses ComObj;");
            writer.WriteLine();
            writer.WriteLine("end.");
            }

        private void WriteType(TextWriter writer, ITypeLibraryTypeDescriptor source) {
            if (source.IsPointer) {
                var type = source.UnderlyingType;
                if (type.IsDispatch || type.IsInterface) {
                    WriteType(writer, type);
                    return;
                    }
                }
            if (source.IsArray)
                {
                writer.Write("PSafeArray");
                return;
                }
            switch (source.Name)
                {
                case "string"         : { writer.Write("WideString"); } break;
                case "bool"           : { writer.Write("Boolean");    } break;
                case "GUID"           : { writer.Write("TGuid");      } break; 
                case "unsigned long"  : { writer.Write("LongWord");   } break;
                case "unsigned short" : { writer.Write("Word");       } break;
                case "long"           : { writer.Write("Integer");    } break;
                case "short"          : { writer.Write("SmallInt");   } break;
                case "VARIANT"        : { writer.Write("Variant");    } break; 
                case "DateTime"       : { writer.Write("TDateTime");  } break;
                case "float"          : { writer.Write("Single");     } break;
                case "double"         : { writer.Write("Double");     } break;
                case "char"           : { writer.Write("ShortInt");   } break;
                case "char*"          : { writer.Write("PChar");   } break;
                case "void*"          : { writer.Write("Pointer");   } break;
                case "byte"           : { writer.Write("Byte");       } break;
                case "decimal"        : { writer.Write("TDecimal");   } break;
                case "long long"      : { writer.Write("Int64");      } break;
                case "unsigned long long"      : { writer.Write("LargeUInt"); } break;
                default:
                    {
                    writer.Write(source.Name);
                    }
                    break;
                }
            }

        private String MapParameterName(String source)
            {
            if (source == null) { return "AValue"; }
            switch (source.ToUpper())
                {
                case "TYPE"  : { return "FType";  }
                case "ARRAY" : { return "FArray"; }
                case "OR"    : { return "or_";    }
                default: { return source; }
                }
            }

        private String MapTypeName(String source, String alt)
            {
            if (alt != null) { alt = alt.Replace('.', '_'); }
            switch (source.ToUpper())
                {
                case "LABEL"   : { return alt ?? "_Label";   }
                case "BYTE"    : { return alt ?? "_Byte";    }
                case "BOOLEAN" : { return alt ?? "_Boolean"; }
                case "CHAR"    : { return alt ?? "_Char";    }
                case "GUID"    : { return alt ?? "_Guid";    }
                case "INT64"   : { return alt ?? "_Int64";   }
                case "SINGLE"  : { return alt ?? "_Single";  }
                case "DOUBLE"  : { return alt ?? "_Double";  }
                default: { return source; }
                }
            }

        private void WriteParameter(TextWriter writer, ITypeLibraryParameterDescriptor source) {
            var prefix = String.Empty;
            var type = source.ParameterType;
            if (type.IsPointer) {
                if (type.UnderlyingType.IsDispatch || type.UnderlyingType.IsInterface) {
                    type = type.UnderlyingType;
                    }
                else
                    {
                    type = type.UnderlyingType;
                    prefix = source.IsIn
                        ? "var "
                        : "out ";
                    }
                }

            writer.Write(prefix);
            writer.Write(MapParameterName(source.Name));
            writer.Write(":");
            switch (type.Name)
                {
                default:
                    {
                    WriteType(writer, type);
                    }
                    break;
                }
            }

        private void WriteProcedure(TextWriter writer, ITypeLibraryMethodDescriptor source, String prefix) {
            writer.Write("{0}procedure {2}{1}", IndentString, source.Name, prefix);
            if (source.Parameters.Count > 0) {
                writer.Write("(");
                var i = 0;
                foreach (var parameter in source.Parameters) {
                    if (i > 0) {
                        writer.Write("; ");
                        }
                    WriteParameter(writer, parameter);
                    i++;
                    }
                writer.Write(")");
                }
            writer.WriteLine("; safecall;");
            }

        private void WriteFunction(TextWriter writer, ITypeLibraryMethodDescriptor source, String prefix, ITypeLibraryTypeDescriptor rettype, IEnumerable<ITypeLibraryParameterDescriptor> parameters) {
            writer.Write("{2}function  {0}{1}", prefix, ToUpperFirstLetter(source.Name), IndentString);
            var @params = parameters.ToArray();
            if (@params.Length > 0) {
                writer.Write("(");
                var i = 0;
                foreach (var parameter in @params) {
                    if (i > 0) {
                        writer.Write("; ");
                        }
                    WriteParameter(writer, parameter);
                    i++;
                    }
                writer.Write(")");
                }
            writer.Write(":");
            WriteType(writer, rettype);
            writer.WriteLine(";safecall;");
            }

        private void WriteMethod(TextWriter writer, ITypeLibraryMethodDescriptor source, String prefix) {
            var parameters = source.Parameters;
            var c = parameters.Count;
            if (source.ReturnType.Name == "HRESULT") {
                if (c == 0) { WriteProcedure(writer, source, prefix); }
                else
                    {
                    var lastparam = parameters[c - 1];
                    if (lastparam.IsRetval && lastparam.ParameterType.IsPointer) {
                        var typeref = lastparam.ParameterType.UnderlyingType;
                        WriteFunction(writer, source, prefix, typeref, parameters.Take(c - 1));
                        }
                    else
                        {
                        WriteProcedure(writer, source, prefix);    
                        }
                    }
                }
            else
                {
                writer.WriteLine("{0}yyyy", IndentString);
                }
            }

        private void WriteMethod(TextWriter writer, ITypeLibraryPropertyDescriptor source) {
            if (source.CanRead)
                {
                WriteMethod(writer, source.GetMethod, "Get_");
                }
            if (source.CanWrite)
                {
                WriteMethod(writer, source.SetMethod, "Set_");
                }
            }

        private String ToUpperFirstLetter(String source)
            {
            return $"{Char.ToUpper(source[0])}{source.Substring(1)}";
            }

        private void WriteProperty(TextWriter writer, ITypeLibraryPropertyDescriptor source) {
            var name = ToUpperFirstLetter(source.Name);
            writer.Write("{0}property {1}", IndentString, name);
            if (source.Parameters.Count > 0) {
                writer.Write("[");
                for (var i = 0; i < source.Parameters.Count; i++) {
                    if (i > 0) { writer.Write("; "); }
                    WriteParameter(writer, source.Parameters[i]);
                    }
                writer.Write("]");
                }
            writer.Write(":");
            WriteType(writer, source.PropertyType);
            if (source.CanRead)
                {
                writer.Write(" read Get_{0}", name);
                }
            if (source.CanWrite)
                {
                writer.Write(" write Set_{0}", name);
                }
            writer.WriteLine(";");
            }

        private class RecordTypeComparer : IComparer<ITypeLibraryTypeDescriptor>
            {
            /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
            ///  Value
            ///  Meaning
            ///  Less than zero
            /// <paramref name="x" /> is less than <paramref name="y" />.
            ///  Zero
            /// <paramref name="x" /> equals <paramref name="y" />.
            ///  Greater than zero
            /// <paramref name="x" /> is greater than <paramref name="y" />.</returns>
            public Int32 Compare(ITypeLibraryTypeDescriptor x, ITypeLibraryTypeDescriptor y)
                {
                if (ReferenceEquals(x, y)) { return 0; }
                if (x == null) { return -1; }
                if (y == null) { return +1; }
                if (y.DeclaredFields.FirstOrDefault(i => i.FieldType.Name == x.Name) != null) {
                    Debug.Print($"{x.Name}-{y.Name}:+1");
                    return -1;
                    }
                if (x.DeclaredFields.FirstOrDefault(i => i.FieldType.Name == y.Name) != null) {
                    Debug.Print($"{x.Name}-{y.Name}:-1");
                    return +1;
                    }
                return x.Name.CompareTo(y.Name);
                }
            }

        private IEnumerable<ITypeLibraryTypeDescriptor> Order(IEnumerable<ITypeLibraryTypeDescriptor> types) {
            return types.OrderBy(i => i, new RecordTypeComparer());
            //var values = new LinkedList<ITypeLibraryTypeDescriptor>(types);
            //var i = values.First;
            //for (; i != null;) {
            //    var fields = i.Value.DeclaredFields.Where(field => values.FirstOrDefault(j => j.Name == field.FieldType.Name) != null).ToArray();
            //    if (fields.Length == 0) { i = i.Next; }
            //    else
            //        {
            //        foreach (var field in fields) {
            //            var j = values                        
            //            }
            //        }
            //    }
            //return values;
            }
        }
    }
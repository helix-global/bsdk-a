using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace XSDSC
    {
    internal class Program
        {
        [MTAThread]
        internal static void Main(String[] args) {
            try
                {
                var inputfilename = String.Empty;
                foreach (var arg in args) {
                    if (arg.StartsWith("input:")) { inputfilename = arg.Substring(6); }
                    }
                if (String.IsNullOrWhiteSpace(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
                if (!File.Exists(inputfilename)) { throw new FileNotFoundException("File not found.", inputfilename); }
                switch (Path.GetExtension(inputfilename).ToLower()) {
                    case ".xsd":
                        {
                        var schema = XmlSchema.Read(new XmlTextReader(inputfilename), delegate(Object sender, ValidationEventArgs e) {
                            if ((sender != null) && (e != null)) {

                                }
                            });
                        if (schema != null) {
                            if (!schema.IsCompiled) {
                                #pragma warning disable CS0618
                                schema.Compile(delegate(Object sender, ValidationEventArgs e) {
                                    if ((sender != null) && (e != null)) {
                                        }
                                    });
                                #pragma warning restore CS0618
                                }
                            Process(schema);
                            }
                        }
                        break;
                    default:
                        {
                        }
                    break;
                    }
                }
            catch (Exception e)
                {
                Console.WriteLine(e);
                }
            }

        private static String GetDesiredName(String value) {
            var values = value.Split(new []{' ','-'}, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(String.Empty, values.Select(i =>{
                var c = i.Length;
                return (c == 1)
                    ? i.ToUpper()
                    : i.Substring(0,1).ToUpper() + i.Substring(1).ToLower();
                }));
            }

        private static void Process(XmlSchemaAttributeGroup source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var DesiredName = GetDesiredName(source.Name);
            using (var writer = new StreamWriter(File.OpenWrite($"I{DesiredName}.cs"))) {
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("namespace BinaryStudio.ReportingServices.FormattingObjects");
                writer.WriteLine($"    {{");
                writer.WriteLine($"    public interface I{DesiredName}");
                writer.WriteLine($"        {{");
                var properties = new SortedDictionary<String,Type>();
                foreach (var attribute in source.Attributes.OfType<XmlSchemaAttribute>()) {
                    var type = ((attribute.AttributeSchemaType.BaseXmlSchemaType != null)
                        ? attribute.AttributeSchemaType.BaseXmlSchemaType
                        : attribute.AttributeSchemaType).Datatype.ValueType;
                    properties[GetDesiredName(attribute.Name)] = type;
                    }
                foreach (var attribute in source.Attributes.OfType<XmlSchemaAttributeGroup>().SelectMany(i => i.Attributes.OfType<XmlSchemaAttribute>())) {
                    var type = ((attribute.AttributeSchemaType.BaseXmlSchemaType != null)
                        ? attribute.AttributeSchemaType.BaseXmlSchemaType
                        : attribute.AttributeSchemaType).Datatype.ValueType;
                    properties[GetDesiredName(attribute.Name)] = type;
                    }
                foreach (var pi in properties) {
                    writer.Write("        ");
                    writer.Write($"{pi.Value.Name} ");
                    writer.WriteLine($"{pi.Key} {{get;set;}}");
                    }
                writer.WriteLine($"        }}");
                writer.WriteLine($"    }}");
                }
            }

        private class PropertyInfo
            {
            public String Type;
            public Boolean CanRead;
            public Boolean CanWrite;
            public String DefaultValue;
            public Boolean IsCollection;
            public Boolean IsComplexType;
            public Boolean IsOptional;
            public IList<String> Attributes = new List<String>();
            }

        private static void Fill(IDictionary<String,PropertyInfo> target,IEnumerable<XmlSchemaAttribute> attributes) {
            foreach (var attribute in attributes) {
                PropertyInfo pi;
                var type = ((attribute.AttributeSchemaType.BaseXmlSchemaType != null)
                    ? attribute.AttributeSchemaType.BaseXmlSchemaType
                    : attribute.AttributeSchemaType).Datatype.ValueType;
                target[GetDesiredName(attribute.Name)] = pi = new PropertyInfo{
                    Type = type.Name,
                    CanRead = true,
                    CanWrite = true,
                    DefaultValue = attribute.DefaultValue,
                    IsOptional = attribute.Use != XmlSchemaUse.Required
                    };
                pi.Attributes.Add($@"XmlAttribute(""{attribute.Name}"")");
                if (!String.IsNullOrWhiteSpace(attribute.DefaultValue)) { pi.Attributes.Add($@"DefaultValue(""{attribute.DefaultValue}"")"); }
                }
            }

        private static void Process(
            IDictionary<String,XmlSchemaAttributeGroup> groupA,
            IDictionary<String,XmlSchemaComplexType> complexTypes,
            XmlSchemaComplexType source, String name = null,
            XmlQualifiedName QualifiedName = null) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            name = name ?? source.Name;
            var IsAbstract = source.IsAbstract;
            var DesiredName = GetDesiredName(name);
            var BaseType = (source.BaseSchemaType == null)
                ? String.Empty
                : GetDesiredName(source.BaseXmlSchemaType.Name);
            QualifiedName = QualifiedName ?? source.QualifiedName;
            var Extension = (source.DerivedBy == XmlSchemaDerivationMethod.Extension)
                ? (XmlSchemaComplexContentExtension)source.ContentModel.Content
                : null;
            var ImplementedInterfaces = new HashSet<String>(((Extension != null)
                ? Extension.Attributes
                : source.Attributes).OfType<XmlSchemaAttributeGroupRef>().Select(i=> $"I{GetDesiredName(i.RefName.Name)}"));
            using (var writer = new StreamWriter(File.OpenWrite($"{DesiredName}.cs"))) {
                var DerivedTypes = new HashSet<String>(complexTypes.Where(i => (i.Value.DerivedBy == XmlSchemaDerivationMethod.Extension) && (i.Value.BaseXmlSchemaType.Name == source.Name)).Select(i => GetDesiredName(i.Key)));
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("using System.ComponentModel;");
                writer.WriteLine("using System.IO;");
                writer.WriteLine("using System.Xml.Serialization;");
                writer.WriteLine();
                writer.WriteLine("namespace BinaryStudio.ReportingServices.FormattingObjects");
                writer.WriteLine($"    {{");
                if (DerivedTypes.Any()) {
                    foreach (var DerivedType in DerivedTypes) {
                        writer.WriteLine($@"    [XmlInclude(typeof({DerivedType}))]");
                        }
                    }
                writer.WriteLine($@"    [Serializable]");
                writer.WriteLine($@"    [XmlType(Namespace = ""{QualifiedName.Namespace}"")]");
                writer.WriteLine($@"    [XmlRoot(Namespace = ""{QualifiedName.Namespace}"",ElementName=""{name}"")]");
                writer.Write(@"    public {1}class {0}", DesiredName, IsAbstract ? "abstract " : "");
                if ((ImplementedInterfaces.Count > 0) || (!String.IsNullOrWhiteSpace(BaseType))) {
                    var types = new List<String>();
                    if (!String.IsNullOrWhiteSpace(BaseType)) { types.Add(BaseType); }
                    if (ImplementedInterfaces.Count > 0) { types.AddRange(ImplementedInterfaces); }
                    writer.Write(" : {0}", String.Join(",", types));
                    }
                writer.WriteLine();
                writer.WriteLine($"        {{");
                
                var properties = new SortedDictionary<String,PropertyInfo>();
                if (source.DerivedBy == XmlSchemaDerivationMethod.Extension) {
                    Fill(properties, Extension.Attributes.OfType<XmlSchemaAttribute>());
                    Fill(properties, Extension.Attributes.OfType<XmlSchemaAttributeGroup>().SelectMany(i => i.Attributes.OfType<XmlSchemaAttribute>()));
                    Fill(properties, Extension.Attributes.OfType<XmlSchemaAttributeGroupRef>().SelectMany(i => groupA[i.RefName.Name].Attributes.OfType<XmlSchemaAttribute>()));
                    }
                else
                    {
                    Fill(properties, source.Attributes.OfType<XmlSchemaAttribute>());
                    Fill(properties, source.Attributes.OfType<XmlSchemaAttributeGroup>().SelectMany(i => i.Attributes.OfType<XmlSchemaAttribute>()));
                    Fill(properties, source.Attributes.OfType<XmlSchemaAttributeGroupRef>().SelectMany(i => groupA[i.RefName.Name].Attributes.OfType<XmlSchemaAttribute>()));
                    }

                if ((Extension?.Particle ?? source.Particle) is XmlSchemaSequence sequence) {
                    foreach (var sequenceE in sequence.Items.OfType<XmlSchemaElement>()) {
                        var pi = new PropertyInfo();
                        if (sequenceE.ElementSchemaType is XmlSchemaComplexType complexT) {
                            if (sequenceE.MaxOccursString == "unbounded") {
                                properties[GetDesiredName(sequenceE.Name)] = pi = new PropertyInfo{
                                    Type=GetDesiredName(complexT.Name),
                                    IsCollection = true,
                                    CanWrite = true,
                                    CanRead = true
                                    };
                                }
                            else
                                {
                                properties[GetDesiredName(sequenceE.Name)] = pi = new PropertyInfo{
                                    Type=$"{GetDesiredName(complexT.Name)}",
                                    CanWrite = true,
                                    CanRead = true,
                                    IsComplexType = true,
                                    IsOptional = sequenceE.MinOccurs==0
                                    };
                                }
                            }
                        pi.Attributes.Add($@"XmlElement(""{sequenceE.Name}"")");
                        }
                    }
                if ((Extension?.Particle ?? source.Particle) is XmlSchemaChoice Choice) {
                    foreach (var ChoiceE in Choice.Items.OfType<XmlSchemaElement>()) {
                        var pi = new PropertyInfo();
                        if (ChoiceE.ElementSchemaType is XmlSchemaComplexType complexT) {
                            if (ChoiceE.MaxOccursString == "unbounded") {
                                properties[GetDesiredName(ChoiceE.Name)] = pi = new PropertyInfo{
                                    Type=GetDesiredName(complexT.Name),
                                    IsCollection = true,
                                    CanWrite = true,
                                    CanRead = true
                                    };
                                }
                            else
                                {
                                properties[GetDesiredName(ChoiceE.Name)] = pi = new PropertyInfo{
                                    Type=$"{GetDesiredName(complexT.Name)}",
                                    CanWrite = true,
                                    CanRead = true,
                                    IsComplexType = true,
                                    IsOptional = ChoiceE.MinOccurs==0
                                    };
                                }
                            }
                        pi.Attributes.Add($@"XmlElement(""{ChoiceE.Name}"")");
                        }
                    }
                foreach (var pi in properties) {
                    writer.WriteLine("        {3} public {0} {1} {{ {2} }}",
                        pi.Value.IsCollection
                            ? $"List<{pi.Value.Type}>"
                            : pi.Value.Type,
                        pi.Key, (pi.Value.CanRead && pi.Value.CanWrite)
                            ? "get;set;"
                            : pi.Value.CanRead
                                ? "get;"
                                : "set;",
                        String.Join(String.Empty, pi.Value.Attributes.Select(i => $"[{i}]"))
                        );
                    }
                writer.WriteLine(@"        {1} {0}()", DesiredName, IsAbstract ? "protected" : "public");
                writer.WriteLine($"            {{");
                foreach (var pi in properties) {
                         if (pi.Value.IsCollection)  { writer.WriteLine("            {0} = new List<{1}>();", pi.Key, pi.Value.Type); }
                    else if ( pi.Value.IsComplexType && !pi.Value.IsOptional) { writer.WriteLine("            {0} = new {1}();", pi.Key, pi.Value.Type); }
                    else if (!pi.Value.IsComplexType && !pi.Value.IsOptional) { writer.WriteLine("            {0} = Guid.NewGuid().ToString();", pi.Key); }
                    else if (!String.IsNullOrWhiteSpace(pi.Value.DefaultValue)) { writer.WriteLine(@"            {0} = ""{1}"";",pi.Key,pi.Value.DefaultValue); }
                    }
                writer.WriteLine($"            }}");
                writer.WriteLine();
                if (!IsAbstract) {
                    writer.WriteLine($"        #region P:Serializer:XmlSerializer");
                    writer.WriteLine($"        private static XmlSerializer serializer;");
                    writer.WriteLine($"        public static XmlSerializer Serializer {{ get {{");
                    writer.WriteLine($"            serializer = serializer ?? new XmlSerializer(typeof({DesiredName}));");
                    writer.WriteLine($"            return serializer;");
                    writer.WriteLine($"            }}}}");
                    writer.WriteLine($"        #endregion");
                    writer.WriteLine();
                    writer.WriteLine($"        public override String Serialize()");
                    writer.WriteLine($"            {{");
                    writer.WriteLine($"            using (var output = new MemoryStream()) {{");
                    writer.WriteLine($"                Serializer.Serialize(output, this);");
                    writer.WriteLine($"                output.Seek(0, SeekOrigin.Begin);");
                    writer.WriteLine($"                using (var reader = new StreamReader(output)) {{");
                    writer.WriteLine($"                    return reader.ReadToEnd();");
                    writer.WriteLine($"                    }}");
                    writer.WriteLine($"                }}");
                    writer.WriteLine($"            }}");
                    }
                else
                    {
                    writer.WriteLine($"        public abstract String Serialize();");
                    }
                writer.WriteLine($"        }}");
                writer.WriteLine($"    }}");
                }
            }

        private static void Process(IDictionary<String,XmlSchemaAttributeGroup> groupA, IDictionary<String,XmlSchemaComplexType> complexTypes,XmlSchemaElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Process(groupA, complexTypes, source.ElementSchemaType as XmlSchemaComplexType, source.Name, source.QualifiedName);
            }

        private static void Process(XmlSchema source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var groupsA = source.Items.OfType<XmlSchemaAttributeGroup>().ToDictionary(i => i.Name, i => i);
            var groupsC = source.Items.OfType<XmlSchemaComplexType>().ToDictionary(i => i.Name, i => i);
            foreach (var item in source.Items.OfType<XmlSchemaComplexType>()) {
                Process(groupsA, groupsC, item);
                }
            foreach (var item in source.Items.OfType<XmlSchemaAttributeGroup>()) {
                Process(item);
                }
            foreach (var item in source.Items.OfType<XmlSchemaElement>()) {
                Process(groupsA, groupsC, item);
                }
            }
        }
    }

#if BBB
using System;
using System.Linq;
using Newtonsoft.Json;

namespace RationalRose.Serialization
    {
    public class ModelJsonConverter : JsonConverter
        {
        private void Write(JsonWriter writer, String name, Boolean value, Boolean defaultvalue) {
            if (value != defaultvalue) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                }
            }
        private void Write(JsonWriter writer, String name, String value, String defaultvalue) {
            if (value != defaultvalue) {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
                }
            }
        private void Write(JsonWriter writer, String name, Object value)
            {
            writer.WritePropertyName(name);
            if (value is IRoseRichType)
                {
                Write(writer, (IRoseRichType)value, true);                
                }
            else if (value is IRoseRole)
                {
                Write(writer, (IRoseRole)value, true);
                }
            else
                {
                writer.WriteValue(value);    
                }
            }

        private void Write(JsonWriter writer, IREICOMObject source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            //Write(writer, "IdentifyClass", source.IdentifyClass());
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IREICOMElement source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IREICOMObject)source, false);
            Write(writer, "Name", source.Name);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IREICOMItem source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IREICOMElement)source, false);
            Write(writer, "Stereotype", source.Stereotype, String.Empty);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseRelation source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IREICOMItem)source, false);
            Write(writer, "SupplierName", source.SupplierName);
            Write(writer, "HasClient", source.HasClient());
            Write(writer, "HasSupplier", source.HasSupplier());
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseRole source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IRoseRelation)source, false);
            Write(writer, "IsAggregate", source.Aggregate);
            Write(writer, "IsStatic", source.Static);
            Write(writer, "IsNavigable", source.Navigable);
            Write(writer, "Cardinality", source.Cardinality);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseControllableUnit source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IREICOMItem)source, false);
            Write(writer, "IsControlled", source.IsControlled(), false);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRosePackage source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IRoseControllableUnit)source, false);
            Write(writer, "IsRootPackage", source.IsRootPackage(), false);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseRichType source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, "Value", source.Value);
            Write(writer, "Name", source.Name);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseAttribute source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            //Write(writer, (IREICOMItem)source, false);
            //Write(writer, "Type", source.Type);
            Write(writer, "Attribute", $"{source.Type} {source.Name}" + " { get;set; }");
            //Write(writer, "Containment", source.Containment);
            //Write(writer, "ExportControl", source.ExportControl);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseAssociation source, Boolean decorate, String classname)
            {
            if (decorate) { writer.WriteStartObject(); }
            //Write(writer, (IREICOMItem)source, false);
            var role1 = source.Role1;
            var role2 = source.Role2;
            //Write(writer, "Role1", source.Role1);
            //Write(writer, "Role2", source.Role2);
            if (role1.SupplierName == classname)
                {
                if (!String.IsNullOrWhiteSpace(role2.Name))
                    {
                    Write(writer, "Association", $"{role2.SupplierName}[{role2.Cardinality}] {role2.Name}" +  " { get; }");
                    }
                }
            else
                {
                if (!String.IsNullOrWhiteSpace(role1.Name))
                    {
                    Write(writer, "Association", $"{role1.SupplierName}[{role1.Cardinality}] {role1.Name}" +  " { get; }");
                    }
                }
            //Write(writer, "Containment", source.Containment);
            //Write(writer, "ExportControl", source.ExportControl);
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseClass source, Boolean decorate)
            {
            if (decorate) { writer.WriteStartObject(); }
            Write(writer, (IREICOMItem)source, false);
            //Write(writer, "IsAbstract", source.Abstract, false);
            var basetypes = source.GetSuperclasses().ToArray();
            if (basetypes.Length > 0) {
                writer.WritePropertyName("BaseTypes");
                writer.WriteStartArray();
                foreach (var i in basetypes) { writer.WriteValue(i.Name); }
                writer.WriteEndArray();
                }
            var attributes = source.Attributes.ToArray();
            if (attributes.Length > 0) {
                writer.WritePropertyName("Attributes");
                writer.WriteStartArray();
                foreach (var i in attributes)
                    {
                    Write(writer, i, true);
                    }
                writer.WriteEndArray();
                }

            var name = source.Name;
            var associations = source.GetAssociations().ToArray();
            if (associations.Length > 0) {
                writer.WritePropertyName("Associations");
                writer.WriteStartArray();
                foreach (var i in associations)
                    {
                    Write(writer, i, true, name);
                    }
                writer.WriteEndArray();
                }
            if (decorate) { writer.WriteEndObject(); }
            }

        private void Write(JsonWriter writer, IRoseCategory source)
            {
            writer.WriteStartObject();
            Write(writer, (IRosePackage)source, false);
            var categories = source.Categories.ToArray();
            if (categories.Length > 0) {
                writer.WritePropertyName("Categories");
                writer.WriteStartArray();
                foreach (var category in categories) {
                    Write(writer, category);
                    }
                writer.WriteEndArray();
                }
            var classes = source.Classes.ToArray();
            if (classes.Length > 0) {
                writer.WritePropertyName("Classes");
                writer.WriteStartArray();
                foreach (var i in classes) {
                    Write(writer, i, true);
                    }
                writer.WriteEndArray();
                }
            writer.WriteEndObject();
            }

        private void Write(JsonWriter writer, IRoseModel source)
            {
            writer.WriteStartObject();
            writer.WritePropertyName("Packages");
            writer.WriteStartArray();
            if (source.RootUseCaseCategory != null) { Write(writer, source.RootUseCaseCategory); }
            if (source.RootCategory != null)        { Write(writer, source.RootCategory);        }
            writer.WriteEndArray();
            writer.WriteEndObject();
            }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (value != null) {
                if (value is IRoseModel) { Write(writer, (IRoseModel)value); }
                }
            }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override Boolean CanConvert(Type objectType)
            {
            return true;
            }
        }
    }
#endif
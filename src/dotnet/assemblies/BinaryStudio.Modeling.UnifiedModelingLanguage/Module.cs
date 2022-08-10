using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using BinaryStudio.Modeling.UnifiedModelingLanguage.Internal;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public class Module
        {
        #region M:LoadModel(XmlDocument):Model
        public Model LoadModel(XmlDocument source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.DocumentElement == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
            var nsmgr = new XmlNamespaceManager(source.NameTable);
            var xmlns = new SortedDictionary<String, String>();
            foreach (XmlAttribute attribute in source.DocumentElement.Attributes) {
                if (attribute.Prefix == "xmlns") {
                    xmlns.Add(attribute.LocalName, attribute.Value);
                    nsmgr.AddNamespace(attribute.LocalName, attribute.Value);
                    }
                }
            if (source.DocumentElement.LocalName == "XMI") {
                var XMIprefix = source.DocumentElement.Prefix;
                foreach (var e in source.DocumentElement.ChildNodes.OfType<XmlElement>()) {
                    var id = e.GetAttribute("id", xmlns[XMIprefix]);
                    switch (e.LocalName) {
                        case "Model":
                            {
                            var UMLprefix = e.Prefix;
                            var r = new EModel(id);
                            UpdateTarget(xmlns, XMIprefix, UMLprefix, r, e);
                            }
                            break;
                        }
                    }
                }
            return null;
            }
        #endregion

        private static EElement CreateElement(String id, String type) {
            switch (type)
                {
                case "Package" : return new EPackage(id);
                case "Class"   : return new EClass(id);
                case "Abstraction" : return new EAbstraction(id);
                case "Component" : return new EComponent(id);
                case "PrimitiveType": return new EPrimitiveType(id);
                default: throw new ArgumentOutOfRangeException(nameof(type));
                }
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Package target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (NamedElement)target, source);
            foreach (var e in source.ChildNodes.OfType<XmlElement>()) {
                var id = e.GetAttribute("id", xmlns[XMIprefix]);
                switch (e.LocalName) {
                    case "packagedElement":
                        {
                        var type = e.GetAttribute("type", xmlns[XMIprefix]);
                        if (type.StartsWith($"{UMLprefix}:")) {
                            type = type.Substring(UMLprefix.Length + 1);
                            var E = CreateElement(id, type);
                            switch (type)
                                {
                                case "Package":
                                    target.OwnedElement.Add((Package)E);
                                    UpdateTarget(xmlns, XMIprefix, UMLprefix, (Package)E, e);
                                    break;
                                case "Class":
                                    target.OwnedElement.Add((Type)E);
                                    UpdateTarget(xmlns, XMIprefix, UMLprefix, (Class)E, e);
                                    break;
                                case "Component":
                                    target.OwnedElement.Add((Component)E);
                                    UpdateTarget(xmlns, XMIprefix, UMLprefix, (Component)E, e);
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Class target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (StructuredClassifier)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, StructuredClassifier target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (Classifier)target, source);
            foreach (var e in source.ChildNodes.OfType<XmlElement>()) {
                var id = e.GetAttribute("id", xmlns[XMIprefix]);
                switch (e.LocalName) {
                    case "ownedAttribute":
                        {
                        var E = new EProperty(id);
                        target.OwnedElement.Add(E);
                        UpdateTarget(xmlns, XMIprefix, UMLprefix, E, e);
                        }
                        break;
                    }
                }
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, StructuralFeature target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (NamedElement)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Property target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (StructuralFeature)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Classifier target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (Type)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Type target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (NamedElement)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Abstraction target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (Dependency)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Dependency target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (NamedElement)target, source);
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, NamedElement target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            target.Name = source.GetAttribute("name");
            switch (source.GetAttribute("visibility").ToLowerInvariant()) {
                case "public"    : target.Visibility = VisibilityKind.Public;    break;
                case "private"   : target.Visibility = VisibilityKind.Private;   break;
                case "protected" : target.Visibility = VisibilityKind.Protected; break;
                case "package"   : target.Visibility = VisibilityKind.Package;   break;
                }
            }

        private static void UpdateTarget(IDictionary<String,String> xmlns, String XMIprefix, String UMLprefix, Model target, XmlElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UpdateTarget(xmlns, XMIprefix, UMLprefix, (Package)target, source);
            }

        //#region M:LoadModel(XmlReader):Model
        //public Model LoadModel(XmlReader reader)
        //    {
        //    if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
        //    var model = new EModel();
        //    while (reader.Read())
        //        {
        //        switch (reader.NodeType)
        //            {
        //            case XmlNodeType.Attribute:
        //                break;
        //            case XmlNodeType.None:
        //                break;
        //            case XmlNodeType.Element:
        //                    {

        //                    }
        //                break;
        //            case XmlNodeType.Text:
        //                break;
        //            case XmlNodeType.CDATA:
        //                break;
        //            case XmlNodeType.EntityReference:
        //                break;
        //            case XmlNodeType.Entity:
        //                break;
        //            case XmlNodeType.ProcessingInstruction:
        //                break;
        //            case XmlNodeType.Comment:
        //                break;
        //            case XmlNodeType.Document:
        //                break;
        //            case XmlNodeType.DocumentType:
        //                break;
        //            case XmlNodeType.DocumentFragment:
        //                break;
        //            case XmlNodeType.Notation:
        //                break;
        //            case XmlNodeType.Whitespace:
        //                break;
        //            case XmlNodeType.SignificantWhitespace:
        //                break;
        //            case XmlNodeType.EndElement:
        //                break;
        //            case XmlNodeType.EndEntity:
        //                break;
        //            case XmlNodeType.XmlDeclaration:
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException();
        //            }
        //        }
        //    return model;
        //    }
        //#endregion
        #region M:LoadModel(String):Model
        public Model LoadModel(String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            switch (Path.GetExtension(filename).ToLowerInvariant()) {
                case ".emx":
                    {
                    
                    }
                    break;
                }
            throw new ArgumentOutOfRangeException(nameof(filename));
            }
        #endregion
        }
    }
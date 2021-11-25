using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using LIBFLAGS = System.Runtime.InteropServices.ComTypes.LIBFLAGS;
using SYSKIND = System.Runtime.InteropServices.ComTypes.SYSKIND;

namespace BinaryStudio.PortableExecutable
    {
    [JsonObject(MemberSerialization.OptIn)]
    //[JsonConverter(typeof(Converter))]
    public abstract class TypeLibraryDescriptor : MetadataObject, ITypeLibraryDescriptor, IServiceProvider 
        {
        public abstract Version Version { get; }
        public abstract CultureInfo Culture { get; }
        public abstract String Name { get; }
        public abstract String HelpFile { get; }
        public virtual String HelpString { get { return null; }}
        public abstract Guid UniqueIdentifier { get; }
        public abstract Int32 HelpContext { get; }
        public abstract IList<ITypeLibraryTypeDescriptor> DefinedTypes { get; }
        public virtual IList<TypeLibraryCustomAttribute> CustomAttributes { get { return EmptyList<TypeLibraryCustomAttribute>.Value; }}
        public virtual IList<ITypeLibraryDescriptor> ImportedLibraries { get { return EmptyList<ITypeLibraryDescriptor>.Value; }}
        public abstract LIBFLAGS Flags { get; }
        public abstract SYSKIND TargetOperatingSystemPlatform { get; }
        public Encoding Encoding { get; }
        public TypeLibraryIdentifier Identifier { get { return new TypeLibraryIdentifier(UniqueIdentifier, Version, TargetOperatingSystemPlatform); }}

        //private class Converter : TypeLibraryJsonConverter
        //    {
        //    public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        //        {
        //        throw new NotImplementedException();
        //        }

        //    public override Boolean CanConvert(Type objectType)
        //        {
        //        throw new NotImplementedException();
        //        }
            //}

        protected TypeLibraryDescriptor(MetadataScope scope, Encoding encoding)
            :base(scope)
            {
            Encoding = encoding;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            if (Name != null) {
                var r = new StringBuilder();
                r.Append(Name);
                if (Version != null) {
                    r.Append(", Version=");
                    r.Append(Version);
                    }
                if (Culture != null) {
                    r.Append(", Culture=");
                    r.Append(Culture);
                    }
                r.Append(", UniqueIdentifier=");
                r.Append(UniqueIdentifier);
                r.AppendFormat(", {0}", TargetOperatingSystemPlatform);
                return r.ToString();
                }
            return "[library]";
            }

        //#region M:IXmlSerializable.GetSchema:XmlSchema
        ///**
        // * <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="XmlSchemaProviderAttribute" /> to the class.</summary>
        // * <returns>An <see cref="XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        // * */
        //XmlSchema IXmlSerializable.GetSchema()
        //    {
        //    throw new NotImplementedException();
        //    }
        //#endregion
        //#region M:IXmlSerializable.ReadXml(XmlReader)
        ///**
        // * <summary>Generates an object from its XML representation.</summary>
        // * <param name="reader">The <see cref="XmlReader" /> stream from which the object is deserialized.</param>
        // * */
        //void IXmlSerializable.ReadXml(XmlReader reader)
        //    {
        //    throw new NotImplementedException();
        //    }
        //#endregion
        //#region M:IXmlSerializable.WriteXml(XmlWriter)
        ///**
        // * <summary>Converts an object into its XML representation.</summary>
        // * <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized.</param>
        // * */
        //void IXmlSerializable.WriteXml(XmlWriter writer)
        //    {
        //    WriteXml(writer);
        //    }
        //#endregion
        #region M:IServiceProvider.GetService(Type):Object
        /**
         * <summary>Gets the service object of the specified type.</summary>
         * <param name="service">An object that specifies the type of service object to get. </param>
         * <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        Object IServiceProvider.GetService(Type service) {
            return GetService(service);
            }
        #endregion

        private static void WriteAttribute(XmlWriter writer, String key, Object value) {
            if (value != null) {
                writer.WriteStartElement(key);
                writer.WriteString(value.ToString());
                writer.WriteEndElement();
                }
            }

        private static void WriteAttribute(XmlWriter writer, String key, String value) {
            if (!String.IsNullOrWhiteSpace(value)) {
                writer.WriteStartElement(key);
                writer.WriteString(value);
                writer.WriteEndElement();
                }
            }

        /**
         * <summary>Converts an object into its XML representation.</summary>
         * <param name="writer">The <see cref="XmlWriter" /> stream to which the object is serialized.</param>
         * */
        protected virtual void WriteXml(XmlWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (Name    != null) { writer.WriteAttributeString("Name", Name); }
            if (Version != null) { writer.WriteAttributeString("Version", Version.ToString()); }
            if (Culture != null) { writer.WriteAttributeString("Culture", Culture.ToString()); }
            writer.WriteAttributeString("UniqueIdentifier", UniqueIdentifier.ToString());
            WriteAttribute(writer, nameof(HelpFile),    HelpFile);
            WriteAttribute(writer, nameof(HelpString),  HelpString);
            WriteAttribute(writer, nameof(HelpContext), HelpContext);
            WriteAttribute(writer, nameof(TargetOperatingSystemPlatform), TargetOperatingSystemPlatform);
            WriteAttribute(writer, nameof(Flags), Flags);
            if (DefinedTypes.Count > 0) {
                writer.WriteStartElement(nameof(DefinedTypes));
                foreach (var item in DefinedTypes.OfType<IXmlSerializable>()) {
                    item.WriteXml(writer);
                    }
                writer.WriteEndElement();
                }
            }

        /**
         * <summary>Gets the service object of the specified type.</summary>
         * <param name="service">An object that specifies the type of service object to get. </param>
         * <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Object GetService(Type service)
            {
            if (service == GetType()) { return this; }
            if (service == typeof(ITypeLibraryDescriptor)) { return this; }
            if (service == typeof(IXmlSerializable)) { return this; }
            if (service == typeof(TypeLibraryDescriptor)) { return this; }
            throw new ArgumentOutOfRangeException(nameof(service));
            }

        //public static unsafe ITypeLibraryDescriptor LoadFrom(MetadataScope scope, String filename, Encoding encoding) {
        //    if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
        //    if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
        //    if (String.IsNullOrWhiteSpace(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
        //    if (!File.Exists(filename)) { throw new FileNotFoundException(); }
        //    using (var mapping = new FileMapping(filename)) {
        //        return LoadFrom(scope, mapping.Memory, mapping.Length, encoding);
        //        }
        //    }

        //public static unsafe ITypeLibraryDescriptor LoadFrom(MetadataScope scope, Byte[] source, Encoding encoding) {
        //    if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
        //    if (source == null) { throw new ArgumentNullException(nameof(source)); }
        //    fixed (Byte* r = source) {
        //        return LoadFrom(scope, r, source.LongLength, encoding);
        //        }
        //    }

        private const UInt16 IMAGE_DOS_SIGNATURE = 0x5A4D;
        private const UInt32 MSFT_SIGNATURE = 0x5446534D; /* "MSFT" */
        private const UInt32 SLTG_SIGNATURE = 0x47544c53; /* "SLTG" */

        //private static unsafe ITypeLibraryDescriptor LoadFrom(MetadataScope scope, Byte* source, Int64 size, Encoding encoding) {
        //    encoding = encoding ?? Encoding.GetEncoding(GetACP());
        //    if ((*(UInt16*)source) == IMAGE_DOS_SIGNATURE) { return (ITypeLibraryDescriptor)((IServiceProvider)PortableExecutableSource.LoadFrom(scope, source, size)).GetService(typeof(ITypeLibraryDescriptor)); }
        //    if ((*(UInt32*)source) == MSFT_SIGNATURE) { return new MSFTMetadataTypeLibrary(scope, source, size, encoding); }
        //    if ((*(UInt32*)source) == SLTG_SIGNATURE) { return new SLTGTypeLibrary(scope, source, size, encoding); }
        //    throw new NotSupportedException();
        //    }

        //protected unsafe TypeLibraryDescriptor(MetadataScope scope, Byte* source, Int64 size, Encoding encoding) {
        //    if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
        //    Scope = scope;
        //    }

        #region M:BeginInvoke(Action[])
        protected void BeginInvoke(params Action[] actions) {
            if (actions != null) {
                var r = new List<WaitHandle>();
                for (var i = 0; i < actions.Length; ++i) {
                    if (actions[i] != null) {
                        r.Add(actions[i].BeginInvoke(null, null).AsyncWaitHandle);
                        }
                    }
                WaitHandle.WaitAll(r.ToArray());
                }
            }
        #endregion

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetACP();
        }
    }
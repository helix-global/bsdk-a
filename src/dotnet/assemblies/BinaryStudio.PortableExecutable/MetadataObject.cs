using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable
    {
    public abstract class MetadataObject : IServiceProvider, IDisposable,
        IJsonSerializable,
        ITextOutput,
        IXmlSerializable
        {
        private MetadataObjectState state = MetadataObjectState.Pending;

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="type">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="type" />. -or- <see langword="null" /> if there is no service object of type <paramref name="type" />.</returns>
        public virtual Object GetService(Type type)
            {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (type == GetType()) { return this; }
            return null;
            }

        public virtual MetadataObjectState State { get { return state; }}
        public MetadataScope Scope { get; }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            state = MetadataObjectState.Disposed;
            if (disposing) {
                }
            }
        #endregion-
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~MetadataObject() {
            Dispose(false);
            }
        #endregion

        //#region M:LoadInternal([Out]Exception,FileMapping):Boolean
        //internal unsafe Boolean LoadInternal(out Exception e, FileMapping mapping) {
        //    if (mapping == null) { throw new ArgumentNullException(nameof(mapping)); }
        //    try
        //        {
        //        using (var memory = new FileMappingMemory(mapping)) {
        //            return LoadInternal(out e, (Byte*)(void*)memory, mapping.Size);
        //            }
        //        }
        //    catch (Exception x)
        //        {
        //        e = new Exception(x.Message, x);
        //        return false;
        //        }
        //    }
        //#endregion
        //#region M:LoadInternal([Out]Exception,Byte*,Int64):Boolean
        //internal unsafe Boolean LoadInternal(out Exception e, Byte* source, Int64 size) {
        //    e = null;
        //    if (source == null) { throw new ArgumentNullException(nameof(source)); }
        //    if (state != MetadataObjectState.Pending) { throw new InvalidOperationException(); }
        //    try
        //        {
        //        state = MetadataObjectState.Loading;
        //        if (!Load(source, size)) {
        //            state = MetadataObjectState.Failed;
        //            return false;
        //            }
        //        state = MetadataObjectState.Loaded;
        //        return true;
        //        }
        //    catch (Exception x)
        //        {
        //        state = MetadataObjectState.Failed;
        //        e = new Exception(x.Message, x);
        //        return false;
        //        }
        //    }
        //#endregion

        protected MetadataObject(MetadataScope scope)
            {
            if (scope == null) { throw new ArgumentNullException(nameof(scope)); }
            Scope = scope;
            }

        protected internal abstract unsafe Boolean Load(out Exception e, Byte* source, Int64 size);
        public virtual void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            throw new NotImplementedException();
            }

        void ITextOutput.WriteText(TextWriter writer)
            {
            WriteText(0, writer);
            }

        protected internal virtual void WriteText(Int32 offset, TextWriter writer)
            {
            }

        #region M:IXmlSerializable.GetSchema:XmlSchema
        /// <summary>This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.</summary>
        /// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        XmlSchema IXmlSerializable.GetSchema()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IXmlSerializable.ReadXml(XmlReader)
        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IXmlSerializable.WriteXml(XmlWriter)
        /**
         * <summary>Converts an object into its XML representation.</summary>
         * <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
         */
        void IXmlSerializable.WriteXml(XmlWriter writer) {
            WriteXml(writer);
            }

        protected internal virtual void WriteXml(XmlWriter writer)
            {
            throw new NotImplementedException();
            }
        #endregion
        }
    }
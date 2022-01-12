using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using BinaryStudio.DataProcessing;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {   
    [DebuggerDisplay(@"\{{" + nameof(ToString) + @"(),nq}\}")]
    [TypeConverter(typeof(ObjectTypeConverter))]
    [XmlRoot("Extension")]
    public class Asn1CertificateExtension : Asn1LinkObject, IX509CertificateExtension
        {
        [Asn1DisplayName(nameof(Asn1CertificateExtension) + "." + nameof(Identifier))] public Asn1ObjectIdentifier Identifier { get; }
        [Asn1DisplayName(nameof(Asn1CertificateExtension) + "." + nameof(IsCritical))][TypeConverter(typeof(X509BooleanConverter))] public Boolean IsCritical { get; }

        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsExplicitConstructed { get { return base.IsExplicitConstructed; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsImplicitConstructed { get { return base.IsImplicitConstructed; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Boolean IsIndefiniteLength { get { return base.IsIndefiniteLength; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int32 Count { get { return base.Count; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Size { get { return base.Size; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Length { get { return base.Length; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Int64 Offset { get { return base.Offset; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1ObjectClass Class { get { return base.Class; } }
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override ReadOnlyMappingStream Content { get { return base.Content; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public override Asn1Object UnderlyingObject { get { return base.UnderlyingObject; }}
        [Browsable(false)][DebuggerBrowsable(DebuggerBrowsableState.Never)] public Asn1OctetString Body { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectIdentifier IX509CertificateExtension.Identifier { get { return Identifier; }}

        protected internal Asn1CertificateExtension(Asn1Object source)
            : base(source)
            {
            var c = source.Count;
            var i = 1;
            Identifier = (Asn1ObjectIdentifier)source[0];
            if (c > i) {
                if ((source[i].Class == Asn1ObjectClass.Universal) && (((Asn1UniversalObject)source[i]).Type == Asn1ObjectType.Boolean)) {
                    IsCritical = (Asn1Boolean)source[i];
                    i++;
                    }
                }
            if (c > i) {
                Body = (Asn1OctetString)source[i];
                }
            }

        protected internal Asn1CertificateExtension(Asn1CertificateExtension source)
            : base(source)
            {
            Identifier = source.Identifier;
            IsCritical = source.IsCritical;
            Body = source.Body;
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override String ToString()
            {
            return Identifier.ToString();
            }

        private static readonly IDictionary<String, Type> types = new ConcurrentDictionary<String, Type>();
        private static readonly ReaderWriterLockSlim syncobject = new ReaderWriterLockSlim();

        public static Asn1CertificateExtension From(Asn1CertificateExtension source) {
            if (ReferenceEquals(source, null)) { throw new ArgumentNullException(nameof(source)); }
            EnsureFactory();
            using (ReadLock(syncobject)) {
                if (types.TryGetValue(source.Identifier.ToString(), out var type)) {
                    if (type.IsSubclassOf(typeof(Asn1CertificateExtension))) {
                        var r = (Asn1CertificateExtension)Activator.CreateInstance(type, source);
                        return r;
                        }
                    }
                }
            return source;
            }

        #region M:EnsureFactory
        private static void EnsureFactory() {
            using (UpgradeableReadLock(syncobject)) {
                if (types.Count == 0) {
                    using (WriteLock(syncobject)) {
                        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(i => i.IsSubclassOf(typeof(Asn1CertificateExtension)))) {
                            foreach (var attribute in type.GetCustomAttributes(typeof(Asn1SpecificObjectAttribute), false).OfType<Asn1SpecificObjectAttribute>())
                                {
                                types.Add(attribute.Key, type);
                                }
                            }
                        }
                    }
                }
            }
        #endregion

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            throw new NotImplementedException(Identifier.ToString());
            using (writer.ObjectScope(serializer)) {
                //writer.WriteIndent();
                writer.WriteComment($" {OID.ResourceManager.GetString(Identifier.ToString(), CultureInfo.InvariantCulture)} ");
                writer.WriteValue(serializer, nameof(Identifier), Identifier.ToString());
                writer.WriteValue(serializer, nameof(IsCritical), IsCritical);
                }
            }
        }
    }
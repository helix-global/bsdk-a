using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    [DefaultProperty(nameof(Identifier))]
    public class CmsContentSpecificObject : Asn1LinkObject
        {
        [TypeConverter(typeof(Asn1ObjectIdentifierTypeConverter))][Order(-1)]
        [DisplayName("{Identifier}")]
        public Asn1ObjectIdentifier Identifier { get; }
        protected internal CmsContentSpecificObject(Asn1Object source)
            : base(source)
            {
            Identifier = (Asn1ObjectIdentifier)source[0];
            }

        private static readonly IDictionary<String, Type> factories = new ConcurrentDictionary<String, Type>();
        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();

        public static CmsContentSpecificObject From(CmsContentSpecificObject source) {
            EnsureFactory();
            using (ReadLock(o)) {
                if (factories.TryGetValue(source.Identifier.ToString(), out var type)) {
                    if (((Type)type).IsSubclassOf(typeof(CmsContentSpecificObject))) {
                        var r = (CmsContentSpecificObject)Activator.CreateInstance(type,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic,
                            null,
                            new Object[] {source},
                            null
                            );
                        return r;
                        }
                    }
                }
            return source;
            }

        private static void EnsureFactory() {
            using (UpgradeableReadLock(o)) {
                if (factories.Count == 0) {
                    using (WriteLock(o)) {
                        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                            var attribute = (CmsContentSpecificObjectAttribute)type.GetCustomAttributes(typeof(CmsContentSpecificObjectAttribute), false).FirstOrDefault();
                            if (attribute != null) {
                                try
                                    {
                                    factories.Add(attribute.Key, type);
                                    }
                                catch (ArgumentException e)
                                    {
                                    if (factories.TryGetValue(attribute.Key, out var etype)) { 
                                        e.Data["ExistingType"] = etype.FullName;
                                        }
                                    throw;
                                    }
                                catch (Exception e)
                                    {
                                    throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
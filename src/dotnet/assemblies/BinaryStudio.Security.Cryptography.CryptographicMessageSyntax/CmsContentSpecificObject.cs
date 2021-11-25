using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    public class CmsContentSpecificObject : Asn1LinkObject
        {
        public Asn1ObjectIdentifier Identifier { get; }
        protected internal CmsContentSpecificObject(Asn1Object source)
            : base(source)
            {
            Identifier = (Asn1ObjectIdentifier)source[0];
            }

        private static readonly IDictionary<String, Type> factories = new ConcurrentDictionary<String, Type>();
        private static readonly ReaderWriterLockSlim so = new ReaderWriterLockSlim();

        public static CmsContentSpecificObject From(CmsContentSpecificObject source) {
            EnsureFactory();
            using (ReadLock(so)) {
                if (factories.TryGetValue(source.Identifier.ToString(), out var type)) {
                    if (((Type)type).IsSubclassOf(typeof(CmsContentSpecificObject))) {
                        var r = (CmsContentSpecificObject)Activator.CreateInstance((Type)type, source);
                        return r;
                        }
                    }
                }
            return source;
            }

        private static void EnsureFactory() {
            using (UpgradeableReadLock(so)) {
                if (factories.Count == 0) {
                    using (WriteLock(so)) {
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
                                        e.Data["ExistedType"] = etype.FullName;
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
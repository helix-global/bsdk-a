using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public abstract class X509PublicKeyParameters : Asn1LinkObject
        {
        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();
        private static readonly IDictionary<String,Type> types = new Dictionary<String, Type>();

        #region M:From(String,Asn1Object):Object
        internal static Object From(String key, Asn1Object source) {
            Ensure();
            using (ReadLock(o)) {
                if (types.TryGetValue(key, out var type)) {
                    var r = (X509PublicKeyParameters)Activator.CreateInstance(type,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic,
                        null,
                        new Object[] {source},
                        null
                        );
                    if (!r.IsFailed) {
                        return r;
                        }
                    }
                }
            return source.Body;
            }
        #endregion
        #region M:EnsureFactory
        private static void Ensure() {
            using (UpgradeableReadLock(o)) {
                if (types.Count == 0) {
                    using (WriteLock(o)) {
                        foreach (var type in Assembly.GetExecutingAssembly().
                            GetTypes().
                            Where(i => i.IsSubclassOf(typeof(X509PublicKeyParameters)))) {
                            var attribute = (Asn1SpecificObjectAttribute)type.GetCustomAttributes(typeof(Asn1SpecificObjectAttribute), false).FirstOrDefault();
                            if (attribute != null) {
                                try
                                    {
                                    types.Add(attribute.Key, type);
                                    }
                                catch (ArgumentException e)
                                    {
                                    if (types.TryGetValue(attribute.Key, out var etype)) { 
                                        e.Data["ExistingType"] = etype.FullName;
                                        }
                                    throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        #endregion

        protected X509PublicKeyParameters(Asn1Object source)
            : base(source)
            {
            }
        }
    }
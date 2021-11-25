using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public abstract class Asn1SpecificObject : Asn1LinkObject
        {
        private static readonly ReaderWriterLockSlim so = new ReaderWriterLockSlim();
        private static readonly HashSet<Type> types = new HashSet<Type>();

        protected Asn1SpecificObject(Asn1Object o)
            :base(o)
            {
            }

        #region M:From(Asn1Object):Asn1Object
        public static Asn1Object From(Asn1Object source) {
            EnsureFactory();
            using (ReadLock(so)) {
                foreach (var type in types) {
                    var ci = type.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public, null, new[]{ typeof(Asn1Object)}, null);
                    if (ci != null) {
                        var r = (Asn1Object)ci.Invoke(new Object[]{ source });
                        if (r != null) {
                            if (!r.IsFailed)
                                {
                                return r;
                                }
                            }
                        }
                    }
                }
            return source;
            }
        #endregion
        #region M:EnsureFactory
        private static void EnsureFactory() {
            using (UpgradeableReadLock(so)) {
                if (types.Count == 0) {
                    using (WriteLock(so)) {
                        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(i => i.IsSubclassOf(typeof(Asn1SpecificObject)))) {
                            types.Add(type);
                            }
                        }
                    }
                }
            }
        #endregion
        }
    }

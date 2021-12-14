using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using BinaryStudio.Diagnostics;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal abstract class OpenSSLHashAlgorithm : HashAlgorithm, IHashAlgorithm, IHashOperation
        {
        protected OpenSSLCryptographicContext Context { get; }
        private static readonly IDictionary<String, Type> types = new ConcurrentDictionary<String, Type>();
        private static readonly ReaderWriterLockSlim o = new ReaderWriterLockSlim();

        /// <summary>
        /// Constructs hash engine using specified context.
        /// </summary>
        /// <param name="context">Hash engine cryptographic context.</param>
        protected OpenSSLHashAlgorithm(OpenSSLCryptographicContext context) {
            Context = context;
            }

        /// <inheritdoc/>
        public Byte[] Compute(Byte[] bytes)
            {
            if (bytes == null) { throw new ArgumentNullException(nameof(bytes)); }
            using (new TraceScope(bytes.Length)) {
                return ComputeHash(bytes);
                }
            }

        public void CreateSignature(Stream signature, KeySpec keyspec)
            {
            throw new NotImplementedException();
            }

        public Boolean VerifySignature(out Exception e, Byte[] signature, Byte[] digest, ICryptKey key)
            {
            throw new NotImplementedException();
            }

        void IHashOperation.HashCore(Byte[] array, Int32 startindex, Int32 size)
            {
            HashCore(array, startindex, size);
            }

        internal static OpenSSLHashAlgorithm Create(OpenSSLCryptographicContext context, String identifer) {
            EnsureFactory();
            using (ReadLock(o)) {
                if (types.TryGetValue(identifer, out var type)) {
                    var r = (OpenSSLHashAlgorithm)Activator.CreateInstance(type,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic,
                        null,
                        new Object[] {context},
                        null
                        );
                    return r;
                    }
                }
            throw new NotSupportedException($"Algorithm {{{identifer}}} is not supported.");
            }

        private static void EnsureFactory() {
            using (UpgradeableReadLock(o)) {
                if (types.Count == 0) {
                    using (WriteLock(o)) {
                        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                            if (typeof(OpenSSLHashAlgorithm).IsAssignableFrom(type)) {
                                var attribute = (AlgorithmIdentiferAttribute)type.GetCustomAttributes(typeof(AlgorithmIdentiferAttribute), false).FirstOrDefault();
                                if (attribute != null) {
                                    try
                                        {
                                        types.Add(attribute.AlgorithmIdentifer, type);
                                        }
                                    catch (ArgumentException e)
                                        {
                                        if (types.TryGetValue(attribute.AlgorithmIdentifer, out var etype)) {
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
            }

        private class ReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public ReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterReadLock();
                }

            public void Dispose()
                {
                o.ExitReadLock();
                o = null;
                }
            }

        private class UpgradeableReadLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public UpgradeableReadLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterUpgradeableReadLock();
                }

            public void Dispose()
                {
                o.ExitUpgradeableReadLock();
                o = null;
                }
            }

        private class WriteLockScope : IDisposable
            {
            private ReaderWriterLockSlim o;
            public WriteLockScope(ReaderWriterLockSlim o)
                {
                this.o = o;
                o.EnterWriteLock();
                }

            public void Dispose()
                {
                o.ExitWriteLock();
                o = null;
                }
            }

        protected internal static IDisposable ReadLock(ReaderWriterLockSlim o)            { return new ReadLockScope(o);            }
        protected internal static IDisposable WriteLock(ReaderWriterLockSlim o)           { return new WriteLockScope(o);           }
        protected internal static IDisposable UpgradeableReadLock(ReaderWriterLockSlim o) { return new UpgradeableReadLockScope(o); }
        }
    }
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public partial class CryptographicContext
        {
        #region M:DecryptMessageDER(Byte[],X509CertificateStorage):Byte[]
        private unsafe Byte[] DecryptMessageDER(Byte[] inputblock, X509CertificateStorage storage) {
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblock.Length)) {
                var c = 0;
                var para = new CRYPT_DECRYPT_MESSAGE_PARA
                    {
                    Size = sizeof(CRYPT_DECRYPT_MESSAGE_PARA),
                    CertStoreCount = 1,
                    CertStore = (IntPtr*)LocalAlloc(sizeof(IntPtr)),
                    MsgAndCertEncodingType = CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    #if CRYPT_DECRYPT_MESSAGE_PARA_HAS_EXTRA_FIELDS
                    Flags = CRYPT_MESSAGE_SILENT_KEYSET_FLAG
                    #endif
                    };
                para.CertStore[0] = storage.Handle;
                Validate(CryptDecryptMessage(ref para,inputblock,inputblock.Length, null, ref c, IntPtr.Zero));
                var r = new Byte[c];
                Validate(CryptDecryptMessage(ref para,inputblock,inputblock.Length, r, ref c, IntPtr.Zero));
                return r;
                }
            }
        #endregion
        #region M:DecryptMessageDER(IntPtr,Int32,X509CertificateStorage):Byte[]
        private unsafe Byte[] DecryptMessageDER(IntPtr inputblock,Int32 blocksize, X509CertificateStorage storage) {
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            if (inputblock == IntPtr.Zero) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(blocksize)) {
                var c = 0;
                var para = new CRYPT_DECRYPT_MESSAGE_PARA
                    {
                    Size = sizeof(CRYPT_DECRYPT_MESSAGE_PARA),
                    CertStoreCount = 1,
                    CertStore = (IntPtr*)LocalAlloc(sizeof(IntPtr)),
                    MsgAndCertEncodingType = CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    #if CRYPT_DECRYPT_MESSAGE_PARA_HAS_EXTRA_FIELDS
                    Flags = CRYPT_MESSAGE_SILENT_KEYSET_FLAG
                    #endif
                    };
                para.CertStore[0] = storage.Handle;
                Validate(CryptDecryptMessage(ref para,inputblock,blocksize, null, ref c, IntPtr.Zero));
                var r = new Byte[c];
                Validate(CryptDecryptMessage(ref para,inputblock,blocksize, r, ref c, IntPtr.Zero));
                return r;
                }
            }
        #endregion
        #region M:DecryptMessageDER(Byte[],IX509CertificateStorage,X509Certificate[Out]):Byte[]
        private unsafe Byte[] DecryptMessageDER(Byte[] inputblock, IX509CertificateStorage storage, out X509Certificate certificate) {
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblock.Length)) {
                var c = 0;
                var cert = IntPtr.Zero;
                var para = new CRYPT_DECRYPT_MESSAGE_PARA
                    {
                    Size = sizeof(CRYPT_DECRYPT_MESSAGE_PARA),
                    CertStoreCount = 1,
                    CertStore = (IntPtr*)LocalAlloc(sizeof(IntPtr)),
                    MsgAndCertEncodingType = CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING
                    };
                para.CertStore[0] = storage.Handle;
                Validate(CryptDecryptMessage(ref para,inputblock,inputblock.Length, null, ref c, IntPtr.Zero));
                var r = new Byte[c];
                Validate(CryptDecryptMessage(ref para,inputblock,inputblock.Length, r, ref c, ref cert));
                certificate = new X509Certificate(cert);
                return r;
                }
            }
        #endregion
        #region M:DecryptMessageDER(IntPtr,Int32,X509CertificateStorage,X509Certificate[Out]):Byte[]
        private unsafe Byte[] DecryptMessageDER(IntPtr inputblock,Int32 inputblocksize, IX509CertificateStorage storage, out X509Certificate certificate) {
            if (storage == null) { throw new ArgumentNullException(nameof(storage)); }
            if (inputblock == IntPtr.Zero) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblocksize)) {
                var c = 0;
                var cert = IntPtr.Zero;
                var para = new CRYPT_DECRYPT_MESSAGE_PARA
                    {
                    Size = sizeof(CRYPT_DECRYPT_MESSAGE_PARA),
                    CertStoreCount = 1,
                    CertStore = (IntPtr*)LocalAlloc(sizeof(IntPtr)),
                    MsgAndCertEncodingType = CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING
                    };
                para.CertStore[0] = storage.Handle;
                Validate(CryptDecryptMessage(ref para,inputblock,inputblocksize, null, ref c, IntPtr.Zero));
                var r = new Byte[c];
                Validate(CryptDecryptMessage(ref para,inputblock,inputblocksize, r, ref c, ref cert));
                certificate = new X509Certificate(cert);
                return r;
                }
            }
        #endregion
        #region M:DecryptMessageDER(Byte[],X509Certificate):Byte[]
        private Byte[] DecryptMessageDER(Byte[] inputblock, X509Certificate certificate) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            using (new TraceScope(inputblock.Length)) {
                using (var store = new X509CertificateStorage(new []{certificate })) {
                    return DecryptMessageDER(inputblock, store);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageDER(IntPtr,Int32,X509Certificate):Byte[]
        private Byte[] DecryptMessageDER(IntPtr inputblock,Int32 inputblocksize, X509Certificate certificate) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            using (new TraceScope(inputblocksize)) {
                using (var store = new X509CertificateStorage(new []{certificate })) {
                    return DecryptMessageDER(inputblock,inputblocksize,store);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageDER(IEnumerable<Byte[]>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> DecryptMessageDER(IEnumerable<Byte[]> inputstream, Int32 nthreads = 16) {
            if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
            using (var trace = new TraceScope(new StackTrace(true).GetFrame(1)))
            using (var store = new CryptographicContextStorage(this)) {
                var status = 0;
                using (ReaderWriterLockSlim
                    readL = new ReaderWriterLockSlim(),
                    wrteL = new ReaderWriterLockSlim(),
                    rsltL = new ReaderWriterLockSlim()
                    )
                    {
                    var wrteQ = new LinkedList<CryptBlock<Byte[]>>();
                    var rsltQ = new Queue<Byte[]>();
                    var readQ = new Queue<CryptBlock<Byte[]>>();
                    var crrntI = 0;
                    var tasks = new List<Thread>();
                    var firstblock = true;
                    X509Certificate certificate = null;

                    var rsltT = new Thread(()=>{
                        for (;;) {
                            Yield();
                            using (UpgradeableReadLock(wrteL)) {
                                if (wrteQ.Count > 0) {
                                    using (WriteLock(wrteL)) {
                                        for (var i = wrteQ.First; i != null; i = i.Next) {
                                            if (i.Value.Index == crrntI) {
                                                using (WriteLock(rsltL)) {
                                                    rsltQ.Enqueue(i.Value.Block);
                                                    wrteQ.Remove(i);
                                                    crrntI++;
                                                    break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                if (wrteQ.Count == 0) {
                                    using (ReadLock(rsltL)) {
                                        if (rsltQ.Count == 0) {
                                            if (status == 2) {
                                                break;
                                                } 
                                            }
                                        }
                                    }
                                }
                            }
                        Yield();
                        status = 3;
                        Yield();
                        });

                    var readT = new Thread(() => {
                        var i = 0;
                        foreach (var block in inputstream) {
                            for (;;)
                                {
                                Yield();
                                using (UpgradeableReadLock(readL)) {
                                    if (readQ.Count < nthreads) {
                                        using (WriteLock(readL)) {
                                            readQ.Enqueue(new CryptBlock<Byte[]>(i, block));
                                            //Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{block.Item1:D8}]");
                                            i++;
                                            if ((i % 10) == 0)
                                                {
                                                GC.Collect();
                                                }
                                            break;
                                            }
                                        }
                                    }
                                }
                            }
                        status = 1;
                        Yield();
                        foreach (var task in tasks)
                            {
                            task.Join();
                            }
                        status = 2;
                        //Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        Yield();
                        });

                    ThreadStart threadproc = null;
                    threadproc = delegate
                        {
                        for (;;) {
                            CryptBlock<Byte[]> block = null;
                            using (UpgradeableReadLock(readL)) {
                                if (readQ.Count > 0) {
                                    using (WriteLock(readL))
                                        {
                                        block = readQ.Dequeue();
                                        }
                                    }
                                }
                            if (block != null) {
                                var flags = CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_CACHE_FLAG;
                                if (!firstblock)
                                    {
                                    flags |= CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_SILENT_FLAG;
                                    }
                                var r = firstblock
                                    ? DecryptMessageDER(block.Block, store, out certificate)
                                    : DecryptMessageDER(block.Block, certificate);
                                if (firstblock)
                                    {
                                    firstblock = false;
                                    for (var i = 0; i < nthreads - 1; i++) {
                                        var thread = new Thread(threadproc);
                                        tasks.Add(thread);
                                        thread.Start();
                                        }
                                    }
                                using (WriteLock(wrteL)) {
                                    wrteQ.AddLast(new CryptBlock<Byte[]>(block.Index, r));
                                    }
                                }
                            Yield();
                            if (status == 1) {
                                using (UpgradeableReadLock(readL)) {
                                    if (readQ.Count == 0) { break; }
                                    }
                                }
                            }
                        //Debug.Print($"E:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        };

                    tasks.Add(new Thread(threadproc));

                    foreach (var task in tasks) { task.Start(); }
                    readT.Start();
                    rsltT.Start();

                    for (;;)
                        {
                        Yield();
                        Byte[] block = null;
                        using (UpgradeableReadLock(rsltL)) {
                            if (rsltQ.Count > 0) {
                                using (WriteLock(rsltL)) {
                                    block = rsltQ.Dequeue();
                                    }
                                }
                            }
                        if (block != null) {
                            //trace.DataSize += block.Length;
                            yield return block;
                            }
                        if (status == 3) { break; }
                        }

                    rsltT.Join();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageDER(IEnumerable<MemoryBlock>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> DecryptMessageDER(IEnumerable<MemoryBlock> inputstream, Int32 nthreads = 16) {
            if (IntPtr.Size == sizeof(Int32)) { throw new NotSupportedException(); }
            if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
            using (var trace = new TraceScope(new StackTrace(true).GetFrame(1)))
            using (var store = new CryptographicContextStorage(this)) {
                var status = 0;
                using (ReaderWriterLockSlim
                    readL = new ReaderWriterLockSlim(),
                    wrteL = new ReaderWriterLockSlim(),
                    rsltL = new ReaderWriterLockSlim()
                    )
                    {
                    var wrteQ = new LinkedList<CryptBlock<Byte[]>>();
                    var rsltQ = new Queue<Byte[]>();
                    var readQ = new Queue<CryptBlock<MemoryBlock>>();
                    var crrntI = 0;
                    var tasks = new List<Thread>();
                    var firstblock = true;
                    X509Certificate certificate = null;

                    var rsltT = new Thread(()=>{
                        for (;;) {
                            Yield();
                            using (UpgradeableReadLock(wrteL)) {
                                if (wrteQ.Count > 0) {
                                    using (WriteLock(wrteL)) {
                                        for (var i = wrteQ.First; i != null; i = i.Next) {
                                            if (i.Value.Index == crrntI) {
                                                using (WriteLock(rsltL)) {
                                                    rsltQ.Enqueue(i.Value.Block);
                                                    wrteQ.Remove(i);
                                                    crrntI++;
                                                    break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                if (wrteQ.Count == 0) {
                                    using (ReadLock(rsltL)) {
                                        if (rsltQ.Count == 0) {
                                            if (status == 2) {
                                                break;
                                                } 
                                            }
                                        }
                                    }
                                }
                            }
                        Yield();
                        status = 3;
                        Yield();
                        });

                    var readT = new Thread(() => {
                        var i = 0;
                        foreach (var block in inputstream) {
                            for (;;)
                                {
                                Yield();
                                using (UpgradeableReadLock(readL)) {
                                    if (readQ.Count < nthreads) {
                                        using (WriteLock(readL)) {
                                            readQ.Enqueue(new CryptBlock<MemoryBlock>(i, block));
                                            //Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{block.Item1:D8}]");
                                            i++;
                                            if ((i % 10) == 0)
                                                {
                                                GC.Collect();
                                                }
                                            break;
                                            }
                                        }
                                    }
                                }
                            }
                        status = 1;
                        Yield();
                        foreach (var task in tasks)
                            {
                            task.Join();
                            }
                        status = 2;
                        //Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        Yield();
                        });

                    ThreadStart threadproc = null;
                    threadproc = delegate
                        {
                        for (;;) {
                            CryptBlock<MemoryBlock> block = null;
                            using (UpgradeableReadLock(readL)) {
                                if (readQ.Count > 0) {
                                    using (WriteLock(readL))
                                        {
                                        block = readQ.Dequeue();
                                        }
                                    }
                                }
                            if (block != null) {
                                var flags = CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_CACHE_FLAG;
                                if (!firstblock)
                                    {
                                    flags |= CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_SILENT_FLAG;
                                    }
                                var r = firstblock
                                    ? DecryptMessageDER(block.Block.Block,(Int32)block.Block.Size, store, out certificate)
                                    : DecryptMessageDER(block.Block.Block,(Int32)block.Block.Size, certificate);
                                if (firstblock)
                                    {
                                    firstblock = false;
                                    for (var i = 0; i < nthreads - 1; i++) {
                                        var thread = new Thread(threadproc);
                                        tasks.Add(thread);
                                        thread.Start();
                                        }
                                    }
                                using (WriteLock(wrteL)) {
                                    wrteQ.AddLast(new CryptBlock<Byte[]>(block.Index, r));
                                    }
                                }
                            Yield();
                            if (status == 1) {
                                using (UpgradeableReadLock(readL)) {
                                    if (readQ.Count == 0) { break; }
                                    }
                                }
                            }
                        //Debug.Print($"E:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        };

                    tasks.Add(new Thread(threadproc));

                    foreach (var task in tasks) { task.Start(); }
                    readT.Start();
                    rsltT.Start();

                    for (;;)
                        {
                        Yield();
                        Byte[] block = null;
                        using (UpgradeableReadLock(rsltL)) {
                            if (rsltQ.Count > 0) {
                                using (WriteLock(rsltL)) {
                                    block = rsltQ.Dequeue();
                                    }
                                }
                            }
                        if (block != null) {
                            //trace.DataSize += block.Length;
                            yield return block;
                            }
                        if (status == 3) { break; }
                        }

                    rsltT.Join();
                    }
                }
            }
        #endregion

        [DllImport("crypt32.dll",  BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, [MarshalAs(UnmanagedType.LPArray)] Byte[] encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, ref IntPtr r);
        [DllImport("crypt32.dll",  BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, [MarshalAs(UnmanagedType.LPArray)] Byte[] encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, IntPtr r);
        [DllImport("crypt32.dll",  BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, IntPtr encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, ref IntPtr r);
        [DllImport("crypt32.dll",  BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CryptDecryptMessage(ref CRYPT_DECRYPT_MESSAGE_PARA para, IntPtr encblob, Int32 encblobsize, [MarshalAs(UnmanagedType.LPArray)] Byte[] decblob, ref Int32 decblobsize, IntPtr r);
        }
    }
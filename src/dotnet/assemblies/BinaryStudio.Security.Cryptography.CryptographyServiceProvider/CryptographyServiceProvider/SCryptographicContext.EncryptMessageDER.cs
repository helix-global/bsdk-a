using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public partial class SCryptographicContext
        {
        #region M:EncryptMessageDER(IntPtr,IList<IX509Certificate>,Byte[],Int32):Byte[]
        private unsafe Byte[] EncryptMessageDER(IntPtr algid, IList<IX509Certificate> recipients, Byte[] block, Int32 blocksize) {
            if (block == null)              { throw new ArgumentNullException(nameof(block));            }
            if (algid == IntPtr.Zero)       { throw new ArgumentOutOfRangeException(nameof(algid));      }
            if (recipients == null)         { throw new ArgumentNullException(nameof(recipients));       }
            if (recipients.Count == 0)      { throw new ArgumentOutOfRangeException(nameof(recipients)); }
            if (blocksize > block.Length)   { throw new ArgumentOutOfRangeException(nameof(blocksize));  }
            using (new TraceScope(blocksize)) {
                var para = new CRYPT_ENCRYPT_MESSAGE_PARA
                    {
                    Size = sizeof(CRYPT_ENCRYPT_MESSAGE_PARA),
                    InnerContentType = 0,
                    Flags = CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE,
                    MsgEncodingType = CRYPT_MSG_TYPE.X509_ASN_ENCODING | CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    ContentEncryptionAlgorithm = new CRYPT_ALGORITHM_IDENTIFIER {ObjectId = algid},
                    CryptProv = Handle
                    };
                var certificates = new CERT_CONTEXT*[recipients.Count];
                var i = 0;
                foreach (var recipient in recipients) {
                    certificates[i] = (CERT_CONTEXT*)recipient.Handle;
                    i++;
                    }
                fixed (CERT_CONTEXT** certs = certificates) {
                    fixed (Byte* iblock = block) {
                        var sz = 0;
                        Validate(CryptEncryptMessage(&para, certificates.Length, certs,iblock,blocksize,null,&sz));
                        var output = new Byte[sz];
                        fixed (Byte* oblock = output) {
                            Validate(CryptEncryptMessage(&para, certificates.Length, certs,iblock,blocksize,oblock,&sz));
                            var r = new Byte[sz];
                            Array.Copy(output, r, sz);
                            return r;
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:EncryptMessageDER(Oid,IList<IX509Certificate>,Byte[],Int32):Byte[]
        public Byte[] EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Byte[] block, Int32 blocksize) {
            if (algid == null)                    { throw new ArgumentNullException(nameof(algid)); }
            using (new TraceScope(blocksize)) {
                var algorithm = Marshal.StringToCoTaskMemAnsi(algid.Value);
                try
                    {
                    return EncryptMessageDER(algorithm, recipients, block, blocksize);
                    }
                finally
                    {
                    Marshal.FreeCoTaskMem(algorithm);
                    }
                }
            }
        #endregion
        #region M:EncryptMessageDER(Oid,IList<IX509Certificate>,Byte[]):Byte[]
        public Byte[] EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Byte[] block) {
            if (block == null) { throw new ArgumentNullException(nameof(block)); }
            using (new TraceScope(block.Length)) {
                return EncryptMessageDER(algid, recipients, block, block.Length);
                }
            }
        #endregion
        #region M:EncryptMessageDER(Oid,IList<IX509Certificate>,Stream,Stream)
        public void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream) {
            using (new TraceScope()) {
                if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
                if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
                using (var istream = new MemoryStream()) {
                    inputstream.CopyTo(istream);
                    var r = EncryptMessageDER(algid, recipients, istream.ToArray());
                    outputstream.Write(r, 0, r.Length);
                    }
                }
            }
        #endregion
        #region M:EncryptMessageDER(Oid,IList<IX509Certificate>,IEnumerable<Byte[]>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, IEnumerable<Byte[]> inputstream, Int32 nthreads)
            {
            using (var trace = new TraceScope(new StackTrace(true).GetFrame(1))) {
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
                    #if FEATURE_ENCRYPT_TASK_SUPPORT
                    var tasks = new List<Task>();
                    #else
                    var tasks = new List<Thread>();
                    #endif

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
                                                    //Debug.Print($"W:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{i.Value.Item1:D8}]");
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
                        //Debug.Print($"W:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
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
                                                GC.Collect(2, GCCollectionMode.Optimized);
                                                }
                                            break;
                                            }
                                        }
                                    }
                                }
                            }
                        status = 1;
                        Yield();
                        GC.Collect(2, GCCollectionMode.Optimized);
                        foreach (var task in tasks)
                            {
                            #if FEATURE_ENCRYPT_TASK_SUPPORT
                            task.Wait();
                            #else
                            task.Join();
                            #endif
                            }
                        status = 2;
                        //Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        Yield();
                        });

                    for (var i = 0; i < nthreads; i++) {
                        tasks.Add(
                            #if FEATURE_ENCRYPT_TASK_SUPPORT
                            Task.Factory.StartNew(() =>
                            #else
                            new Thread(() =>
                            #endif
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
                                    //Debug.Print($"E:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{block.Item1:D8}]");
                                    var r = EncryptMessageDER(algid, recipients, block.Block);
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
                            }
                        #if FEATURE_ENCRYPT_TASK_SUPPORT
                            , TaskCreationOptions.LongRunning
                        #endif
                            ));
                        }

                    #if !FEATURE_ENCRYPT_TASK_SUPPORT
                    foreach (var task in tasks) {
                        task.Priority = ThreadPriority.Highest;
                        task.Start();
                        }
                    #endif
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

        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern unsafe Boolean CryptEncryptMessage(CRYPT_ENCRYPT_MESSAGE_PARA* para, Int32 recipientcount, CERT_CONTEXT** recipients, Byte* block, Int32 blocksize, Byte* encryptedblob, [In, Out] Int32* encryptedblobsize);
        }
    }
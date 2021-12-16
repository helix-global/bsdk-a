#define FEATURE_TASK
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
        #region M:EncryptMessageBER(Oid,IList<IX509Certificate>,Stream,Stream)
        public unsafe void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream) {
            if (algid == null)                    { throw new ArgumentNullException(nameof(algid)); }
            if (inputstream == null)  { throw new ArgumentNullException(nameof(inputstream));  }
            if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
            if (recipients == null)   { throw new ArgumentNullException(nameof(recipients)); }
            using (new TraceScope()) {
                var length = (Double)inputstream.Length;
                var m_recipients = LocalAlloc(recipients.Count * IntPtr.Size);
                var ei = new CMSG_ENVELOPED_ENCODE_INFO
                    {
                    Size = sizeof(CMSG_ENVELOPED_ENCODE_INFO),
                    ContentEncryptionAlgorithm = new CRYPT_ALGORITHM_IDENTIFIER
                        {
                        ObjectId = Marshal.StringToCoTaskMemAnsi(algid.Value)
                        }
                    };
                var j = (CERT_INFO**)m_recipients;
                var c = recipients.Count;
                for (var i = 0; i < c; i++) {
                    j[i] = ((CERT_CONTEXT*)recipients[i].Handle)->CertInfo;
                    }
                ei.RecipientsCount = c;
                ei.Recipients = j;
                var so = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                    var bytes = new Byte[size];
                    for (var i = 0; i < size; i++) {
                        bytes[i] = data[i];
                        }
                    outputstream.Write(bytes, 0, bytes.Length);
                    return true;
                    }, IntPtr.Zero);
                using (var r = new CryptographicMessage(
                    CryptMsgOpenToEncode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING | CRYPT_MSG_TYPE.X509_ASN_ENCODING, 0, CMSG_TYPE.CMSG_ENVELOPED, ref ei, IntPtr.Zero, ref so)
                    ))
                    {
                    var block = new Byte[BLOCK_SIZE_64M];
                    var size = 0.0;
                    for (;;) {
                        Yield();
                        var sz = inputstream.Read(block, 0, block.Length);
                        if (sz == 0) { break; }
                        r.Update(block, sz, false);
                        size += sz;
                        }
                    r.Update(block, 0, true);
                    }
                }
            }
        #endregion
        #region M:EncryptMessageBER(Oid,IList<IX509Certificate>,Byte[],Stream)
        public void EncryptMessageBER(Oid encryptionalgorithm, IList<IX509Certificate> recipients, Byte[] input, Stream outputstream) {
            if (input == null)  { throw new ArgumentNullException(nameof(input));  }
            using (new TraceScope(input.Length)) {
                using (var inputstream = new MemoryStream(input))
                    {
                    EncryptMessageBER(
                        encryptionalgorithm, recipients,
                        inputstream, outputstream);
                    }
                }
            }
        #endregion
        #region M:EncryptMessageBER(Oid,IList<IX509Certificate>,Byte[]):Byte[]
        public Byte[] EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Byte[] block) {
            if (block == null) { throw new ArgumentNullException(nameof(block)); }
            using (new TraceScope(block.Length)) {
                using (var inputstream = new MemoryStream(block)) {
                    using (var outputstream = new MemoryStream()) {
                        EncryptMessageBER(algid, recipients, inputstream, outputstream);
                        return outputstream.ToArray();
                        }
                    }
                }
            }
        #endregion
        #region M:EncryptMessageBER(Oid,IList<IX509Certificate>,IEnumerable<Byte[]>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, IEnumerable<Byte[]> inputstream, Int32 nthreads) {
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
                    #if FEATURE_TASK
                    var tasks = new List<Thread>();
                    #else
                    var tasks = new List<Task>();
                    #endif

                    #region rsltT
                    #if FEATURE_TASK
                    var rsltT = new Thread(() =>
                    #else
                    var rsltT = new Task(() =>
                    #endif
                        {
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
                    #endregion
                    #region readT
                    #if FEATURE_TASK
                    var readT = new Thread(() =>
                    #else
                    var readT = new Task(() =>
                    #endif
                        {
                        var i = 0;
                        foreach (var block in inputstream) {
                            for (;;)
                                {
                                //Yield();
                                using (UpgradeableReadLock(readL)) {
                                    if (readQ.Count < nthreads) {
                                        using (WriteLock(readL)) {
                                            readQ.Enqueue(new CryptBlock<Byte[]>(i, block));
                                            Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{block:D8}]");
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
                        //Yield();
                        GC.Collect(2, GCCollectionMode.Optimized);
                        foreach (var task in tasks)
                            {
                            #if FEATURE_TASK
                            task.Join();
                            #else
                            task.Wait();
                            #endif
                            }
                        status = 2;
                        Debug.Print($"R:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                        //Yield();
                        })
                        {
                        #if FEATURE_TASK
                        Priority = ThreadPriority.Highest
                        #endif
                        };
                    #endregion

                    for (var i = 0; i < nthreads; i++) {
                        #if FEATURE_TASK
                        tasks.Add(new Thread(() =>
                        #else
                        tasks.Add(new Task(() =>
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
                                    Debug.Print($"E:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{block.Block:D8}]");
                                    var r = EncryptMessageBER(algid, recipients, block.Block);
                                    //var r = block.Block;
                                    using (WriteLock(wrteL)) {
                                        wrteQ.AddLast(new CryptBlock<Byte[]>(block.Index, r));
                                        }
                                    }
                                //Yield();
                                if (status == 1) {
                                    using (UpgradeableReadLock(readL)) {
                                        if (readQ.Count == 0) { break; }
                                        }
                                    }
                                }
                            Debug.Print($"E:[{Thread.CurrentThread.ManagedThreadId:D3}]:[FINISHED]");
                            }));
                        }

                    foreach (var task in tasks) {
                        #if FEATURE_TASK
                        task.Priority = ThreadPriority.Lowest;
                        #endif
                        task.Start();
                        }
                    #if FEATURE_TASK
                    readT.SetApartmentState(ApartmentState.MTA);
                    #endif
                    readT.Start();
                    rsltT.Start();

                    for (;;)
                        {
                        //Yield();
                        Byte[] block = null;
                        using (UpgradeableReadLock(rsltL)) {
                            //Debug.Print($"F:[{Thread.CurrentThread.ManagedThreadId:D3}]:[{rsltQ.Count}]");
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

                    #if FEATURE_TASK
                    rsltT.Join();
                    #else
                    rsltT.Wait();
                    #endif
                    }
                }
            }
        #endregion
        }
    }
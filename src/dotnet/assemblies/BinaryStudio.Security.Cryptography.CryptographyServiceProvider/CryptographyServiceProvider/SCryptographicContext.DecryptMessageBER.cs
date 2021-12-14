using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public partial class CryptographicContext
        {
        #region M:DecryptMessageBER(Stream,Stream,X509Certificate[],X509Certificate[Out],Int32[Out],UInt32)
        private unsafe void DecryptMessageBER(Stream inputstream, Stream outputstream, X509Certificate[] certificates, out X509Certificate certificate, out Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            certificate = null;
            recipientindex = -1;
            using (new TraceScope()) {
                if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
                if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
                IntPtr msg;
                var si = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                    var bytes = new Byte[size];
                    for (var i = 0; i < size; i++) {
                        bytes[i] = data[i];
                        }
                    outputstream.Write(bytes, 0, bytes.Length);
                    return true;
                    }, IntPtr.Zero);
                msg = CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    CRYPT_OPEN_MESSAGE_FLAGS.CMSG_NONE,
                    CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero,
                    ref si);
                if (msg == IntPtr.Zero) { Validate(GetHRForLastWin32Error()); }
                var decrypting = false;
                using (var m = new CryptographicMessage(msg)) {
                    var block = new Byte[BLOCK_SIZE_64K];
                    for (;;) {
                        Yield();
                        var sz = inputstream.Read(block, 0, block.Length);
                        if (sz == 0) { break; }
                        m.Update(block, sz, false);
                        if (!decrypting) {
                            var r = m.GetParameter(CMSG_PARAM.CMSG_ENVELOPE_ALGORITHM_PARAM, 0, out var hr);
                            if (hr == 0) {
                                fixed (Byte* p = r) { Debug.Print($"algid:{Marshal.PtrToStringAnsi(((CRYPT_ALGORITHM_IDENTIFIER*)p)->ObjectId)}"); }
                                var a = new List<Exception>();
                                for (var index = 0;;index++) {
                                    r = m.GetParameter(CMSG_PARAM.CMSG_CMS_RECIPIENT_INFO_PARAM, index);
                                    String serialnumber;
                                    fixed (Byte* p = r) {
                                        String issuer;
                                        if (IntPtr.Size == 4)
                                            {
                                            var keytrans = ((CMSG_CMS_RECIPIENT_INFO32*)p)->KeyTrans;
                                            var certid = keytrans->RecipientId;
                                            serialnumber = DecodeSerialNumberString(ref certid.IssuerSerialNumber.SerialNumber);
                                            issuer       = DecodeNameString(ref certid.IssuerSerialNumber.Issuer);
                                            }
                                        else
                                            {
                                            var keytrans = ((CMSG_CMS_RECIPIENT_INFO64*)p)->KeyTrans;
                                            var certid = keytrans->RecipientId;
                                            serialnumber = DecodeSerialNumberString(ref certid.IssuerSerialNumber.SerialNumber);
                                            issuer       = DecodeNameString(ref certid.IssuerSerialNumber.Issuer);
                                            }
                                        certificate  = certificates.FirstOrDefault(i => i.SerialNumber == serialnumber);
                                        if (certificate == null)
                                            {
                                            a.Add(new Exception($"Certificate [{serialnumber}][{issuer}] not found in local system."));
                                            continue;
                                            }

                                        using (var context = new CryptographicContext(this, certificate, flags)) {
                                            var para = new CMSG_CTRL_DECRYPT_PARA {
                                                Size = sizeof(CMSG_CTRL_DECRYPT_PARA),
                                                KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec,
                                                CryptProv = context.Handle,
                                                RecipientIndex = index
                                                };
                                            m.Control(CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE, CMSG_CTRL.CMSG_CTRL_DECRYPT, ref para);
                                            decrypting = true;
                                            block = new Byte[BLOCK_SIZE_64M];
                                            recipientindex = index;
                                            break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    m.Update(block, 0, true);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Stream,Stream,X509Certificate,Int32,CRYPT_ACQUIRE_FLAGS)
        private unsafe void DecryptMessageBER(Stream inputstream, Stream outputstream, X509Certificate certificate, Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            using (new TraceScope()) {
                if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
                if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
                IntPtr msg;
                var si = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                    var bytes = new Byte[size];
                    for (var i = 0; i < size; i++) {
                        bytes[i] = data[i];
                        }
                    outputstream.Write(bytes, 0, bytes.Length);
                    return true;
                    }, IntPtr.Zero);
                msg = CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    CRYPT_OPEN_MESSAGE_FLAGS.CMSG_NONE,
                    CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero,
                    ref si);
                if (msg == IntPtr.Zero) { Validate(GetHRForLastWin32Error()); }
                var decrypting = false;
                using (var m = new CryptographicMessage(msg)) {
                    var block = new Byte[BLOCK_SIZE_64K];
                    for (;;) {
                        Yield();
                        var sz = inputstream.Read(block, 0, block.Length);
                        if (sz == 0) { break; }
                        m.Update(block, sz, false);
                        if (!decrypting) {
                            m.GetParameter(CMSG_PARAM.CMSG_ENVELOPE_ALGORITHM_PARAM, 0, out var hr);
                            if (hr == 0) {
                                using (var context = new CryptographicContext(this, certificate, flags)) {
                                    var para = new CMSG_CTRL_DECRYPT_PARA {
                                        Size = sizeof(CMSG_CTRL_DECRYPT_PARA),
                                        KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec,
                                        CryptProv = context.Handle,
                                        RecipientIndex = recipientindex
                                        };
                                    m.Control(CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE, CMSG_CTRL.CMSG_CTRL_DECRYPT, ref para);
                                    decrypting = true;
                                    block = new Byte[BLOCK_SIZE_64M];
                                    }
                                }
                            }
                        }
                    m.Update(block, 0, true);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Stream,Stream)
        public void DecryptMessageBER(Stream inputstream, Stream outputstream) {
            using (new TraceScope()) {
                if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
                if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
                using (var store = new CryptographicContextStorage(this)) {
                    var certificates = store.Certificates.ToArray();
                    DecryptMessageBER(inputstream, outputstream, certificates,
                        out X509Certificate certificate, out Int32 recipientindex,
                        CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_CACHE_FLAG);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Byte[]):Byte[]
        public Byte[] DecryptMessageBER(Byte[] inputblock) {
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblock.Length)) {
                using (var r = new MemoryStream()) {
                    using (var inputstream = new MemoryStream(inputblock)) {
                        DecryptMessageBER(inputstream, r);
                        }
                    return r.ToArray();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Byte[],X509Certificate[],X509Certificate[Out],Int32[Out],CRYPT_ACQUIRE_FLAGS):Byte[]
        private Byte[] DecryptMessageBER(Byte[] inputblock, X509Certificate[] certificates, out X509Certificate certificate, out Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblock.Length)) {
                using (var r = new MemoryStream()) {
                    using (var inputstream = new MemoryStream(inputblock)) {
                        DecryptMessageBER(inputstream, r, certificates, out certificate, out recipientindex, flags);
                        }
                    return r.ToArray();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Byte*,Int32,Stream,X509Certificate[],X509Certificate[Out],Int32[Out],CRYPT_ACQUIRE_FLAGS)
        private unsafe void DecryptMessageBER(Byte* inputblock,Int32 inputblocksize, Stream outputstream, X509Certificate[] certificates, out X509Certificate certificate, out Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            certificate = null;
            recipientindex = -1;
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
            using (new TraceScope(inputblocksize)) {
                IntPtr msg;
                var si = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                    var bytes = new Byte[size];
                    for (var i = 0; i < size; i++) {
                        bytes[i] = data[i];
                        }
                    outputstream.Write(bytes, 0, bytes.Length);
                    return true;
                    }, IntPtr.Zero);
                msg = CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    CRYPT_OPEN_MESSAGE_FLAGS.CMSG_NONE,
                    CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero,
                    ref si);
                if (msg == IntPtr.Zero) { Validate(GetHRForLastWin32Error()); }
                var decrypting = false;
                using (var m = new CryptographicMessage(msg)) {
                    var n = BLOCK_SIZE_64K;
                    for (;;) {
                        Yield();
                        var sz = Math.Min(n, inputblocksize);
                        if (sz == 0) { break; }
                        m.Update(inputblock, sz, false);
                        inputblocksize -= sz;
                        inputblock     += sz;
                        if (!decrypting) {
                            var r = m.GetParameter(CMSG_PARAM.CMSG_ENVELOPE_ALGORITHM_PARAM, 0, out var hr);
                            if (hr == 0) {
                                fixed (Byte* p = r) { Debug.Print($"algid:{Marshal.PtrToStringAnsi(((CRYPT_ALGORITHM_IDENTIFIER*)p)->ObjectId)}"); }
                                var a = new List<Exception>();
                                for (var index = 0;;index++) {
                                    r = m.GetParameter(CMSG_PARAM.CMSG_CMS_RECIPIENT_INFO_PARAM, index);
                                    String serialnumber;
                                    fixed (Byte* p = r) {
                                        String issuer;
                                        if (IntPtr.Size == 4)
                                            {
                                            var keytrans = ((CMSG_CMS_RECIPIENT_INFO32*)p)->KeyTrans;
                                            var certid = keytrans->RecipientId;
                                            serialnumber = DecodeSerialNumberString(ref certid.IssuerSerialNumber.SerialNumber);
                                            issuer       = DecodeNameString(ref certid.IssuerSerialNumber.Issuer);
                                            }
                                        else
                                            {
                                            var keytrans = ((CMSG_CMS_RECIPIENT_INFO64*)p)->KeyTrans;
                                            var certid = keytrans->RecipientId;
                                            serialnumber = DecodeSerialNumberString(ref certid.IssuerSerialNumber.SerialNumber);
                                            issuer       = DecodeNameString(ref certid.IssuerSerialNumber.Issuer);
                                            }
                                        certificate  = certificates.FirstOrDefault(i => i.SerialNumber == serialnumber);
                                        if (certificate == null)
                                            {
                                            a.Add(new Exception($"Certificate [{serialnumber}][{issuer}] not found in local system."));
                                            continue;
                                            }

                                        using (var context = new CryptographicContext(this, certificate, flags)) {
                                            var para = new CMSG_CTRL_DECRYPT_PARA {
                                                Size = sizeof(CMSG_CTRL_DECRYPT_PARA),
                                                KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec,
                                                CryptProv = context.Handle,
                                                RecipientIndex = index
                                                };
                                            m.Control(CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE, CMSG_CTRL.CMSG_CTRL_DECRYPT, ref para);
                                            decrypting = true;
                                            n = BLOCK_SIZE_64M;
                                            recipientindex = index;
                                            break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    m.Update(inputblock, 0, true);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Byte*,Int32,Stream,X509Certificate,Int32,CRYPT_ACQUIRE_FLAGS)
        private unsafe void DecryptMessageBER(Byte* inputblock,Int32 inputblocksize, Stream outputstream, X509Certificate certificate, Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (certificate == null) { throw new ArgumentNullException(nameof(certificate)); }
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            if (outputstream == null) { throw new ArgumentNullException(nameof(outputstream)); }
            using (new TraceScope(inputblocksize)) {
                IntPtr msg;
                var si = new CMSG_STREAM_INFO(CMSG_INDEFINITE_LENGTH,delegate(IntPtr arg, Byte* data, UInt32 size, Boolean final) {
                    var bytes = new Byte[size];
                    for (var i = 0; i < size; i++) {
                        bytes[i] = data[i];
                        }
                    outputstream.Write(bytes, 0, bytes.Length);
                    return true;
                    }, IntPtr.Zero);
                msg = CryptMsgOpenToDecode(CRYPT_MSG_TYPE.PKCS_7_ASN_ENCODING,
                    CRYPT_OPEN_MESSAGE_FLAGS.CMSG_NONE,
                    CMSG_TYPE.CMSG_NONE, IntPtr.Zero, IntPtr.Zero,
                    ref si);
                if (msg == IntPtr.Zero) { Validate(GetHRForLastWin32Error()); }
                var decrypting = false;
                using (var m = new CryptographicMessage(msg)) {
                    var n = BLOCK_SIZE_64K;
                    for (;;) {
                        Yield();
                        var sz = Math.Min(n, inputblocksize);
                        if (sz == 0) { break; }
                        m.Update(inputblock, sz, false);
                        inputblock     += sz;
                        inputblocksize -= sz;
                        if (!decrypting) {
                            m.GetParameter(CMSG_PARAM.CMSG_ENVELOPE_ALGORITHM_PARAM, 0, out var hr);
                            if (hr == 0) {
                                using (var context = new CryptographicContext(this, certificate, flags)) {
                                    var para = new CMSG_CTRL_DECRYPT_PARA {
                                        Size = sizeof(CMSG_CTRL_DECRYPT_PARA),
                                        KeySpec = (KEY_SPEC_TYPE)certificate.KeySpec,
                                        CryptProv = context.Handle,
                                        RecipientIndex = recipientindex
                                        };
                                    m.Control(CRYPT_MESSAGE_FLAGS.CRYPT_MESSAGE_NONE, CMSG_CTRL.CMSG_CTRL_DECRYPT, ref para);
                                    decrypting = true;
                                    n = BLOCK_SIZE_64M;
                                    }
                                }
                            }
                        }
                    m.Update(inputblock, 0, true);
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(Byte[],X509Certificate,Int32,CRYPT_ACQUIRE_FLAGS):Byte[]
        private Byte[] DecryptMessageBER(Byte[] inputblock, X509Certificate certificate, Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(inputblock.Length)) {
                using (var r = new MemoryStream()) {
                    using (var inputstream = new MemoryStream(inputblock)) {
                        DecryptMessageBER(inputstream, r, certificate, recipientindex, flags);
                        }
                    return r.ToArray();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(IntPtr,Int32,X509Certificate[],X509Certificate[Out],Int32[Out],CRYPT_ACQUIRE_FLAGS):Byte[]
        private unsafe Byte[] DecryptMessageBER(IntPtr inputblock,Int32 blocksize, X509Certificate[] certificates, out X509Certificate certificate, out Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(blocksize)) {
                using (var r = new MemoryStream()) {
                    DecryptMessageBER((Byte*)inputblock, blocksize, r, certificates, out certificate, out recipientindex, flags);
                    return r.ToArray();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(IntPtr,Int32,X509Certificate,Int32,CRYPT_ACQUIRE_FLAGS):Byte[]
        private unsafe Byte[] DecryptMessageBER(IntPtr inputblock, Int32 blocksize, X509Certificate certificate, Int32 recipientindex, CRYPT_ACQUIRE_FLAGS flags) {
            if (inputblock == null) { throw new ArgumentNullException(nameof(inputblock)); }
            using (new TraceScope(blocksize)) {
                using (var r = new MemoryStream()) {
                    DecryptMessageBER((Byte*)inputblock,blocksize, r, certificate, recipientindex, flags);
                    return r.ToArray();
                    }
                }
            }
        #endregion
        #region M:DecryptMessageBER(IEnumerable<Byte[]>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> DecryptMessageBER(IEnumerable<Byte[]> inputstream, Int32 nthreads = 16) {
            if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
            using (var trace = new TraceScope((new StackTrace(true).GetFrame(1))))
            using (var store = new CryptographicContextStorage(this)) {
                var certificates = store.Certificates.ToArray();
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
                    Int32 recipientindex = -1;

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
                                    ? DecryptMessageBER(block.Block, certificates, out certificate, out recipientindex, flags)
                                    : DecryptMessageBER(block.Block, certificate, recipientindex, flags);
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
        #region M:DecryptMessageBER(IEnumerable<MemoryBlock>,Int32):IEnumerable<Byte[]>
        public IEnumerable<Byte[]> DecryptMessageBER(IEnumerable<MemoryBlock> inputstream, Int32 nthreads = 16) {
            if (IntPtr.Size == sizeof(Int32)) { throw new NotSupportedException(); }
            if (inputstream == null) { throw new ArgumentNullException(nameof(inputstream)); }
            using (var trace = new TraceScope(new StackTrace(true).GetFrame(1)))
            using (var store = new CryptographicContextStorage(this)) {
                var certificates = store.Certificates.ToArray();
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
                    Int32 recipientindex = -1;

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
                                    ? DecryptMessageBER(block.Block.Block, (Int32)block.Block.Size, certificates, out certificate, out recipientindex, flags)
                                    : DecryptMessageBER(block.Block.Block, (Int32)block.Block.Size, certificate, recipientindex, flags);
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
        }
    }
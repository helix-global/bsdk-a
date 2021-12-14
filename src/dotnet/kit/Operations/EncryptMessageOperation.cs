using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BinaryStudio.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Kit;
using Options;
using Options.Descriptors;

namespace Operations
    {
    internal class EncryptMessageOperation : CreateOrEncryptMessageOperation
        {
        private static readonly Int32 DEFAULT_BLOCK_SIZE = (IntPtr.Size == 4)
            ?  5*1024*1024
            : 16*1024*1024;

        public AlgId AlgId { get; }
        public Boolean IsBlock { get; }
        public Int32 BlockSize { get; }
        public CryptographicMessageFlags MessageFlags { get; }
        public EncryptMessageOperation(TextWriter output, IList<OperationOption> args)
            : base(output, args)
            {
            AlgId = args.OfType<AlgId>().FirstOrDefault();
            var flags = args.OfType<EncryptOption>().FirstOrDefault();
            if (flags != null) {
                if (flags.HasValue("indefinite")) { MessageFlags |= CryptographicMessageFlags.IndefiniteLength; }
                if (flags.HasValue("block")) {
                    IsBlock = true;
                    BlockSize = DEFAULT_BLOCK_SIZE;
                    }
                else
                    {
                    
                    }
                }
            else
                {
                MessageFlags = 0;
                }
            if (AlgId == null) {
                throw new OptionRequiredException(new AlgIdOptionDescriptor());
                }
            }

        protected override void Execute(TextWriter output, CryptographicContext context, IX509CertificateStorage store)
            {
            EncryptMessage(output, context,
                OutputFileName?.FirstOrDefault(),
                store.Certificates.ToArray());
            }

        #region M:EncryptMessage(TextWriter,CryptographicContext,String,IList<IX509Certificate>)
        private void EncryptMessage(TextWriter output, CryptographicContext context,String outputfilename, IList<IX509Certificate> recipients)
            {
            using (var inputfile = File.OpenRead(InputFileName[0])) {
                if (outputfilename != null) {
                    if (String.IsNullOrWhiteSpace(outputfilename)) { throw new ArgumentOutOfRangeException(nameof(outputfilename)); }
                    using (var outputfile = File.Create(outputfilename)) {
                        EncryptMessage(output, inputfile, outputfile, context, recipients);
                        GC.Collect();
                        }
                    }
                else
                    {
                    using (var outputfile = new MemoryStream()) {
                        EncryptMessage(output, inputfile, outputfile, context, recipients);
                        outputfile.Seek(0, SeekOrigin.Begin);
                        Utilities.Hex(outputfile.ToArray(), Console.Out);
                        GC.Collect();
                        }
                    }
                }
            }
        #endregion
        #region M:EncryptMessage(TextWriter,Stream,Stream,CryptographicContext,IList<X509Certificate>)
        private void EncryptMessage(TextWriter output, Stream inputfile, Stream outputfile, CryptographicContext context, IList<IX509Certificate> recipients)
            {
            if (MessageFlags.HasFlag(CryptographicMessageFlags.IndefiniteLength))
                {
                context.EncryptMessageBER(AlgId.Value, recipients, inputfile, outputfile);
                }
            else
                {
                context.EncryptMessageDER(AlgId.Value, recipients, inputfile, outputfile);    
                }
            
            /*
                 if (asn1 == "ber") { context.EncryptMessageBER(AlgId.Value, recipients, inputfile, outputfile); }
            else if (asn1 == "der") { context.EncryptMessageDER(AlgId.Value, recipients, inputfile, outputfile); }
            else if (asn1 == "der,block")
                {
                using (var writer = new BinaryWriter(outputfile)) {
                    var size = 0.0;
                    var start  = DateTime.Now;
                    Double velocity;
                    Double span;
                    var i = 0;
                    foreach (var block in context.EncryptMessageDER(AlgId.Value, recipients, FetchBlockFromStream(inputfile), NEncThreads))
                        {
                        size += block.Length;
                        span = (DateTime.Now - start).TotalMilliseconds;
                        if (span > 0) {
                            velocity = (size/span)*0.9765625;
                            //logger.LogTrace(
                            //    (velocity <= 1024)
                            //        ? String.Format(culture, "[{0:D8}]:Velocity:{1:F2} KBs", i, velocity)
                            //        : String.Format(culture, "[{0:D8}]:Velocity:{1:F2} MBs", i, velocity / 1024));
                            }
                        i++;
                        writer.Write(block.Length);
                        outputfile.Write(block, 0, block.Length);
                        }

                    span = (DateTime.Now - start).TotalMilliseconds;
                    //logger.LogTrace(String.Format("M:span:{0}", DateTime.Now - start));
                    if (span > 0) {
                        velocity = (size/span)*0.9765625;
                        //logger.LogTrace(
                        //    (velocity <= 1024)
                        //        ? String.Format(culture, "M:Velocity:{0:F2} KBs", velocity)
                        //        : String.Format(culture, "M:Velocity:{0:F2} MBs", velocity / 1024));
                        }
                    }
                }
            else if (asn1 == "ber,block")
                {
                using (var writer = new BinaryWriter(outputfile)) {
                    var size = 0.0;
                    var start  = DateTime.Now;
                    Double velocity;
                    Double span;
                    var i = 0;
                    foreach (var block in context.EncryptMessageBER(AlgId.Value, recipients, FetchBlockFromStream(inputfile), NEncThreads))
                        {
                        size += block.Length;
                        span = (DateTime.Now - start).TotalMilliseconds;
                        if (span > 0) {
                            velocity = (size/span)*0.9765625;
                            //logger.LogTrace(
                            //    (velocity <= 1024)
                            //        ? String.Format(culture, "[{0:D8}]:Velocity:{1:F2} KBs", i, velocity)
                            //        : String.Format(culture, "[{0:D8}]:Velocity:{1:F2} MBs", i, velocity / 1024));
                            }
                        i++;
                        writer.Write(block.Length);
                        outputfile.Write(block, 0, block.Length);
                        }

                    span = (DateTime.Now - start).TotalMilliseconds;
                    //logger.LogTrace(String.Format("M:span:{0}", DateTime.Now - start));
                    if (span > 0) {
                        velocity = (size/span)*0.9765625;
                        //logger.LogTrace(
                        //    (velocity <= 1024)
                        //        ? String.Format(culture, "M:Velocity:{0:F2} KBs", velocity)
                        //        : String.Format(culture, "M:Velocity:{0:F2} MBs", velocity / 1024));
                        }
                    }
                return;
                }
                */
            }
        #endregion
        #region M:FetchBlockFromStream(Stream):IEnumerable<Byte[]>
        private static IEnumerable<Byte[]> FetchBlockFromStream(Stream inputfile) {
            //Operations.Enqueue(new Message("SetProgressState", TaskbarProgressBarState.Normal));
            #if USE_WINDOWS_API_CODE_PACK
            Program.taskbar.SetProgressValue(0, 1, Program.WindowHandle);
            #endif
            var totallength = inputfile.Length;
            var block = new Byte[DEFAULT_BLOCK_SIZE];
            var currentlength = 0.0;
            var percent = 0.0;
            for (;;) {
                Thread.Yield();
                var sz = inputfile.Read(block, 0, block.Length);
                currentlength += sz;
                percent = (currentlength/totallength);
                //Debug.Print($"{percent}");
                //Operations.Enqueue(new Message("SetProgressValue", (Int32)(percent*100), 100));
                #if USE_WINDOWS_API_CODE_PACK
                Program.taskbar.SetProgressValue((Int32)(percent*100), 100, Program.WindowHandle);
                #endif
                //Program.mainform.BeginInvoke(new Action(()=>
                //    {
                    
                //    }));
                //Application.DoEvents();
                if (sz == 0) { break; }
                var r = new Byte[sz];
                Array.Copy(block, 0, r, 0, sz);
                yield return r;
                }
            }
        #endregion
        }
    }
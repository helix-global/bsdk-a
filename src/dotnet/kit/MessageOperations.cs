using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace Kit
    {
    public class MessageOperations
        {
        private static readonly Int32 DEFAULT_BLOCK_SIZE = (IntPtr.Size == 4)
            ?  5*1024*1024
            : 16*1024*1024;
        private static readonly Int32 NEncThreads = (IntPtr.Size == 4)
            ? 16
            : 16;

        #region M:EncryptMessage(Stream,Stream,CryptographicContext,IList<X509Certificate>,Oid)
        private static void EncryptMessage(Stream inputfile, Stream outputfile, SCryptographicContext context, IList<IX509Certificate> recipients, Oid algid, String asn1)
            {
                 if (asn1 == "ber") { context.EncryptMessageBER(algid, recipients, inputfile, outputfile); }
            else if (asn1 == "der") { context.EncryptMessageDER(algid, recipients, inputfile, outputfile); }
            else if (asn1 == "der,block")
                {
                using (var writer = new BinaryWriter(outputfile)) {
                    var size = 0.0;
                    var start  = DateTime.Now;
                    Double velocity;
                    Double span;
                    var i = 0;
                    foreach (var block in context.EncryptMessageDER(algid, recipients, FetchBlockFromStream(inputfile), NEncThreads))
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
                    foreach (var block in context.EncryptMessageBER(algid, recipients, FetchBlockFromStream(inputfile), NEncThreads))
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
            }
        #endregion
        #region M:FetchBlockFromStream(Stream):IEnumerable<Byte[]>
        private static IEnumerable<Byte[]> FetchBlockFromStream(Stream inputfile) {
            #if USE_WINDOWS_API_CODE_PACK
            Operations.Enqueue(new Message("SetProgressState", TaskbarProgressBarState.Normal));
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
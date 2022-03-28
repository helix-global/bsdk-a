using System;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.SmartCard;

namespace SmartCardSample
    {
    internal class Program
        {
        private static void Main(String[] args)
            {
            var L = new Win32SharedObject((IntPtr.Size == 4)? "clrx86.dll" : "clrx64.dll");
            var P = L.Get("DllGetClassObject");
            using (var context = new SCardContext(SCardScope.System)) {
                var index = 0;
                foreach (var reader in context.Readers)
                    {
                    Console.Write($"{index}:{reader.Name}:");
                    try
                        {
                        using (var rcontext = reader.Open(SCardShareMode.Shared)) {
                            Console.WriteLine(HRESULT.SCARD_S_SUCCESS);
                            rcontext.Transmit("00a4 0000 02 3f00", out var o);
                            rcontext.Transmit("00b0 0000",     out o);
                            }
                        }
                    catch (HResultException e)
                        {
                        Console.WriteLine((HRESULT)e.ErrorCode);
                        }
                    index++;
                    }
                }
            }
        }
    }

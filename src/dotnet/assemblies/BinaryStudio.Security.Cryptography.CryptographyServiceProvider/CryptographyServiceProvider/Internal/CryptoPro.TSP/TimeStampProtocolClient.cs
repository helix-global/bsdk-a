using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;

namespace CryptoPro.TSP
    {
    //#if CODE_ANALYSIS
    //[SuppressMessage("Design", "CA1060:Move pinvokes to native methods class", Justification = "<Pending>")]
    //#endif
    //public class TimeStampProtocolClient : CryptographicObject, ITimeStampProtocolClient
    //    {
    //    #region P:Handle:SafeHandle
    //    public override SafeHandle Handle { get {
    //        EnsureCore();
    //        return so;
    //        }}
    //    #endregion
    //    private String Address { get; }

    //    protected override ILogger Logger { get; }

    //    private SharedObject so;
    //    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    //    private unsafe delegate HRESULT DTSPSendRequest([MarshalAs(UnmanagedType.LPWStr)] String address, [MarshalAs(UnmanagedType.LPWStr)] String hashoid, [MarshalAs(UnmanagedType.SafeArray,SafeArraySubType = VarEnum.VT_UI1)] Byte[] hashval, Boolean responsecertificate, out IntPtr store, [Out][MarshalAs(UnmanagedType.SafeArray,SafeArraySubType = VarEnum.VT_UI1)] out Byte[] response);

    //    private SharedMethod<DTSPSendRequest> FTSPSendRequest;

    //    public TimeStampProtocolClient(String tspserver)
    //        {
    //        Address = tspserver;
    //        }

    //    #region M:Validate(HRESULT)
    //    private void Validate(HRESULT status) {
    //        if (status != HRESULT.S_OK) {
    //            Exception e;
    //            var i = (Int32)status;
    //            if ((i >= 0xFFFF) || (i < 0))
    //                {
    //                e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
    //                }
    //            else
    //                {
    //                e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
    //                }
    //            throw e;
    //            }
    //        }
    //    #endregion
    //    #region M:EnsureCore
    //    protected override void EnsureCore() {
    //        if (so == null) {
    //            using (TraceManager.Instance.Trace()) {
    //                var executingassembly = Assembly.GetExecutingAssembly();
    //                var location = executingassembly.Location;
    //                if (String.IsNullOrEmpty(location))
    //                    {
    //                    location = AppDomain.CurrentDomain.BaseDirectory;
    //                    }
    //                else
    //                    {
    //                    location = Path.GetDirectoryName(location);
    //                    }
    //                #if DEBUG
    //                so = new SharedObject(Path.Combine(location, (IntPtr.Size == 8)
    //                    ? "cptspcli64d.dll"
    //                    : "cptspcli32d.dll"));
    //                #else
    //                so = new SharedObject(Path.Combine(location, (IntPtr.Size == 8)
    //                    ? "cptsplci64.dll"
    //                    : "cptsplci32.dll"));
    //                #endif
    //                FTSPSendRequest = new SharedMethod<DTSPSendRequest>(so, "TSPSendRequest");
    //                }
    //            }
    //        }
    //    #endregion
    //    #region M:SendRequest(String,String,Byte[])
    //    public void SendRequest(String address, String hashoid, Byte[] hashval, out IX509CertificateStorage storage)
    //        {
    //        if (hashval == null) { throw new ArgumentNullException(nameof(hashval)); }
    //        if (hashval.Length == 0) { throw new ArgumentOutOfRangeException(nameof(hashval)); }
    //        if (address == null) { throw new ArgumentNullException(nameof(address)); }
    //        if (hashoid == null) { throw new ArgumentNullException(nameof(hashoid)); }
    //        if (String.IsNullOrWhiteSpace(address)) { throw new ArgumentOutOfRangeException(address); }
    //        if (String.IsNullOrWhiteSpace(hashoid)) { throw new ArgumentOutOfRangeException(hashoid); }
    //        Validate(TSPSendRequest(address, hashoid, hashval, true, out var store, out var response));
    //        storage = new X509CertificateStorage(store);
    //        }
    //    #endregion
    //    #region M:SendRequest(String,String,Byte[],[Out]Byte[])
    //    public void SendRequest(String address, String hashoid, Byte[] hashval, Boolean responsecertificate, out Byte[] response)
    //        {
    //        if (hashval == null) { throw new ArgumentNullException(nameof(hashval)); }
    //        if (hashval.Length == 0) { throw new ArgumentOutOfRangeException(nameof(hashval)); }
    //        if (address == null) { throw new ArgumentNullException(nameof(address)); }
    //        if (hashoid == null) { throw new ArgumentNullException(nameof(hashoid)); }
    //        if (String.IsNullOrWhiteSpace(address)) { throw new ArgumentOutOfRangeException(address); }
    //        if (String.IsNullOrWhiteSpace(hashoid)) { throw new ArgumentOutOfRangeException(hashoid); }
    //        Validate(TSPSendRequest(address, hashoid, hashval, responsecertificate, out var store, out response));
    //        CertCloseStore(store, 0);
    //        }
    //    #endregion

    //    [DllImport("crypt32.dll", SetLastError = true)] internal static extern Boolean CertCloseStore(IntPtr handle, UInt32 flags);
    //    #region M:TSPSendRequest(String,String,Byte[],Int32):HRESULT
    //    private HRESULT TSPSendRequest([MarshalAs(UnmanagedType.LPWStr)] String address, [MarshalAs(UnmanagedType.LPWStr)] String hashoid, [MarshalAs(UnmanagedType.SafeArray,SafeArraySubType = VarEnum.VT_UI1)] Byte[] hashval, Boolean responsecertificate, out IntPtr store,[Out][MarshalAs(UnmanagedType.SafeArray,SafeArraySubType = VarEnum.VT_UI1)] out Byte[] response)
    //        {
    //        using (TraceManager.Instance.Trace()) {
    //            EnsureCore();
    //            return ((DTSPSendRequest)FTSPSendRequest)(address, hashoid, hashval, responsecertificate, out store, out response);
    //            }
    //        }
    //    #endregion

    //    public void SendRequest(Oid hashoid, Byte[] hashval, out IX509CertificateStorage storage)
    //        {
    //        SendRequest(Address, hashoid.Value, hashval, out storage);
    //        }

    //    public void SendRequest(Oid hashoid, Byte[] hashval, Boolean responsecertificate, out Byte[] response)
    //        {
    //        SendRequest(Address, hashoid.Value, hashval, responsecertificate, out response);
    //        }
    //    }
    }
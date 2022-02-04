using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibraryA : Library, IFintechLibrary
        {
        private readonly ILogger logger;
        private Boolean Disposed;
        public FintechLibraryA(String filepath, Version version, ILogger logger)
            : base(filepath)
            {
            this.logger = logger;
            Version = version;
            EnsureProcedure("DllOpenCertificateStore", ref FDllOpenCertificateStore);
            EnsureProcedure("DllVerifyMRTDCertificate", ref FDllVerifyMRTDCertificate);
            }

        #region M:Make(Type,params Object[]):T
        private static Object Make(Type type, params Object[] args)
            {
            return Activator.CreateInstance(type, BindingFlags.CreateInstance, null, args, CultureInfo.CurrentCulture);
            }
        #endregion
        #region M:Make(String):Exception
        private static Exception Make(String message, IList<Exception> exceptions, String stacktrace, HRESULT? scode, Type basetype, String source) {
            SecurityException e = null;
            message = TrimDup(String.Join(" ", (message ?? String.Empty).Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())));
            if (scode != null) {
                Type type = null;
                switch(scode.Value) {
                    case HRESULT.FINTECH_E_CERT_MISSING:             type = typeof(CertificateMissingException); break;
                    case HRESULT.FINTECH_E_CERT_SIGNATURE:           type = typeof(CertificateRootAuthorityInvalidSignatureException); break;
                    case HRESULT.FINTECH_E_CERT_EXPIRED:             type = typeof(CertificateRootAuthorityExpiredException); break;
                    case HRESULT.CERT_E_CHAINING:                    type = typeof(CertificateMissingException); break;
                    case HRESULT.TRUST_E_CERT_SIGNATURE:             type = typeof(CertificateSignatureException); break;
                    case HRESULT.CRYPT_E_NO_REVOCATION_CHECK:        type = typeof(CertificateRevocationException); break;
                    case HRESULT.CRYPT_E_REVOCATION_OFFLINE:         type = typeof(CertificateRevocationOfflineException); break;
                    case HRESULT.CRYPT_E_REVOKED:                    type = typeof(CertificateIsRevokedException); break;
                    case HRESULT.CERT_E_EXPIRED:                     type = typeof(CertificateExpiredException); break;
                    case HRESULT.CRYPT_E_OID_FORMAT:                 type = typeof(InvalidObjectIdentifierException); break;
                    case HRESULT.FINTECH_E_CERT_WRONG_EKU:           type = typeof(CertificateInvalidExtendedKeyUsageException); break;
                    case HRESULT.FINTECH_E_CERT_STORE_NOT_FOUND:     type = typeof(CertificateStoreNotFoundException); break;
                    case HRESULT.FINTECH_E_CERT_PRIVATE_KEY_EXPIRED: type = typeof(CertificatePrivateKeyExpiredException); break;
                    case HRESULT.FINTECH_E_CERT_DECODE:              type = typeof(CertificateInvalidFormatException); break;
                    case HRESULT.CRYPT_E_MSG_ERROR:                  type = typeof(CmsOperationException); break;
                    case HRESULT.CRYPT_E_INVALID_MSG_TYPE:           type = typeof(CmsInvalidTypeException); break;
                    case HRESULT.NTE_BAD_SIGNATURE:                  type = typeof(CertificateInvalidSignatureException); break;
                    case HRESULT.NTE_BAD_HASH:                       type = typeof(CertificateInvalidHashException); break;
                    case HRESULT.NTE_BAD_DATA:
                    case HRESULT.CRYPT_E_ASN1_BADTAG:                type = typeof(SecurityInvalidDataException); break;
                    case HRESULT.NTE_BAD_KEYSET:
                    case HRESULT.NTE_NO_KEY:                         type = typeof(CertificatePrivateKeyMissingException); break;
                    case HRESULT.COR_E_ARGUMENTOUTOFRANGE:           basetype = typeof(ArgumentOutOfRangeException); break;
                    case HRESULT.COR_E_NOTSUPPORTED:                 basetype = typeof(NotSupportedException); break;
                    case HRESULT.COR_E_NULLREFERENCE:                basetype = basetype ?? typeof(NullReferenceException); break;
                    case HRESULT.FINTECH_E_CRL_MISSING:             type = typeof(CrlMissingException); break;
                    case HRESULT.FINTECH_E_CRL_SIGNATURE:           type = typeof(CrlSignatureException); break;
                    case HRESULT.FINTECH_E_CRL_EXPIRED:             type = typeof(CrlExpiredException); break;
                    }
                if (basetype != null)
                    {
                    return ((exceptions == null) || (exceptions.Count == 0))
                            ? (Exception)Make(basetype, message)
                            : (exceptions.Count == 1)
                                ? (Exception)Make(basetype, message, exceptions[0])
                                : (Exception)Make(basetype, message,
                                    new SecurityException(message, exceptions)
                                        {
                                        InternalStackTrace = stacktrace,
                                        InternalHResult = scode
                                        });
                    }
                if (type != null) {
                    if (String.IsNullOrWhiteSpace(message)) { message = HResultException.FormatMessage((Int32)scode); }
                    e = (exceptions != null)
                        ? (SecurityException)Make(type, message, exceptions)
                        : (SecurityException)Make(type, message);
                    }
                }
            if (e == null)
                {
                if ((scode != null) && String.IsNullOrWhiteSpace(message)) { message = HResultException.FormatMessage((Int32)scode); }
                e = (exceptions != null)
                    ? new SecurityException(message, exceptions)
                    : new SecurityException(message);
                }
            e.InternalHResult = scode;
            e.InternalStackTrace = String.Join(Environment.NewLine, stacktrace.Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries).Reverse());
            return e;
            }
        #endregion
        #region M:From(IErrorInfo,HRESULT):Exception
        private static Exception From(IErrorInfo errorinfo, HRESULT r)
            {
            return (errorinfo != null)
                ? From(errorinfo)
                : new Win32Exception(unchecked((Int32)r));
            }
        #endregion
        #region M:From(IErrorInfo):Exception
        private static Exception From(IErrorInfo errorinfo)
            {
            if (errorinfo == null) { throw new ArgumentNullException(nameof(errorinfo)); }
            var e = errorinfo as IException;
            return (e != null)
                ? From(e)
                : new SecurityException(errorinfo.GetDescription())
                    {
                    Source = errorinfo.GetSource()
                    };
            }
        #endregion
        #region M:From(IException):Exception
        private static Exception From(IException exception) {
            if (exception == null) { throw new ArgumentNullException(nameof(exception)); }
            if (exception is Exception r) { return r; }
            r = null;
            Type basetype = null;
            var stacktrace = String.Join("\n", exception.StackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray());
            var data = new Dictionary<Object,Object>();
            #region Заполнение словаря
            var values = exception.Data;
            var scode  = exception.SCODE;
            var source = (exception as IErrorInfo)?.GetSource();
            if (values != null)
                {
                var enumeratorD = values.GetEnumerator();
                var enumeratorM = (IEnumerator)enumeratorD;
                while (enumeratorM.MoveNext())
                    {
                    data.Add(
                        enumeratorD.Key,
                        enumeratorD.Value);
                    }
                }
            #endregion
            if (exception is IArgumentNullException)       { basetype = typeof(ArgumentNullException);       }
            if (exception is IArgumentOutOfRangeException) { basetype = typeof(ArgumentOutOfRangeException); }
            if (exception is IAggregateException aggr) {
                var exceptions = new List<Exception>();
                var innerExceptions = aggr.InnerExceptions;
                if (innerExceptions != null) {
                    var count = innerExceptions.Count;
                    for(var j = 0; j < count; j++) {
                        if (innerExceptions[j] is IException e)
                            {
                            exceptions.Add(From(e));
                            }
                        }
                    }
                var errorinfo = exception as IErrorInfo;
                r = (exceptions.Count == 1)
                    ? exceptions[0]
                    : Make(errorinfo?.GetDescription(), exceptions, stacktrace, scode, basetype, source);
                }
            else if (exception.InnerException != null)
                {
                var errorinfo = exception as IErrorInfo;
                r = Make(errorinfo?.GetDescription(),
                    new []{From(exception.InnerException) },
                    stacktrace, scode, basetype, source);
                }
            else
                {
                var errorinfo = exception as IErrorInfo;
                r = Make(errorinfo?.GetDescription(), null,
                    stacktrace, scode, basetype, source);
                }
            foreach (var i in data)
                {
                r.Add(i.Key.ToString(), i.Value);
                }
            if (!String.IsNullOrEmpty(source)) { r.Source = source; }
            return r;
            }
        #endregion
        #region M:Validate(HRESULT)
        private static void Validate(HRESULT r) {
            if (r != HRESULT.S_OK) {
                IErrorInfo i = null;
                GetErrorInfo(0, ref i);
                var e = From(i, r);
                if (i != null) {
                    Dispose(ref i);
                    }
                SetErrorInfo(0, i);
                throw e ?? Marshal.GetExceptionForHR((Int32)r);
                }
            }
        #endregion
        #region M:TrimDup(String)
        private static String TrimDup(String source) {
            return (source == null)
                ? source
                : String.Join(" ",
                    TrimDup(source.Split(new []{' ' },
                    StringSplitOptions.RemoveEmptyEntries)));
            }
        #endregion
        #region M:TrimDup(String[]):String[]
        private static String[] TrimDup(String[] source) {
            if ((source == null) || (source.Length < 2)) { return source; }
            var offset = source.Length - 2;
            while (offset >= 0) {
                var index = FindLast(source, offset);
                if (index == -2) { return source; }
                if (index == -1)
                    {
                    offset--;
                    continue;
                    }
                var target = new String[offset];
                Array.Copy(source, 0, target, 0, offset);
                source = target;
                offset = source.Length - 2;
                }
            return source;
            }
        #endregion
        #region M:FindLast(String[],Int32):Int32
        private static Int32 FindLast(String[] source, Int32 offset) {
            var count = source.Length - offset;
            var first = offset - count;
            if (first < 0) { return -2; }
            for (var i = 0; i < count; i++) {
                if (source[first + i] != source[offset + i]) {
                    return -1;
                    }
                }
            return first;
            }
        #endregion

        [DllImport("oleaut32.dll", PreserveSig = true)] private static extern HRESULT GetErrorInfo(Int32 i, [In][Out] ref IErrorInfo r);
        [DllImport("oleaut32.dll", PreserveSig = true)] private static extern HRESULT SetErrorInfo(Int32 i, [In] IErrorInfo r);
        [DllImport("kernel32.dll", BestFitMapping = false)] private static extern HRESULT GetLastError();

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("6e4e3eb4-3f68-4206-8e41-024fff324909")]
        private interface IException
            {
            String StackTrace { get; }
            HRESULT SCODE { get; }
            IException InnerException { get; }
            IDictionary Data { get; }
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("c991949b-e623-3f24-885c-bbb01ff43564")]
        private interface IArgumentNullException
            {
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("77da3028-bc45-3e82-bf76-2c123ee2c021")]
        private interface IArgumentOutOfRangeException
            {
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("69aa94cc-19ac-47e4-b623-90f58219bd8c")]
        private interface IAggregateException
            {
            IList InnerExceptions { get; }
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("06FB720C-11DE-4BD3-B761-AC27363A77F1")]
        internal interface ICertificate
            {
            Int32 EncodingType { get; }
            Byte[] Bytes { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            UInt32 Version { get; }
            Byte[] SerialNumber { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            String SignatureAlgorithm { get; }
            String Issuer { get; }
            String Subject { get; }
            DateTime NotBefore { get; }
            DateTime NotAfter { get; }
            String PublicKeyAlgorithmIdentifier { get; }
            Byte[] PublicKey { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            Byte[] PublicKeyBLOB { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            String Container { get; }
            X509KeyUsageFlags KeyUsage { get; }
            String HashAlgorithmIdentifier { get; }
            String SubjectAlternativeName { get; }
            String IssuerCommonName { get; }
            [PreserveSig] HRESULT Verify(String cn, String identifiers, UInt32 policy);
            [PreserveSig] HRESULT CreateSignature(KeySpec keyspecpolicy, IStream i, IStream o, UInt32 type, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][Out] out Byte[] digestvalue);
            [PreserveSig] HRESULT VerifySignature(IStream i, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] signature);
            [PreserveSig] HRESULT CreateEncryptor([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)][In] ICertificate[] certificates, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][Out] out Byte[] keyexchange, out ICryptoTransform r);
            [PreserveSig] HRESULT CreateDecryptor([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] keyexchange, out ICryptoTransform r);
            String SubjectCommonName { get; }
            [PreserveSig] HRESULT VerifyMessageSignature(IStream i);
            [PreserveSig] HRESULT CreateMessageSignature(KeySpec keyspecpolicy, IStream i, IStream o, [MarshalAs(UnmanagedType.Bool)] Boolean detached);
            Byte[] Thumbprint { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            String[] ExtendedKeyUsage { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }
            [PreserveSig] HRESULT VerifyExtendedKeyUsage(String i);
            KeySpec KeySpec { get; }
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("06FB720C-11DE-4BD3-B761-AC27363A77F0")]
        private interface ICertificateStore
            {
            [PreserveSig] HRESULT get_AvailableCertificates([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)][Out] out ICertificate[] certificates);
            [DispId(-4)] IEnumerator GetEnumerator();
            [PreserveSig] HRESULT FindCertificateBySerialNumber([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] i, out ICertificate certificate);
            [PreserveSig] HRESULT LoadCertificateFromBLOB([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] i, out ICertificate certificate);
            [PreserveSig] HRESULT LoadCertificateFromHandle([In] IntPtr Handle, out ICertificate certificate);
            [PreserveSig] HRESULT FindCertificateByThumbprint([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] i, out ICertificate certificate);
            [PreserveSig] HRESULT get_Certificates([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)][Out] out ICertificate[] certificates);
            }

        #if TRACE
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("73D2E14E-12CB-424A-A9D5-480C3BA132E1")]
        private interface IStatRow
	        {
            Boolean IsError { get; }
            String SignerSignatureAlgorithm { get; }
            String SignerDigestAlgorithm { get; }
            String Country { get; }
            String CertificateSignatureAlgorithm { get; }
            String CertificateDigestAlgorithm { get; }
            String Organization { get; }
            String Source { get; }
            String ActualDigestMethod { get; }
            Boolean SignatureInverse { get; }
	        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("8964EDA1-5327-4F5F-9932-91613A69C5CB")]
        private interface IStatTable
	        {
            Int32 Count { get; }
            IStatRow this[Int32 index] { get; }
	        }
        #endif

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllVerifyMRTDCertificate([In] ICertificate inputstream);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllOpenCertificateStore(StoreName protocol, StoreLocation location, out ICertificateStore r);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllVerifyMRTD([In] IStream inputstream);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllGetClassObject(ref Guid rclsid, ref Guid riid, [MarshalAs(UnmanagedType.LPWStr)] String parameters, out IntPtr r);

        public Version Version { get; }

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void IFintechLibrary.VerifyMrtdCertificate(IntPtr handle)
            {
            Validate(EnsureProcedure("DllOpenCertificateStore", ref FDllOpenCertificateStore)(StoreName.My,StoreLocation.CurrentUser, out var store));
            Validate(store.LoadCertificateFromHandle(handle, out var certificate));
            Validate(EnsureProcedure("DllVerifyMRTDCertificate", ref FDllVerifyMRTDCertificate)(certificate));
            }

        /// <summary>Verifies CMS using ICAO policy.</summary>
        /// <param name="stream">Input stream containing MRTD CMS.</param>
        void IFintechLibrary.VerifyMrtdMessage(Stream stream) {
            using (var inputstream = new ComStream(stream)) {
                Validate(EnsureProcedure("DllVerifyMRTD", ref FDllVerifyMRTD)(inputstream));
                }
            }

        private void GetClassObject(ref Guid rclsid, ref Guid riid, out IntPtr r)
            {
            Validate(EnsureProcedure("DllGetClassObject", ref FDllGetClassObject)(ref rclsid, ref riid, null, out r));
            }

        private void GetClassObject<T>(out T r)
            where T : class
            {
            var type = typeof(T);
            if (!type.IsImport) { throw new ArgumentOutOfRangeException(nameof(T)); }
            r = null;
            var rclsid = type.GUID;
            var riid = new Guid("00000000-0000-0000-C000-000000000046");
            GetClassObject(ref rclsid, ref riid, out var o);
            var i = Marshal.GetObjectForIUnknown(o);
            var c = Marshal.AddRef(o);
            while (c > 1)
                {
                c = Marshal.Release(o);
                }
            r =  i as T;
            }

        private static void Dispose<T>(T r)
            where T: class
            {
            if (r != null) {
                var i = r as IDisposable;
                if (i != null)
                    {
                    i.Dispose();
                    }
                else
                    {
                    var adapter = r as ICustomAdapter;
                    if (adapter != null)
                        {
                        Dispose(adapter.GetUnderlyingObject());
                        }
                    else
                        {
                        Release(r);
                        }
                    }
                }
            }

        private static void Dispose<T>(ref T r)
            where T: class
            {
            Dispose(r);
            r = null;
            }

        private static void Release(Object r)
            {
            if (r != null)
                {
                Marshal.ReleaseComObject(r);
                }
            }

        private static void Release<T>(ref T r)
            where T: class
            {
            Release(r);
            r = null;
            }

        private DDllVerifyMRTDCertificate FDllVerifyMRTDCertificate;
        private DDllOpenCertificateStore  FDllOpenCertificateStore;
        private DDllVerifyMRTD            FDllVerifyMRTD;
        private DDllGetClassObject        FDllGetClassObject;

        #if TRACE
        private class StatRecord
            {
            [DisplayName("Ошибка")] public Boolean IsError { get;set; }
            [DisplayName("Алгоритм подписи(Сообщение)")] public String SignerSignatureAlgorithm { get;set; }
            [DisplayName("Алгоритм хэширования(Сообщение)")] public String SignerDigestAlgorithm { get;set; }
            [DisplayName("Страна")] public String Country { get;set; }
            [DisplayName("Алгоритм подписи(Сертификат)")] public String CertificateSignatureAlgorithm { get;set; }
            [DisplayName("Алгоритм хэширования(Сертификат)")] public String CertificateDigestAlgorithm { get;set; }
            [DisplayName("Организация")] public String Organization { get;set; }
            [DisplayName("Источник ошибки")] public String Source { get;set; }
            [DisplayName("Используемый aлгоритм хэширования")] public String ActualDigestMethod { get;set; }
            [DisplayName("Инверсия сигнатуры")] public Boolean SignatureInverse { get;set; }
            }

        private static String FormatOID(String oid) {
            if (oid == null) { return "{none}"; }
            if (oid == szOID_NIST_sha224) { return "sha224"; }
            if (oid == szOID_ECDSA_SHA224) { return "sha224ECDSA"; }
            var r = new Oid(oid);
            var o = ((r.FriendlyName != oid) && !(String.IsNullOrWhiteSpace(r.FriendlyName)))
                ? $"{r.FriendlyName}"
                : $"{oid}";
            return o;
            }

        private void ReportStatistics() {
            if (logger.IsEnabled(LogLevel.Trace)) {
                GetClassObject<IStatTable>(out var table);
                if (table != null) {
                    var count = table.Count;
                    var rows = new List<StatRecord>();
                    var scces = 0;
                    var errrs = 0;
                    for (var i = 0; i < count; i++) {
                        var j = table[i];
                        rows.Add(new StatRecord
                            {
                            IsError = j.IsError,
                            Country = j.Country,
                            CertificateDigestAlgorithm = FormatOID(j.CertificateDigestAlgorithm),
                            CertificateSignatureAlgorithm = FormatOID(j.CertificateSignatureAlgorithm),
                            SignerDigestAlgorithm = FormatOID(j.SignerDigestAlgorithm),
                            SignerSignatureAlgorithm = FormatOID(j.SignerSignatureAlgorithm),
                            Organization = j.Organization,
                            Source = j.Source ?? "Нет",
                            ActualDigestMethod = FormatOID(j.ActualDigestMethod),
                            SignatureInverse = j.SignatureInverse
                            });
                        if (j.IsError)
                            {
                            errrs++;
                            }
                        else
                            {
                            scces++;
                            }
                        }
                    var filename = $"csecapi-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv";
                    using (var stream = File.OpenWrite(filename))
                    using (var target = new StreamWriter(stream, Encoding.UTF8)) {
                        var descriptors = TypeDescriptor.GetProperties(typeof(StatRecord)).OfType<PropertyDescriptor>().ToArray();
                        target.WriteLine($"{String.Join(";", descriptors.Select(i => i.DisplayName))}");
                        foreach (var row in rows) {
                            target.WriteLine($"{String.Join(";", descriptors.Select(i => i.GetValue(row)))}");
                            }
                        }
                    //var builder = new StringBuilder();
                    //builder.AppendLine("\n# STATISTICS:");
                    //builder.AppendFormat("  Total:{0}\n", rows.Count);
                    //builder.AppendFormat("  Success:{0}\n", scces);
                    //builder.AppendFormat("  Errors:{0}\n", errrs);
                    //builder.AppendLine("# DETAILS{SignerSignatureAlgorithm}:");
                    //foreach (var j in rows.GroupBy(i => i.SignerSignatureAlgorithm).OrderBy(i => i.Key))
                    //    {
                    //    builder.AppendFormat("  Errors:{{{1}}} Success:{{{2}}} # {{{0}}}\n",
                    //        j.Key,
                    //        j.Count(i =>  i.IsError).ToString("D4"),
                    //        j.Count(i => !i.IsError).ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{SignerDigestAlgorithm}:");
                    //foreach (var j in rows.GroupBy(i => i.SignerDigestAlgorithm).OrderBy(i => i.Key))
                    //    {
                    //    builder.AppendFormat("  Errors:{{{1}}} Success:{{{2}}} # {{{0}}}\n",
                    //        j.Key,
                    //        j.Count(i =>  i.IsError).ToString("D4"),
                    //        j.Count(i => !i.IsError).ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{CertificateSignatureAlgorithm}:");
                    //foreach (var j in rows.GroupBy(i => i.CertificateSignatureAlgorithm).OrderBy(i => i.Key))
                    //    {
                    //    builder.AppendFormat("  Errors:{{{1}}} Success:{{{2}}} # {{{0}}}\n",
                    //        j.Key,
                    //        j.Count(i =>  i.IsError).ToString("D4"),
                    //        j.Count(i => !i.IsError).ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{CertificateDigestAlgorithm}:");
                    //foreach (var j in rows.GroupBy(i => i.CertificateDigestAlgorithm).OrderBy(i => i.Key))
                    //    {
                    //    builder.AppendFormat("  Errors:{{{1}}} Success:{{{2}}} # {{{0}}}\n",
                    //        j.Key,
                    //        j.Count(i =>  i.IsError).ToString("D4"),
                    //        j.Count(i => !i.IsError).ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{Country,Organization}{Only Errors}:");
                    //foreach (var j in rows.Where(i=>i.IsError).GroupBy(i => Tuple.Create(i.Country, i.Organization)).OrderBy(i=> i.Key.Item1))
                    //    {
                    //    builder.AppendFormat("  {{{1}}} # {0}\n",
                    //        $"{j.Key.Item1},{j.Key.Item2}",
                    //        j.Count().ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{Source}{Only Errors}:");
                    //foreach (var j in rows.Where(i=>i.IsError).GroupBy(i => i.Source).OrderBy(i=> i.Key))
                    //    {
                    //    builder.AppendFormat("  {{{1}}} # {0}\n",
                    //        $"{j.Key}",
                    //        j.Count().ToString("D4"));
                    //    }
                    //builder.AppendLine("# DETAILS{Source-Country,Organization}{Only Errors}:");
                    //foreach (var j in rows.Where(i=>i.IsError).GroupBy(i => i.Source).OrderBy(i=> i.Key)) {
                    //    builder.AppendFormat("  {{{1}}} # {0}\n",
                    //        $"{j.Key}",
                    //        j.Count().ToString("D4"));
                    //    foreach (var K in j.GroupBy(i => Tuple.Create(i.Country, i.Organization)).OrderBy(i=> i.Key.Item1)) {
                    //        builder.AppendFormat("    {{{1}}} # {0}\n",
                    //            $"{K.Key.Item1},{K.Key.Item2}",
                    //            K.Count().ToString("D4"));
                    //        foreach (var L in K.GroupBy(i => Tuple.Create(i.SignerSignatureAlgorithm, i.CertificateSignatureAlgorithm)).OrderBy(i=> i.Key.Item1)) {
                    //        builder.AppendFormat("      {{{1}}} # {0}\n",
                    //            $"{L.Key.Item1},{L.Key.Item2}",
                    //            L.Count().ToString("D4"));
                    //            }
                    //        }
                    //    }
                    //logger.Log(LogLevel.Trace, builder.ToString());
                    Dispose(ref table);
                    }
                }
            }
        #endif

        protected override void Dispose(Boolean disposing) {
            if (!Disposed)
                {
                #if TRACE
                ReportStatistics();
                #endif
                base.Dispose(disposing);
                Disposed = true;
                }
            }

        private const String szOID_ECDSA_SHA224 = "1.2.840.10045.4.3.1";
        private const String szOID_NIST_sha224  = "2.16.840.1.101.3.4.2.4";
        }
    }
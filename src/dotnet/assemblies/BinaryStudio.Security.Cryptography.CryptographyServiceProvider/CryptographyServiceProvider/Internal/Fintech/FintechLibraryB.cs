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
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.DirectoryServices;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using Internal.CryptoAPICOM;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibraryB : Library, IFintechLibrary
        {
        private const UInt16 MRTD_GOST_PROVIDER = 1;
        private readonly ILogger logger;
        private Boolean Disposed;

        public FintechLibraryB(String filepath, Version version, ILogger logger)
            : base(filepath)
            {
            this.logger = logger;
            Version = version;
            EnsureProcedure("DllVerifyMRTD", ref FDllVerifyMRTD);
            }

        public Version Version { get; }

        private static Type GetExceptionType(HRESULT? scode) {
            if (scode == null) { return null; }
            switch (scode.Value) {
                case HRESULT.FINTECH_E_CERT_MISSING:             return typeof(CertificateMissingException);
                case HRESULT.FINTECH_E_CERT_SIGNATURE:           return typeof(CertificateRootAuthorityInvalidSignatureException);
                case HRESULT.FINTECH_E_CERT_EXPIRED:             return typeof(CertificateRootAuthorityExpiredException);
                case HRESULT.CERT_E_CHAINING:                    return typeof(CertificateMissingException);
                case HRESULT.TRUST_E_CERT_SIGNATURE:             return typeof(CertificateSignatureException);
                case HRESULT.CRYPT_E_NO_REVOCATION_CHECK:        return typeof(CertificateRevocationException);
                case HRESULT.CRYPT_E_REVOCATION_OFFLINE:         return typeof(CertificateRevocationOfflineException);
                case HRESULT.CRYPT_E_REVOKED:                    return typeof(CertificateIsRevokedException);
                case HRESULT.CERT_E_EXPIRED:                     return typeof(CertificateExpiredException);
                case HRESULT.CRYPT_E_OID_FORMAT:                 return typeof(InvalidObjectIdentifierException);
                case HRESULT.FINTECH_E_CERT_WRONG_EKU:           return typeof(CertificateInvalidExtendedKeyUsageException);
                case HRESULT.FINTECH_E_CERT_STORE_NOT_FOUND:     return typeof(CertificateStoreNotFoundException);
                case HRESULT.FINTECH_E_CERT_PRIVATE_KEY_EXPIRED: return typeof(CertificatePrivateKeyExpiredException);
                case HRESULT.FINTECH_E_CERT_DECODE:              return typeof(CertificateInvalidFormatException);
                case HRESULT.CRYPT_E_MSG_ERROR:                  return typeof(CmsOperationException);
                case HRESULT.CRYPT_E_INVALID_MSG_TYPE:           return typeof(CmsInvalidTypeException);
                case HRESULT.NTE_BAD_SIGNATURE:                  return typeof(CertificateInvalidSignatureException);
                case HRESULT.NTE_BAD_HASH:                       return typeof(CertificateInvalidHashException);
                case HRESULT.NTE_BAD_DATA:
                case HRESULT.CRYPT_E_ASN1_BADTAG:                return typeof(SecurityInvalidDataException);
                case HRESULT.NTE_BAD_KEYSET:
                case HRESULT.NTE_NO_KEY:                         return typeof(CertificatePrivateKeyMissingException);
                case HRESULT.FINTECH_E_CRL_MISSING:              return typeof(CrlMissingException);
                case HRESULT.FINTECH_E_CRL_SIGNATURE:            return typeof(CrlSignatureException);
                case HRESULT.FINTECH_E_CRL_EXPIRED:              return typeof(CrlExpiredException);
                }
            return null;
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
                    case HRESULT.FINTECH_E_CRL_MISSING:              type = typeof(CrlMissingException); break;
                    case HRESULT.FINTECH_E_CRL_SIGNATURE:            type = typeof(CrlSignatureException); break;
                    case HRESULT.FINTECH_E_CRL_EXPIRED:              type = typeof(CrlExpiredException); break;
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
            var e = errorinfo as IClrException;
            return (e != null)
                ? From(e)
                : new SecurityException(errorinfo.GetDescription())
                    {
                    Source = errorinfo.GetSource()
                    };
            }
        #endregion
        #region M:From(IClrException):Exception
        private static Exception From(IClrException exception) {
            if (exception == null) { throw new ArgumentNullException(nameof(exception)); }
            if (exception is Exception r) { return r; }
            r = null;
            Type basetype = null;
            var stacktrace = String.Join("\n", exception.StackTrace.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray());
            var data = new Dictionary<Object,Object>();
            #region Заполнение словаря
            var values = (exception as IException)?.Data;
            var scode  = (exception as IException)?.SCODE;
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
                Marshal.FinalReleaseComObject(values);
                Marshal.FinalReleaseComObject(enumeratorD);
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
                        if (innerExceptions[j] is IClrException e)
                            {
                            exceptions.Add(From(e));
                            }
                        }
                    Marshal.FinalReleaseComObject(innerExceptions);
                    }
                var errorinfo = exception as IErrorInfo;
                r = (exceptions.Count == 1)
                    ? exceptions[0]
                    : Make(errorinfo?.GetDescription(), exceptions, stacktrace, scode, basetype, source);
                if (errorinfo != null)
                    {
                    Marshal.FinalReleaseComObject(errorinfo);
                    }
                }
            else if (exception.InnerException != null)
                {
                r = Make(exception.Message, new []{
                    From(exception.InnerException)
                    }, stacktrace, scode, basetype, source);
                Marshal.FinalReleaseComObject(exception.InnerException);
                }
            else
                {
                r = Make(exception.Message, null,
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
                SetErrorInfo(0, IntPtr.Zero);
                if (i != null)
                    {
                    Marshal.FinalReleaseComObject(i);
                    }
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
        [DllImport("oleaut32.dll", PreserveSig = true)] private static extern HRESULT SetErrorInfo(Int32 i, [In] IntPtr r);
        [DllImport("kernel32.dll", BestFitMapping = false)] private static extern HRESULT GetLastError();

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void IFintechLibrary.VerifyMrtdCertificate(IntPtr handle)
            {
            Validate(EnsureProcedure("DllOpenCertificateStore", ref FDllOpenCertificateStore)(StoreName.My,StoreLocation.CurrentUser, out var store));
            Validate(store.LoadCertificateFromHandle(handle, out var certificate));
            Validate(EnsureProcedure("DllVerifyMRTDCertificate", ref FDllVerifyMRTDCertificate)(out var x, certificate, 0));
            }

        /// <summary>Verifies CMS using ICAO policy.</summary>
        /// <param name="stream">Input stream containing MRTD CMS.</param>
        void IFintechLibrary.VerifyMrtdMessage(Stream stream)
            {
            using (var inputstream = new ComStream(stream)) {
                var hr = (EnsureProcedure("DllVerifyMRTD", ref FDllVerifyMRTD)(inputstream, out var certificates, 0));
                if (certificates.Length > 0) {
                    foreach (var certificate in certificates) {
                        Marshal.FinalReleaseComObject(certificate);
                        }
                    }
                Validate(hr);
                }
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
            [PreserveSig] HRESULT Verify(String cn, String identifiers, UInt32 policy, [MarshalAs(UnmanagedType.Bool)] Boolean foreign);
            [PreserveSig] HRESULT CreateSignature(KeySpec keyspecpolicy, IStream i, IStream o, UInt32 type, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][Out] out Byte[] digestvalue);
            [PreserveSig] HRESULT VerifySignature(IStream i, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)][In] Byte[] signature);
            String SubjectCommonName { get; }
            [PreserveSig] HRESULT VerifyMessageSignature(IStream i);
            [PreserveSig] HRESULT CreateMessageSignature(KeySpec keyspecpolicy, IStream i, IStream o, [MarshalAs(UnmanagedType.Bool)] Boolean detached);
            Byte[] Thumbprint { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
            String[] ExtendedKeyUsage { [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }
            [PreserveSig] HRESULT VerifyExtendedKeyUsage(String i);
            KeySpec KeySpec { get; }
            IntPtr Context { get; }
            String TokenId { get; }
            [PreserveSig] HRESULT VerifyPrivateKeyUsagePeriod();
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

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("6e4e3eb4-3f68-4206-8e41-024fff324909")]
        private interface IException
            {
            String StackTrace { get; }
            HRESULT SCODE { get; }
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

        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        [ComImport,Guid("b36b5c63-42ef-38bc-a07e-0b34c98f164a")]
        internal interface IClrException
            {
            String ToString();
            Boolean Equals(Object obj);
            Int32 GetHashCode();
            Type GetType();
            String Message { get; }
            IClrException GetBaseException();
            String StackTrace { get; }
            String HelpLink { get; set; }
            String Source { get; set; }
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            void GetObjectData(SerializationInfo info, StreamingContext context);
            IClrException InnerException { get; }
            _MethodBase TargetSite { get; }
            }

        [StructLayout(LayoutKind.Sequential,Pack = 1)]
        private struct DateTimeRef
            {
            [MarshalAs(UnmanagedType.Bool)] public Boolean HasValue;
            public DateTime Value;
            }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("73D2E14E-12CB-424A-A9D5-480C3BA132E1")]
        private interface IStatRow
	        {
            UInt32 SCode { get; }
            String Organization { get; }
            String Source { get; }
            String ActualDigestMethod { get; }
            String Modifiers { get; }
            String CCryptError { get; }
            String BCryptError { get; }
            String ActualContentDigestMethod { get; }
            String Stream { get; }
            String CertificateThumbprint { get; }
            String CrlThumbprint { get; }
            Int32 ContentSize { get; }
            Int32 MessageSize { get; }
            Int32 Flags { get; }
	        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport, Guid("8964EDA1-5327-4F5F-9932-91613A69C5CB")]
        private interface IStatTable
	        {
            Int32 Count { get; }
            IStatRow this[Int32 index] { get; }
	        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllVerifyMRTDCertificate([Out] out IClrException x, [In] ICertificate certificate, UInt16 flags);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllOpenCertificateStore(StoreName protocol, StoreLocation location, out ICertificateStore r);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllVerifyMRTD([In] IStream inputstream, [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)] out Array certificates, UInt16 flags);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllGetClassObject(ref Guid rclsid, ref Guid riid, out IntPtr r);

        private DDllVerifyMRTDCertificate FDllVerifyMRTDCertificate;
        private DDllOpenCertificateStore  FDllOpenCertificateStore;
        private DDllVerifyMRTD            FDllVerifyMRTD;
        private DDllGetClassObject        FDllGetClassObject;

        private void GetClassObject(ref Guid rclsid, ref Guid riid, out IntPtr r)
            {
            Validate(EnsureProcedure("DllGetClassObject", ref FDllGetClassObject)(ref rclsid, ref riid, out r));
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

        private static void Release(Object r, Int32 rc = -1) {
            if (r != null) {
                if (rc >= 0) {
                    var o = Marshal.GetIUnknownForObject(r);
                    if (o != IntPtr.Zero) {
                        while (Marshal.Release(o) > rc)
                            {
                            }
                        }
                    }
                //Marshal.ReleaseComObject(r);
                }
            }

        private static void Release<T>(ref T r, Int32 rc)
            where T: class
            {
            Release(r, rc);
            r = null;
            }

        public static readonly IDictionary<String, Country> Countries = new SortedDictionary<String, Country> {
            {"af", new Country("Афганистан","af","afg",4)},
            {"al", new Country("Албания","al","alb",8)},
            {"aq", new Country("Антарктика","aq","ata",10)},
            {"dz", new Country("Алжир","dz","dza",12)},
            {"as", new Country("Американское Самоа","as","asm",16)},
            {"ad", new Country("Андорра","ad","and",20)},
            {"ao", new Country("Ангола","ao","ago",24)},
            {"ag", new Country("Антигуа и Барбуда","ag","atg",28)},
            {"az", new Country("Азербайджан","az","aze",31)},
            {"ar", new Country("Аргентина","ar","arg",32)},
            {"au", new Country("Австралия","au","aus",36)},
            {"at", new Country("Австрия","at","aut",40)},
            {"bs", new Country("Багамские Острова","bs","bhs",44)},
            {"bh", new Country("Бахрейн","bh","bhr",48)},
            {"bd", new Country("Бангладеш","bd","bgd",50)},
            {"am", new Country("Армения","am","arm",51)},
            {"bb", new Country("Барбадос","bb","brb",52)},
            {"be", new Country("Бельгия","be","bel",56)},
            {"bm", new Country("Бермуды","bm","bmu",60)},
            {"bt", new Country("Бутан","bt","btn",64)},
            {"bo", new Country("Боливия","bo","bol",68)},
            {"ba", new Country("Босния и Герцеговина","ba","bih",70)},
            {"bw", new Country("Ботсвана","bw","bwa",72)},
            {"bv", new Country("Остров Буве","bv","bvt",74)},
            {"br", new Country("Бразилия","br","bra",76)},
            {"bz", new Country("Белиз","bz","blz",84)},
            {"io", new Country("Британская территория в Индийском океане","io","iot",86)},
            {"sb", new Country("Соломоновы Острова","sb","slb",90)},
            {"vg", new Country("Виргинские Острова (Великобритания)","vg","vgb",92)},
            {"bn", new Country("Бруней","bn","brn",96)},
            {"bg", new Country("Болгария","bg","bgr",100)},
            {"mm", new Country("Мьянма","mm","mmr",104)},
            {"bi", new Country("Бурунди","bi","bdi",108)},
            {"by", new Country("Белоруссия","by","blr",112)},
            {"kh", new Country("Камбоджа","kh","khm",116)},
            {"cm", new Country("Камерун","cm","cmr",120)},
            {"ca", new Country("Канада","ca","can",124)},
            {"cv", new Country("Кабо-Верде","cv","cpv",132)},
            {"ky", new Country("Острова Кайман","ky","cym",136)},
            {"cf", new Country("ЦАР (Центральноафриканская Республика)","cf","caf",140)},
            {"car", new Country("ЦАР (Центральноафриканская Республика)","cf","caf",140)},
            {"lk", new Country("Шри-Ланка","lk","lka",144)},
            {"td", new Country("Чад","td","tcd",148)},
            {"cl", new Country("Чили","cl","chl",152)},
            {"cn", new Country("Китай (Китайская Народная Республика)","cn","chn",156)},
            {"tw", new Country("Китайская Республика (Тайвань)","tw","twn",158)},
            {"cx", new Country("Остров Рождества","cx","cxr",162)},
            {"cc", new Country("Кокосовые острова","cc","cck",166)},
            {"co", new Country("Колумбия","co","col",170)},
            {"km", new Country("Коморы","km","com",174)},
            {"yt", new Country("Майотта","yt","myt",175)},
            {"cg", new Country("Республика Конго","cg","cog",178)},
            {"cog", new Country("Республика Конго","cg","cog",178)},
            {"cd", new Country("Демократическая Республика Конго","cd","cod",180)},
            {"ck", new Country("Острова Кука","ck","cok",184)},
            {"cr", new Country("Коста-Рика","cr","cri",188)},
            {"hr", new Country("Хорватия","hr","hrv",191)},
            {"cu", new Country("Куба","cu","cub",192)},
            {"cy", new Country("Кипр","cy","cyp",196)},
            {"cz", new Country("Чехия","cz","cze",203)},
            {"bj", new Country("Бенин","bj","ben",204)},
            {"dk", new Country("Дания","dk","dnk",208)},
            {"dm", new Country("Доминика","dm","dma",212)},
            {"do", new Country("Доминиканская Республика","do","dom",214)},
            {"ec", new Country("Эквадор","ec","ecu",218)},
            {"sv", new Country("Сальвадор","sv","slv",222)},
            {"gq", new Country("Экваториальная Гвинея","gq","gnq",226)},
            {"et", new Country("Эфиопия","et","eth",231)},
            {"er", new Country("Эритрея","er","eri",232)},
            {"ee", new Country("Эстония","ee","est",233)},
            {"fo", new Country("Фарерские острова","fo","fro",234)},
            {"fk", new Country("Фолклендские острова","fk","flk",238)},
            {"gs", new Country("Южная Георгия и Южные Сандвичевы Острова","gs","sgs",239)},
            {"fj", new Country("Фиджи","fj","fji",242)},
            {"fi", new Country("Финляндия","fi","fin",246)},
            {"ax", new Country("Аландские острова","ax","ala",248)},
            {"fr", new Country("Франция","fr","fra",250)},
            {"gf", new Country("Гвиана","gf","guf",254)},
            {"pf", new Country("Французская Полинезия","pf","pyf",258)},
            {"tf", new Country("Французские Южные и Антарктические территории","tf","atf",260)},
            {"dj", new Country("Джибути","dj","dji",262)},
            {"ga", new Country("Габон","ga","gab",266)},
            {"ge", new Country("Грузия","ge","geo",268)},
            {"gm", new Country("Гамбия","gm","gmb",270)},
            {"ps", new Country("Государство Палестина","ps","pse",275)},
            {"de", new Country("Германия","de","deu",276)},
            {"gh", new Country("Гана","gh","gha",288)},
            {"gi", new Country("Гибралтар","gi","gib",292)},
            {"ki", new Country("Кирибати","ki","kir",296)},
            {"gr", new Country("Греция","gr","grc",300)},
            {"gl", new Country("Гренландия","gl","grl",304)},
            {"gd", new Country("Гренада","gd","grd",308)},
            {"gp", new Country("Гваделупа","gp","glp",312)},
            {"gu", new Country("Гуам","gu","gum",316)},
            {"gt", new Country("Гватемала","gt","gtm",320)},
            {"gn", new Country("Гвинея","gn","gin",324)},
            {"gy", new Country("Гайана","gy","guy",328)},
            {"ht", new Country("Гаити","ht","hti",332)},
            {"hm", new Country("Херд и Макдональд","hm","hmd",334)},
            {"va", new Country("Ватикан","va","vat",336)},
            {"hn", new Country("Гондурас","hn","hnd",340)},
            {"hk", new Country("Гонконг","hk","hkg",344)},
            {"hu", new Country("Венгрия","hu","hun",348)},
            {"is", new Country("Исландия","is","isl",352)},
            {"in", new Country("Индия","in","ind",356)},
            {"id", new Country("Индонезия","id","idn",360)},
            {"ir", new Country("Иран","ir","irn",364)},
            {"iq", new Country("Ирак","iq","irq",368)},
            {"ie", new Country("Ирландия","ie","irl",372)},
            {"il", new Country("Израиль","il","isr",376)},
            {"it", new Country("Италия","it","ita",380)},
            {"ci", new Country("Кот-д’Ивуар","ci","civ",384)},
            {"jm", new Country("Ямайка","jm","jam",388)},
            {"jp", new Country("Япония","jp","jpn",392)},
            {"kz", new Country("Казахстан","kz","kaz",398)},
            {"jo", new Country("Иордания","jo","jor",400)},
            {"ke", new Country("Кения","ke","ken",404)},
            {"kp", new Country("КНДР (Корейская Народно-Демократическая Республика)","kp","prk",408)},
            {"kr", new Country("Республика Корея","kr","kor",410)},
            {"kw", new Country("Кувейт","kw","kwt",414)},
            {"kg", new Country("Киргизия","kg","kgz",417)},
            {"la", new Country("Лаос","la","lao",418)},
            {"lb", new Country("Ливан","lb","lbn",422)},
            {"ls", new Country("Лесото","ls","lso",426)},
            {"lv", new Country("Латвия","lv","lva",428)},
            {"lr", new Country("Либерия","lr","lbr",430)},
            {"ly", new Country("Ливия","ly","lby",434)},
            {"li", new Country("Лихтенштейн","li","lie",438)},
            {"lt", new Country("Литва","lt","ltu",440)},
            {"lu", new Country("Люксембург","lu","lux",442)},
            {"mo", new Country("Макао","mo","mac",446)},
            {"mg", new Country("Мадагаскар","mg","mdg",450)},
            {"mw", new Country("Малави","mw","mwi",454)},
            {"my", new Country("Малайзия","my","mys",458)},
            {"mv", new Country("Мальдивы","mv","mdv",462)},
            {"ml", new Country("Мали","ml","mli",466)},
            {"mt", new Country("Мальта","mt","mlt",470)},
            {"mq", new Country("Мартиника","mq","mtq",474)},
            {"mr", new Country("Мавритания","mr","mrt",478)},
            {"mu", new Country("Маврикий","mu","mus",480)},
            {"mx", new Country("Мексика","mx","mex",484)},
            {"mc", new Country("Монако","mc","mco",492)},
            {"mn", new Country("Монголия","mn","mng",496)},
            {"md", new Country("Молдавия","md","mda",498)},
            {"me", new Country("Черногория","me","mne",499)},
            {"ms", new Country("Монтсеррат","ms","msr",500)},
            {"ma", new Country("Марокко","ma","mar",504)},
            {"mz", new Country("Мозамбик","mz","moz",508)},
            {"om", new Country("Оман","om","omn",512)},
            {"na", new Country("Намибия","na","nam",516)},
            {"nr", new Country("Науру","nr","nru",520)},
            {"np", new Country("Непал","np","npl",524)},
            {"nl", new Country("Нидерланды","nl","nld",528)},
            {"cw", new Country("Кюрасао","cw","cuw",531)},
            {"aw", new Country("Аруба","aw","abw",533)},
            {"sx", new Country("Синт-Мартен","sx","sxm",534)},
            {"bq", new Country("Бонайре, Синт-Эстатиус и Саба","bq","bes",535)},
            {"nc", new Country("Новая Каледония","nc","ncl",540)},
            {"vu", new Country("Вануату","vu","vut",548)},
            {"nz", new Country("Новая Зеландия","nz","nzl",554)},
            {"ni", new Country("Никарагуа","ni","nic",558)},
            {"ne", new Country("Нигер","ne","ner",562)},
            {"ng", new Country("Нигерия","ng","nga",566)},
            {"nu", new Country("Ниуэ","nu","niu",570)},
            {"nf", new Country("Остров Норфолк","nf","nfk",574)},
            {"no", new Country("Норвегия","no","nor",578)},
            {"mp", new Country("Северные Марианские Острова","mp","mnp",580)},
            {"um", new Country("Внешние малые острова США","um","umi",581)},
            {"fm", new Country("Микронезия","fm","fsm",583)},
            {"mh", new Country("Маршалловы Острова","mh","mhl",584)},
            {"pw", new Country("Палау","pw","plw",585)},
            {"pk", new Country("Пакистан","pk","pak",586)},
            {"pa", new Country("Панама","pa","pan",591)},
            {"pg", new Country("Папуа—Новая Гвинея","pg","png",598)},
            {"py", new Country("Парагвай","py","pry",600)},
            {"pe", new Country("Перу","pe","per",604)},
            {"ph", new Country("Филиппины","ph","phl",608)},
            {"rsm", new Country("Филиппины","ph","phl",608)},
            {"pn", new Country("Острова Питкэрн","pn","pcn",612)},
            {"pl", new Country("Польша","pl","pol",616)},
            {"pt", new Country("Португалия","pt","prt",620)},
            {"gw", new Country("Гвинея-Бисау","gw","gnb",624)},
            {"tl", new Country("Восточный Тимор","tl","tls",626)},
            {"pr", new Country("Пуэрто-Рико","pr","pri",630)},
            {"qa", new Country("Катар","qa","qat",634)},
            {"re", new Country("Реюньон","re","reu",638)},
            {"ro", new Country("Румыния","ro","rou",642)},
            {"ru", new Country("Россия","ru","rus",643)},
            {"rw", new Country("Руанда","rw","rwa",646)},
            {"bl", new Country("Сен-Бартелеми","bl","blm",652)},
            {"sh", new Country("Острова Святой Елены, Вознесения и Тристан-да-Кунья","sh","shn",654)},
            {"kn", new Country("Сент-Китс и Невис","kn","kna",659)},
            {"ai", new Country("Ангилья","ai","aia",660)},
            {"lc", new Country("Сент-Люсия","lc","lca",662)},
            {"mf", new Country("Сен-Мартен","mf","maf",663)},
            {"pm", new Country("Сен-Пьер и Микелон","pm","spm",666)},
            {"vc", new Country("Сент-Винсент и Гренадины","vc","vct",670)},
            {"sm", new Country("Сан-Марино","sm","smr",674)},
            {"st", new Country("Сан-Томе и Принсипи","st","stp",678)},
            {"sa", new Country("Саудовская Аравия","sa","sau",682)},
            {"sn", new Country("Сенегал","sn","sen",686)},
            {"rs", new Country("Сербия","rs","srb",688)},
            {"sc", new Country("Сейшельские Острова","sc","syc",690)},
            {"sl", new Country("Сьерра-Леоне","sl","sle",694)},
            {"sg", new Country("Сингапур","sg","sgp",702)},
            {"sk", new Country("Словакия","sk","svk",703)},
            {"vn", new Country("Вьетнам","vn","vnm",704)},
            {"si", new Country("Словения","si","svn",705)},
            {"so", new Country("Сомали","so","som",706)},
            {"za", new Country("ЮАР","za","zaf",710)},
            {"rss", new Country("ЮАР","za","zaf",710)},
            {"zw", new Country("Зимбабве","zw","zwe",716)},
            {"es", new Country("Испания","es","esp",724)},
            {"ss", new Country("Южный Судан","ss","ssd",728)},
            {"sd", new Country("Судан","sd","sdn",729)},
            {"eh", new Country("САДР (Сахарская Арабская Демократическая Республика)","eh","esh",732)},
            {"sr", new Country("Суринам","sr","sur",740)},
            {"sj", new Country("Шпицберген и Ян-Майен","sj","sjm",744)},
            {"sz", new Country("Эсватини","sz","swz",748)},
            {"se", new Country("Швеция","se","swe",752)},
            {"ch", new Country("Швейцария","ch","che",756)},
            {"sy", new Country("Сирия","sy","syr",760)},
            {"tj", new Country("Таджикистан","tj","tjk",762)},
            {"th", new Country("Таиланд","th","tha",764)},
            {"tg", new Country("Того","tg","tgo",768)},
            {"tk", new Country("Токелау","tk","tkl",772)},
            {"to", new Country("Тонга","to","ton",776)},
            {"tt", new Country("Тринидад и Тобаго","tt","tto",780)},
            {"ae", new Country("ОАЭ (Объединённые Арабские Эмираты)","ae","are",784)},
            {"tn", new Country("Тунис","tn","tun",788)},
            {"tr", new Country("Турция","tr","tur",792)},
            {"tm", new Country("Туркмения","tm","tkm",795)},
            {"tc", new Country("Теркс и Кайкос","tc","tca",796)},
            {"tv", new Country("Тувалу","tv","tuv",798)},
            {"ug", new Country("Уганда","ug","uga",800)},
            {"ua", new Country("Украина","ua","ukr",804)},
            {"mk", new Country("Северная Македония","mk","mkd",807)},
            {"eg", new Country("Египет","eg","egy",818)},
            {"gb", new Country("Великобритания","gb","gbr",826)},
            {"gg", new Country("Гернси","gg","ggy",831)},
            {"je", new Country("Джерси","je","jey",832)},
            {"im", new Country("Остров Мэн","im","imn",833)},
            {"tz", new Country("Танзания","tz","tza",834)},
            {"us", new Country("США","us","usa",840)},
            {"vi", new Country("Виргинские Острова (США)","vi","vir",850)},
            {"bf", new Country("Буркина-Фасо","bf","bfa",854)},
            {"uy", new Country("Уругвай","uy","ury",858)},
            {"uz", new Country("Узбекистан","uz","uzb",860)},
            {"ve", new Country("Венесуэла","ve","ven",862)},
            {"wf", new Country("Уоллис и Футуна","wf","wlf",876)},
            {"ws", new Country("Самоа","ws","wsm",882)},
            {"ye", new Country("Йемен","ye","yem",887)},
            {"zm", new Country("Замбия","zm","zmb",894)},
            {"ks", new Country("Республика Косово","ks","kos",383)},
            {"un", new Country("ООН (Организация Объединенных Наций)","un","un",-1)},
            {"zz", new Country("ООН (Организация Объединенных Наций)","zz","un",-1)},
            };
        private class StatRecord
            {
            public String Organization { get;set; }
            public String Source { get;set; }
            public String ActualDigestMethod { get;set; }
            public String CCryptError { get;set; }
            public String BCryptError { get;set; }
            public String Modifiers { get;set; }
            public String Stream { get;set; }
            public String ActualContentDigestMethod { get;set; }
            public Int32 MessageSize { get;set; }
            public Int32 ContentSize { get;set; }
            public Int32 Flags { get;set; }
            public String Certificate { get;set; }
            public String Crl { get;set; }
            public String SCode { get;set; }
            }

        private static String ShortKey(String value) {
            if (String.IsNullOrWhiteSpace(value)) { return null; }
            if (value.Length > 4) {
                return value.Substring(0,2)+value.Substring(value.Length - 3, 2);
                }
            return value;
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
                try
                    {
                    GetClassObject<IStatTable>(out var table);
                    if (table != null) {
                        var count = table.Count;
                        var rows = new List<StatRecord>();
                        for (var i = 0; i < count; i++) {
                            var j = table[i];
                            rows.Add(new StatRecord
                                {
                                SCode = j.SCode.ToString("x8"),
                                Organization = j.Organization,
                                Source = j.Source ?? "NULL",
                                ActualDigestMethod = j.ActualDigestMethod ?? "NULL",
                                Modifiers = j.Modifiers,
                                CCryptError = j.CCryptError,
                                BCryptError = j.BCryptError,
                                Stream = !String.IsNullOrWhiteSpace(j.Stream) ? Path.GetFileNameWithoutExtension(j.Stream) : "NULL",
                                ActualContentDigestMethod = j.ActualContentDigestMethod ?? "NULL",
                                ContentSize = j.ContentSize,
                                MessageSize = j.MessageSize,
                                Certificate = j.CertificateThumbprint ?? "NULL",
                                Crl = j.CrlThumbprint ?? "NULL",
                                Flags = j.Flags
                                });
                            Marshal.FinalReleaseComObject(j);
                            }
                        var filename = $"csecapi-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.csv";
                        using (var stream = File.OpenWrite(filename))
                        using (var target = new StreamWriter(stream, Encoding.Unicode)) {
                            var descriptors = TypeDescriptor.GetProperties(typeof(StatRecord)).OfType<PropertyDescriptor>().ToArray();
                            target.WriteLine($"{String.Join(";", descriptors.Select(i => i.DisplayName))}");
                            foreach (var row in rows.OrderBy(i => i.Organization)) {
                                target.WriteLine($"{String.Join(";", descriptors.Select(i => i.GetValue(row)))}");
                                }
                            }
                        Marshal.FinalReleaseComObject(table);
                        }
                    }
                catch (Exception e)
                    {
                    Debug.WriteLine(Exceptions.ToString(e));
                    }
                }
            }

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
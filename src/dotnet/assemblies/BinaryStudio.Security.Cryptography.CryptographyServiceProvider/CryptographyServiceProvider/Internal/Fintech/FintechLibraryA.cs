using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibraryA : Library, IFintechLibrary
        {
        public FintechLibraryA(String filepath)
            : base(filepath)
            {
            EnsureProcedure("DllOpenCertificateStore", ref FDllOpenCertificateStore);
            EnsureProcedure("DllVerifyMRTDCertificate", ref FDllVerifyMRTDCertificate);
            }

        #region M:CreateInstance(Type,params Object[]):T
        private static Object CreateInstance(Type type, params Object[] args)
            {
            return Activator.CreateInstance(type, BindingFlags.CreateInstance, null, args, CultureInfo.CurrentCulture);
            }
        #endregion
        #region M:Make(String):Exception
        private static Exception Make(String message, IList<Exception> exceptions, String stacktrace, HRESULT? scode, Type basetype, String source) {
            SecurityException e = null;
            message = String.Join(" ", (message ?? String.Empty).Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()));
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
                    case HRESULT.CRYPT_E_ASN1_BADTAG:
                        type = typeof(InvalidDataException); break;
                    case HRESULT.NTE_BAD_KEYSET:
                    case HRESULT.NTE_NO_KEY:
                        type = typeof(CertificatePrivateKeyMissingException); break;
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
                            ? (Exception)CreateInstance(basetype, message)
                            : (exceptions.Count == 1)
                                ? (Exception)CreateInstance(basetype, message, exceptions[0])
                                : (Exception)CreateInstance(basetype, message,
                                    new SecurityException(message, exceptions)
                                        {
                                        InternalStackTrace = stacktrace,
                                        InternalHResult = scode
                                        });
                    }
                if (type != null)
                    {
                    e = (exceptions != null)
                        ? (SecurityException)CreateInstance(type, message, exceptions)
                        : (SecurityException)CreateInstance(type, message);
                    }
                }
            if (e == null)
                {
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
                r.Data.Add(i.Key, i.Value);
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
                throw e ?? Marshal.GetExceptionForHR((Int32)r);
                }
            }
        #endregion
        #region M:Validate(HRESULT,[Out]Exception):Boolean
        private static Boolean Validate(HRESULT r, out Exception e) {
            e = null;
            if (r != HRESULT.S_OK) {
                IErrorInfo i = null;
                GetErrorInfo(0, ref i);
                e = From(i, r) ?? Marshal.GetExceptionForHR((Int32)r);
                return false;
                }
            return true;
            }
        #endregion

        [DllImport("oleaut32.dll", PreserveSig = false)] public static extern void GetErrorInfo(Int32 i, [In][Out] ref IErrorInfo r);
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

        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllVerifyMRTDCertificate([In] ICertificate inputstream);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate HRESULT DDllOpenCertificateStore(StoreName protocol, StoreLocation location, out ICertificateStore r);

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void IFintechLibrary.VerifyMrtdCertificate(IntPtr handle)
            {
            Validate(EnsureProcedure("DllOpenCertificateStore", ref FDllOpenCertificateStore)(StoreName.My,StoreLocation.CurrentUser, out var store));
            Validate(store.LoadCertificateFromHandle(handle, out var certificate));
            Validate(EnsureProcedure("DllVerifyMRTDCertificate", ref FDllVerifyMRTDCertificate)(certificate));
            }

        private DDllVerifyMRTDCertificate FDllVerifyMRTDCertificate;
        private DDllOpenCertificateStore  FDllOpenCertificateStore;
        }
    }
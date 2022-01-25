using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        private void Validate(HRESULT hr)
            {
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
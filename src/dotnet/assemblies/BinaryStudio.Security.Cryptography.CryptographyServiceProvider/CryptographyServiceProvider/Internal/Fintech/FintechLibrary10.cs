using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibrary10 : Library
        {
        public FintechLibrary10(String filepath)
            : base(filepath)
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
        }
    }
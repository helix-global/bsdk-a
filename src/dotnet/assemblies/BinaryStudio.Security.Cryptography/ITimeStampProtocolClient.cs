using System;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography
    {
    public interface ITimeStampProtocolClient
        {
        void SendRequest(Oid hashoid, Byte[] hashval, out IX509CertificateStorage storage);
        void SendRequest(Oid hashoid, Byte[] hashval, Boolean responsecertificate, out Byte[] response);
        }
    }
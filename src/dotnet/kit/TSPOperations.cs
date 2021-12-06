using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CryptoPro.TSP;
using Internal.CryptoAPICOM;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;

//using CryptoPro.TSP;

namespace Kit
    {
    public class TSPOperations
        {
        public static void MakeRequest()
            {
            //using (var client = new TimeStampProtocolClient(@"http://testca2012.cryptopro.ru/tsp/tsp.srf"))
            //    {
            //    using (var context = new CryptographicContext(CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
            //        using (var hashengine = new CryptHashAlgorithm(context, ALG_ID.CALG_GR3411_2012_256))
            //            {
            //            var hash = hashengine.Compute(Encoding.UTF8.GetBytes("sample"));
            //            client.SendRequest(new Oid("1.2.643.7.1.1.2.2"), hash, true, out Byte[] response);
            //            try
            //                {
            //                context.VerifyAttachedMessageSignature(response, out var certificates, null);
            //                foreach (var certificate in certificates)
            //                    {
            //                    Debug.Print($"{certificate.Thumbprint}");
            //                    }
            //                }
            //            finally
            //                {
            //                }
            //            }
            //        }
            //    }
            return;
            var store = new StoreClass();
            store.Open(CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, "My", CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_READ_ONLY);
            ICertContext c = null;
            foreach (ICertificate cert in store.Certificates) {
                if (String.Equals(cert.Thumbprint,"7a6666faef01d2f9849db282211282b95918279f", StringComparison.OrdinalIgnoreCase)) {
                    c = cert as ICertContext;
                    break;
                    }
                }
            using (var context = new CryptographicContext(CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                //using (var hashengine = new CryptHashAlgorithm(context, ALG_ID.CALG_GR3410_12_256))
                    {
                    //var hash = hashengine.Compute(Encoding.UTF8.GetBytes("sample"));
                    var hashvalue = new HashedDataClass();
                    //hashvalue.Algorithm.
                    hashvalue.Hash("sample");
                    var request = (new TSPRequestClass()) as ITSPRequest2;
                    //request.Hash = hashvalue;
                    request.CertReq = true;
                    request.UseNonce = false;
                    request.TSAAuthType = TSPCOM_AUTH_TYPE.TSPCOM_AUTH_TYPE_ANONYMOUS;
                    request.TSAAddress = @"http://testca2012.cryptopro.ru/tsp/tsp.srf";
                    var oid = new OIDClass
                        {
                        Value = "1.2.643.7.1.1.2.2"
                        };
                    request.HashAlgorithm2 = oid;
                    request.AddData(Encoding.ASCII.GetBytes("sample"));
                    //request.HashAlgorithm.Value = "1.2.643.7.1.1.2.2";
                    //request.PolicyID="1.2.3.4.5.6.7.8.9.0";
                    //request.ClientCertificate = c;
                    //request.TSAUserName = "maystrenco";
                    //request.TSAPassword = "9854312714";

                    //var d = request.Export();
                    request.Display();
                    var tsp = request.Send(false);
                    Debug.Print($"{tsp.Status}");
                    tsp.Display();
                    return;
                    }
                }
            }
        }
    }
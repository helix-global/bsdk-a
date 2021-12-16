using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.PlatformUI.Views;

namespace shell
    {
    internal class DocumentManager
        {
        private readonly DocumentGroup dockgroup;
        public DocumentManager(DocumentGroup dockgroup)
            {
            this.dockgroup = dockgroup;
            }

        #region M:Max<T>(T[],T[]):T[]
        private static T[] Max<T>(T[] x, T[] y) {
            return (x.Length >= y.Length)
                ? x
                : y;
            }
        #endregion
        #region M:LoadObject(String):Object
        public Object LoadObject(String filename) {
            try
                {
                return Max(
                    Asn1Object.Load(filename,Asn1ReadFlags.IgnoreLeadLineEnding).ToArray(),
                    Asn1Object.Load(filename).ToArray());
                }
            catch (Exception e)
                {
                return e;
                }
            }
        #endregion

        public IList<View> LoadView(Object source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = new List<View>();
            if (source is IList) {
                foreach (var o in (IList)source) {
                    r.AddRange(LoadView(o));
                    }
                }
            else if (source is Asn1Object o) {
                var crt = ReadCrt(o); if (crt != null) { r.Add(new View<ECertificate>(new ECertificate(new X509Certificate(crt)))); return r; }
                var cms = ReadCms(o); if (cms != null) { r.Add(new View<ECms>(new ECms(cms))); return r; }
                else
                    {
                    r.Add(new View<EAsn1>(new EAsn1(o)));
                    }
                }
            return r;
            }

        #region M:Add(View)
        public void Add(View source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            dockgroup.Children.Add(source);
            source.IsSelected = true;
            source.IsActive = true;
            }
        #endregion
        #region M:Add(IList<View>,String)
        public void Add(IList<View> source, String header) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Count == 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (source.Count == 1) {
                Add(source[0], header);
                }
            else
                {
                var i = 1;
                foreach (var o in source)
                    {
                    Add(o, $"{header}:{i}");
                    i++;
                    }
                }
            }
        #endregion
        #region M:Add(View,String)
        private void Add(View source, String header) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.Title = header;
            Add(source);
            }
        #endregion

        #region M:ReadCrt(Asn1Object):Asn1Certificate
        private static Asn1Certificate ReadCrt(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new Asn1Certificate(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }
        #endregion
        #region M:ReadCrl(Asn1Object):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList ReadCrl(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new Asn1CertificateRevocationList(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }
        #endregion
        #region M:ReadCms(Asn1Object):CmsMessage
        private static CmsMessage ReadCms(Asn1Object o) {
            if (o != null) {
                try
                    {
                    var r = new CmsMessage(o);
                    if (!r.IsFailed) { return r; }
                    }
                catch
                    {
                    return null;
                    }
                }
            return null;
            }
        #endregion
        }
    }
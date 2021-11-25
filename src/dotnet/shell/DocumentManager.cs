using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.PlatformUI.Shell.Controls;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
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

        private static T[] Max<T>(T[] x, T[] y) {
            return (x.Length >= y.Length)
                ? x
                : y;
            }

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

        public IList<View> LoadView(Object source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = new List<View>();
            if (source is IList) {
                foreach (var o in (IList)source) {
                    r.AddRange(LoadView(o));
                    }
                }
            else if (source is Asn1Object) {
                var o = new EAsn1((Asn1Object)source);
                //DocumentGroupControl.SetHasPinButton(o, false);
                r.Add(o);
                }
            return r;
            }

        public void Add(View source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            dockgroup.Children.Add(source);
            source.IsSelected = true;
            source.IsActive = true;
            }

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

        private void Add(View source, String header) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.Title = header;
            Add(source);
            }
        }
    }
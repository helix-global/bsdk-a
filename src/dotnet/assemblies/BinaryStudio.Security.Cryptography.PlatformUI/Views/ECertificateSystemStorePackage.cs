using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    internal class ECertificateSystemStorePackage : ECertificatePackage
        {
        private readonly HashSet<String> InternalNestedPackages;
        public override String PackageName { get; }

        protected override void EnsureNestedPackages(out ObservableCollection<Object> o) {
            o = new ObservableCollection<Object>();
            foreach (var i in InternalNestedPackages) {
                o.Add(new ECertificatePhysicalStorePackage(this, i));
                }
            }

        public ECertificateSystemStorePackage(ECertificatePackage parent, String name, HashSet<String> nesteditems)
            : base(parent)
            {
            PackageName = name;
            InternalNestedPackages = nesteditems;
            }
        }
    }
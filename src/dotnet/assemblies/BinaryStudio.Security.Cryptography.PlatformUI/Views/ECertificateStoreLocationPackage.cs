using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    internal class ECertificateStoreLocationPackage : ECertificatePackage
        {
        public X509StoreLocation Location { get; }
        public override String PackageName { get { return Location.ToString(); }}

        public ECertificateStoreLocationPackage(X509StoreLocation location)
            :base(null)
            {
            Location = location;
            }

        protected override void EnsureNestedPackages(out ObservableCollection<Object> o) {
            var r = new Dictionary<String, HashSet<String>>();
            foreach (var store in X509CertificateStorage.GetPhysicalStores(Location)) {
                var items = store.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length > 1) {
                    if (!r.TryGetValue(items[0], out var values)) { r.Add(items[0], values = new HashSet<String>()); }
                    values.Add(items[1]);
                    }
                }
            o = new ObservableCollection<Object>();
            foreach (var i in r) {
                o.Add(new ECertificateSystemStorePackage(this, i.Key, i.Value));
                }
            }
        }
    }
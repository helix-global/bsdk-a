using System;
using System.Collections.ObjectModel;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    internal class ECertificatePhysicalStorePackage : ECertificatePackage
        {
        public override String PackageName { get; }
        public ECertificatePhysicalStorePackage(ECertificatePackage parent, String name)
            : base(parent)
            {
            PackageName = name;
            }

        protected override void EnsureNestedPackages(out ObservableCollection<Object> o)
            {
            o = null;
            }
        }
    }
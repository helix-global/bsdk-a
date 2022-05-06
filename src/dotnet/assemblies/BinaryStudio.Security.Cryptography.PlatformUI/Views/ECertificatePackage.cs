using System;
using System.Collections.ObjectModel;
using BinaryStudio.PlatformUI;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    internal abstract class ECertificatePackage : NotifyPropertyChangedDispatcherObject
        {
        private Boolean IsSelectedInternal;
        private Boolean IsExpandedInternal;
        private ObservableCollection<Object> InternalNestedPackages;
        public virtual ObservableCollection<Object> NestedPackages { get {
            if (InternalNestedPackages == null) { EnsureNestedPackages(out InternalNestedPackages); }
            return InternalNestedPackages;
            }}

        public virtual String PackageName { get; }
        public ECertificatePackage ParentPackage { get; }
        #region P:IsSelected:Boolean
        public Boolean IsSelected
            {
            get { return IsSelectedInternal; }
            set { SetValue(ref IsSelectedInternal, value, nameof(IsSelected)); }
            }
        #endregion
        #region P:IsExpanded:Boolean
        public Boolean IsExpanded
            {
            get { return IsExpandedInternal; }
            set { SetValue(ref IsExpandedInternal, value, nameof(IsExpanded)); }
            }
        #endregion
        protected abstract void EnsureNestedPackages(out ObservableCollection<Object> o);

        protected ECertificatePackage(ECertificatePackage parent)
            {
            ParentPackage = parent;
            }
        }
    }
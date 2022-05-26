using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using BinaryStudio.PlatformUI;
using Microsoft.Win32;

namespace shell
    {
    internal class ERegistryKey : NotifyPropertyChangedDispatcherObject<RegistryKey>
        {
        private Boolean IsExpandedInternal;
        private Boolean IsSelectedInternal;
        public RegistryKey ActualSource { get;private set; }
        public String LocalName { get; }
        public String DisplayName { get; }
        public Boolean IsValue { get; }
        public Boolean HasPermissionCheckError { get;private set; }

        #region P:IsExpanded:Boolean
        public Boolean IsExpanded
            {
            get { return IsExpandedInternal; }
            set { SetValue(ref IsExpandedInternal, value, nameof(IsExpanded)); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public Boolean IsSelected
            {
            get { return IsSelectedInternal; }
            set
                {
                SetValue(ref IsSelectedInternal, value, nameof(IsSelected));
                }
            }
        #endregion
        public Object Value { get {
            return IsValue
                ? Source.GetValue(LocalName)
                : null;
            }}

        public RegistryValueKind? Type { get {
            return IsValue
                ? Source.GetValueKind(LocalName)
                : new RegistryValueKind?();
            }}

        public ERegistryKey(RegistryKey source)
            : base(source)
            {
            ActualSource = source;
            LocalName = source.Name;
            DisplayName = source.Name;
            }

        public ERegistryKey(RegistryKey source, String name, Boolean flags)
            : base(source)
            {
            LocalName = name;
            IsValue = flags;
            DisplayName = String.IsNullOrWhiteSpace(name) ? "{default}" : name;
            }

        private void EnsureActualSource()
            {
            if (!IsValue && !HasPermissionCheckError) {
                if (ActualSource == null) {
                    try
                        {
                        ActualSource = Source.OpenSubKey(LocalName, false);
                        }
                    catch (SecurityException)
                        {
                        HasPermissionCheckError = true;
                        }
                    }
                }
            }

        public IEnumerable<ERegistryKey> Children { get {
            EnsureActualSource();
            if (!IsValue) {
                if (!HasPermissionCheckError) {
                    foreach (var name in ActualSource.GetSubKeyNames()) { yield return new ERegistryKey(ActualSource,name,false); }
                    foreach (var name in ActualSource.GetValueNames())  { yield return new ERegistryKey(ActualSource,name, true); }
                    }
                }
            }}

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return DisplayName;
            }
        }
    }
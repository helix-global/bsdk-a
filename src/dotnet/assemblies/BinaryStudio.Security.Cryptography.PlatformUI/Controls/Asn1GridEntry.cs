using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Controls
    {
    public class Asn1GridEntry : DependencyObject
        {
        public IList<Asn1GridEntry> Entries { get{
            if (entries == null) {
                entries = new List<Asn1GridEntry>();
                foreach (var i in source)
                    {
                    entries.Add(new Asn1GridEntry(Owner, i, level + 1));
                    }
                }
            return entries;
            }}

        public Asn1Control Owner { get; }
        public String Class { get; }
        public String Type { get; }
        public String Header { get; }
        public String Offset { get; }
        public String Size { get; }
        public String Length { get; }
        public Boolean IsString { get; }
        public Boolean IsDateTime { get; }
        public Boolean IsExplicitConstructed { get; }
        public Boolean IsImplicitConstructed { get; }
        public Boolean IsIndefiniteLength { get; }
        public Boolean IsObjectIdentifier { get; }
        public Stream Content { get; }
        #region P:Margin:Thickness
        public Thickness Margin { get {
            return new Thickness(
                level*16,
                0,
                5,0);
            }}
        #endregion

        #region P:IsExpanded:Boolean
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean), typeof(Asn1GridEntry), new PropertyMetadata(default(Boolean)));
        public Boolean IsExpanded
            {
            get { return (Boolean)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(Asn1GridEntry), new PropertyMetadata(default(Boolean)));
        public Boolean IsSelected
            {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion
        #region P:IsObjectIdentifierDecoded:Boolean
        private static readonly DependencyPropertyKey IsObjectIdentifierDecodedPropertyKey = DependencyProperty.RegisterReadOnly("IsObjectIdentifierDecoded", typeof(Boolean), typeof(Asn1GridEntry), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsObjectIdentifierDecodedProperty = IsObjectIdentifierDecodedPropertyKey.DependencyProperty;
        public Boolean IsObjectIdentifierDecoded {
            get { return (Boolean)GetValue(IsObjectIdentifierDecodedProperty); }
            private set { SetValue(IsObjectIdentifierDecodedPropertyKey, value); }
            }
        #endregion
        #region P:DecodeObjectIdentifier:Boolean
        public static readonly DependencyProperty DecodeObjectIdentifierProperty = DependencyProperty.Register("DecodeObjectIdentifier", typeof(Boolean), typeof(Asn1GridEntry), new PropertyMetadata(default(Boolean), OnDecodeObjectIdentifierChanged));
        private static void OnDecodeObjectIdentifierChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as Asn1GridEntry);
            if (source != null) {
                source.DecodeObjectIdentifierInternal();
                }
            }

        public Boolean DecodeObjectIdentifier {
            get { return (Boolean)GetValue(DecodeObjectIdentifierProperty); }
            set { SetValue(DecodeObjectIdentifierProperty, value); }
            }
        #endregion
        #region P:ToolTip:Object
        private static readonly DependencyPropertyKey ToolTipPropertyKey = DependencyProperty.RegisterReadOnly("ToolTip", typeof(Object), typeof(Asn1GridEntry), new PropertyMetadata(default(Object)));
        public static readonly DependencyProperty ToolTipProperty = ToolTipPropertyKey.DependencyProperty;
        public Object ToolTip {
            get { return GetValue(ToolTipProperty); }
            private set { SetValue(ToolTipPropertyKey, value); }
            }
        #endregion
        #region P:Value:Object
        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value", typeof(Object), typeof(Asn1GridEntry), new PropertyMetadata(default(Object)));
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;
        public Object Value
            {
            get { return GetValue(ValueProperty); }
            private set { SetValue(ValuePropertyKey, value); }
            }
        #endregion

        public Asn1GridEntry(Asn1Control owner, Asn1Object source)
            :this(owner, source, 0)
            {
            IsExpanded = true;
            }

        public Asn1GridEntry(Asn1Control owner, Asn1Object source, Int32 level)
            {
            this.source = source;
            this.level = level;
            Owner = owner;
            Class = source.Class.ToString();
            Type = (source is Asn1UniversalObject)
                ? ((Asn1UniversalObject)source).Type.ToString()
                : ((IAsn1Object)source).Type.ToString();
            Header = (source.Class == Asn1ObjectClass.Universal)
                ? String.Format("{0}", Type)
                : String.Format("{0}[{1}]", Class, Type);
            Offset = source.Offset.ToString();
            Length = source.Length.ToString();
            Size   = source.Size.ToString();
            IsString = source is Asn1String;
            IsDateTime = source is Asn1Time;
            IsExplicitConstructed = source.IsExplicitConstructed;
            IsImplicitConstructed = source.IsImplicitConstructed;
            IsIndefiniteLength = source.IsIndefiniteLength;
            IsObjectIdentifier = source is Asn1ObjectIdentifier;
            Content = source.Content;
            //IsExpanded = true;
            Value = source.ToString();
            BindingOperations.SetBinding(this, DecodeObjectIdentifierProperty, new Binding { Source = owner, Path = new PropertyPath(Asn1Control.DecodeObjectIdentifierProperty), Mode = BindingMode.OneWay});
            }

        #region M:DecodeObjectIdentifierInternal
        private void DecodeObjectIdentifierInternal() {
            if (IsObjectIdentifier) {
                var value = source.ToString();
                if (DecodeObjectIdentifier) {
                    //var culture = LocalizationManager.CurrentUICulture;
                    var culture = CultureInfo.CurrentUICulture;
                    var r = OID.ResourceManager.GetString(value, culture);
                    if (CultureInfo.Equals(culture, Thread.CurrentThread.CurrentCulture)) {
                        if (String.IsNullOrEmpty(r)) {
                            r = (new Oid(value)).FriendlyName;
                            }
                        }
                    if (!String.IsNullOrEmpty(r)) {
                        ToolTip = Value;
                        Value = r;
                        IsObjectIdentifierDecoded = true;
                        return;
                        }
                    }
                ToolTip = null;
                Value = value;
                IsObjectIdentifierDecoded = false;
                }
            }
        #endregion

        private IList<Asn1GridEntry> entries;
        internal readonly Asn1Object source;
        private readonly Int32 level;
        }
    }
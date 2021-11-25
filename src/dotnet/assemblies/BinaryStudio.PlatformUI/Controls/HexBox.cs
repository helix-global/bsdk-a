using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BinaryStudio.DataProcessing.Annotations;
using BinaryStudio.PlatformUI.Controls.Markups;
using BinaryStudio.PlatformUI.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class HexBox : Control, INotifyPropertyChanged
        {
        static HexBox()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(typeof(HexBox)));
            FlowDirectionProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(OnFlowDirectionChanged));
            FontFamilyProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(OnFontFamilyChanged));
            FontStyleProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(OnFontStyleChanged));
            FontWeightProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(OnFontWeightChanged));
            FontStretchProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(OnFontStretchChanged));
            }

        #region P:Typeface:Typeface
        /// <summary>Identifies the <see cref="Typeface" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="Typeface" /> dependency property.</returns>
        private static readonly DependencyPropertyKey TypefacePropertyKey = DependencyProperty.RegisterReadOnly("Typeface", typeof(Typeface), typeof(HexBox), new PropertyMetadata(new Typeface("Consolas"), OnTypefaceChanged));
        public static readonly DependencyProperty TypefaceProperty = TypefacePropertyKey.DependencyProperty;
        private static void OnTypefaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnTypefaceChanged();
            }

        protected virtual void OnTypefaceChanged() {
            var source = new FormattedText("0", CultureInfo.CurrentCulture, FlowDirection, Typeface, FontSize, null);
            //ItemHeight = source.Height;
            }

        private void DoTypefaceChanged() {
            Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            }

        /// <summary>
        /// Combination of FontFamily, FontWeight, FontStyle, and FontStretch.
        /// </summary>
        public Typeface Typeface {
            get { return (Typeface)GetValue(TypefaceProperty); }
            private set { SetValue(TypefacePropertyKey, value); }
            }
        #endregion
        #region P:FlowDirection:FlowDirection
        private static void OnFlowDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnFlowDirectionChanged();
            }

        private void OnFlowDirectionChanged() {
            DoTypefaceChanged();
            }
        #endregion
        #region P:FontFamily:FontFamily
        private static void OnFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnFontFamilyChanged();
            }

        private void OnFontFamilyChanged() {
            DoTypefaceChanged();
            }
        #endregion
        #region P:FontStyle:FontStyle
        private static void OnFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnFontStyleChanged();
            }

        private void OnFontStyleChanged() {
            DoTypefaceChanged();
            }
        #endregion
        #region P:FontStretch:FontStretch
        private static void OnFontStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnFontStretchChanged();
            }

        private void OnFontStretchChanged() {
            DoTypefaceChanged();
            }
        #endregion
        #region P:FontWeight:FontWeight
        private static void OnFontWeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as HexBox)?.OnFontWeightChanged();
            }

        private void OnFontWeightChanged() {
            DoTypefaceChanged();
            }
        #endregion
        #region P:Source:Object
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Object), typeof(HexBox), new PropertyMetadata(default(Object), OnSourceChanged));
        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBox);
            if (source != null)
                {
                source.OnSourceChanged();
                }
            }

        private void OnSourceChanged()
            {
            }

        public Object Source
            {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
            }
        #endregion
        #region P:Markups:MarkupCollection
        public static readonly DependencyProperty MarkupsProperty = DependencyProperty.Register(nameof(Markups), typeof(MarkupCollection), typeof(HexBox), new PropertyMetadata(default(MarkupCollection), OnMarkupsChanged));
        private static void OnMarkupsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HexBox);
            if (source != null) {
                source.OnMarkupsChanged(
                    (MarkupCollection)e.OldValue,
                    (MarkupCollection)e.NewValue);
                }
            }

        private void OnMarkupsChanged(MarkupCollection o, MarkupCollection n)
            {
            if (o != null) { o.CollectionChanged -= OnMarkupsChanged; }
            if (n != null) { n.CollectionChanged += OnMarkupsChanged; }
            }

        private void OnMarkupsChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            OnPropertyChanged(nameof(Markups));
            }

        public MarkupCollection Markups
            {
            get { return (MarkupCollection)GetValue(MarkupsProperty); }
            set { SetValue(MarkupsProperty, value); }
            }
        #endregion

        public HexBox()
            {
            Markups = new MarkupCollection();
            }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        /// <summary>Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.</summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            H = GetTemplateChild("H") as HexBoxPanelH;
            if (H != null) {
                if (IsFocused) {
                    H.Focus();
                    }
                }
            }

        public void Select(Int32 startindex, Int32 length) {
            if (H != null) {
                H.Selection = new RangeSelection(startindex, length);
                }
            }

        private HexBoxPanelH H;
        }
    }

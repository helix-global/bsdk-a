using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Controls;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI
    {
    [DebuggerDisplay(@"\{{" + nameof(Name) + @"}\}")]
    public class Theme : DependencyObject
        {
        private readonly IDictionary<Type, Style> styles = new Dictionary<Type, Style>();
        private static ResourceKey ToolBarDropDownButtonStyleKeyInternal;

        #region P:CurrentTheme:Theme
        public static Theme CurrentTheme { get; private set; }
        #endregion
        #region P:Application:Application
        private static Application Application { get {
            return Application.Current ?? new Application{
                ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
            }}
        #endregion

        public ResourceDictionary Source {get; }
        public String Name { get; }

        private Theme(String name, String source) {
            Name = name;
            Source = LoadResourceDictionary(source);
            }

        public static Theme[] Themes = {
            new Theme("Classic",            "Classic.xaml"),
            new Theme("Luna.HomeStead",     "Luna.HomeStead.xaml"),
            new Theme("Luna.Metallic",      "Luna.Metallic.xaml"),
            new Theme("Luna.NormalColor",   "Luna.NormalColor.xaml"),
            new Theme("Royale.NormalColor", "Royale.NormalColor.xaml"),
            new Theme("Aero",               "Aero.NormalColor.xaml"),
            new Theme("NormalColor",        "Modern.NormalColor.xaml"),
            new Theme("Dark",               "Modern.Dark.xaml"),
            new Theme("Light",              "Modern.Light.xaml"),
            };

        public static event EventHandler ThemeApply;
        public static ResourceKey ToolBarDropDownButtonStyleKey { get{
            return ToolBarDropDownButtonStyleKeyInternal = ToolBarDropDownButtonStyleKeyInternal??new ComponentResourceKey(typeof(DropDownButton), nameof(ToolBarDropDownButtonStyleKey));
            }}

        #region M:Apply
        public static void Apply() {
            Apply(Themes[0]);
            }
        #endregion
        #region M:Apply(Theme)
        public static void Apply(Theme source) {
            if (source == null) { return; }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            CurrentTheme = source;
            if (Application != null) {
                Application.Resources.MergedDictionaries.Add(source.Source);
                //ValidateBrush("HighlightBrushKey", "HighlightLightBrushKey",      "WindowBrushKey", 0.5f);
                //ValidateBrush("HighlightBrushKey", "HighlightLightLightBrushKey", "WindowBrushKey", 0.5f * 0.5f);
                OnApply(source.Name);
                if (ThemeApply != null) {
                    ThemeApply(source, EventArgs.Empty);
                    }
                }
            }
        #endregion
        #region M:Apply(String)
        public static void Apply(String source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (String.IsNullOrWhiteSpace(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            foreach (var theme in Themes) {
                if (String.Equals(theme.Name, source, StringComparison.OrdinalIgnoreCase)) {
                    Apply(theme);
                    return;
                    }
                }
            throw new ArgumentOutOfRangeException(nameof(source));
            }
        #endregion
        #region M:LoadResourceDictionary(String):ResourceDictionary
        private static ResourceDictionary LoadResourceDictionary(String source) {
            return LoadResourceDictionary("BinaryStudio.PlatformUI", source);
            }
        #endregion
        #region M:LoadResourceDictionary(String,String):ResourceDictionary
        private static ResourceDictionary LoadResourceDictionary(String assembly, String source) {
            var r = (ResourceDictionary)Application.LoadComponent(
                new Uri($"{assembly};component/Themes/{source}",
                UriKind.Relative));
            Debug.Print($"{assembly};component/Themes/{source}");
            return r;
            }
        #endregion
        private static Color Update(Color source, Single brightness) {
            return Color.FromScRgb(source.ScA, source.ScR*brightness, source.ScG*brightness, source.ScB*brightness);
            }
        private static Color Update(Color source, Int32 brightness) {
            return Color.FromArgb(source.A,
                (Byte)Math.Min(255, source.R+brightness),
                (Byte)Math.Min(255, source.G+brightness),
                (Byte)Math.Min(255, source.B+brightness));
            }

        private static void ValidateBrush(Object source, Object target, Object @base, Single opacity) {
            var src = Application.Resources[source];
            var trg = Application.Resources[target];
            if (!Equals(src, trg)) {
                //Application.Resources[target] = (new ColorModifierExtension(opacity)).Convert(src, Application.Resources[@base]);
                }
            }

        private static void OnApply(String source) {
            return;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly  in assemblies) {
                if (!ReferenceEquals(assembly, Assembly.GetExecutingAssembly())) {
                    var resources = assembly.GetManifestResourceNames();
                    if (resources.Length > 0) {
                        foreach (var resource in resources) {
                            if (resource.EndsWith(".g.resources")) {
                                try
                                    {
                                    var r = LoadResourceDictionary(Path.GetFileNameWithoutExtension(assembly.Location), $"Modern.{source}.xaml");
                                    Application.Resources.MergedDictionaries.Add(r);
                                    }
                                catch (IOException) { }
                                catch (MissingSatelliteAssemblyException) { }
                                catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }

        //private static Style FindStyle(ResourceDictionary source, Type key) {
        //    var r = source[key] as DataTemplate;
        //    if (r != null) { return r; }
        //    foreach (var i in source.MergedDictionaries) {
        //        r = FindTemplate(i, key);
        //        if (r != null) { return r; }
        //        }
        //    return null;
        //    }

        //private Style FindStyle(ResourceDictionary source, Type type) {
        //    Style r;
        //    if (!styles.TryGetValue(type, out r)) {
        //        var j = type;
        //        while (j != null) {
        //            if (j == typeof(Object)) { break; }
        //            var key = new DataTemplateKey(j);
        //            r = FindTemplate(source, key);
        //            if (r == null) {
        //                foreach (var i in j.GetInterfaces()) {
        //                    key = new DataTemplateKey(i);
        //                    r = FindTemplate(source, key);
        //                    if (r != null) { break; }
        //                    }
        //                }
        //            if (r != null) { break; }
        //            j = j.BaseType;
        //            }
        //        if (r != null) {
        //            cache[type] = r;
        //            }
        //        }
        //    return r;
        //    }

        #region M:ApplyTo(FrameworkElement,ResourceDictionary)
        private static void ApplyTo(FrameworkElement e, ResourceDictionary source) {
            if (e != null) {
                var type = e.GetType();
                foreach (var dictionary in source.MergedDictionaries) { ApplyTo(e, dictionary); }
                foreach (DictionaryEntry i in source) {
                    e.Resources[i.Key] = i.Value;
                    }
                }
            }
        #endregion
        #region M:ApplyTo(FrameworkElement)
        public void ApplyTo(FrameworkElement e) {
            if (e != null) {
                ApplyTo(e, Source);
                return;
                }
            }
        #endregion

        private static void OnCultureChanged(CultureInfo culture) {
            var handler = CultureChanged;
            if (handler != null) {
                handler(CurrentTheme, EventArgs.Empty);
                }
            }

        public static event EventHandler CultureChanged;
        private static CultureInfo culture = CultureInfo.InstalledUICulture;
        public static CultureInfo Culture {
            get { return culture; }
            set
                {
                culture = value ?? CultureInfo.InstalledUICulture;
                OnCultureChanged(value);
                }
            }

        private class Tracker : ComputerBasedTrainingTracker
            {
            public Tracker()
                {

                }

            //private event EventHandler<TrackFocusEventArgs> E;
            //public event EventHandler<TrackFocusEventArgs> TrackFocus {
            //    [MethodImpl(MethodImplOptions.Synchronized)]
            //    add
            //        {
            //        E += value;
            //        EnsureHandle();
            //        }
            //    [MethodImpl(MethodImplOptions.Synchronized)]
            //    remove
            //        {
            //        E -= value;
            //        TryDestroyHandle();
            //        }
            //    }
            }

        public override String ToString()
            {
            return Name;
            }

        static Theme()
            {
            }
        }
    }
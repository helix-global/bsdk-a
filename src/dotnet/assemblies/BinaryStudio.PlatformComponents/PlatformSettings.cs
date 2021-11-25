using System;
using System.Globalization;

namespace BinaryStudio.PlatformComponents
    {
    public class PlatformSettings
        {
        private static CultureInfo culture = CultureInfo.InstalledUICulture;
        #region P:DefaultCulture:CultureInfo
        public static CultureInfo DefaultCulture {
            get { return culture; }
            set
                {
                value = value ?? CultureInfo.InstalledUICulture;
                culture = value;
                DefaultCultureChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        #endregion
        public static event EventHandler DefaultCultureChanged;
        }
    }
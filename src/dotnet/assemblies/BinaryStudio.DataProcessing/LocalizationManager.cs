using System.Globalization;

namespace BinaryStudio.DataProcessing
    {
    public class LocalizationManager
        {
        private static CultureInfo culture = CultureInfo.InstalledUICulture;

        #region P:DefaultCulture:CultureInfo
        public static CultureInfo DefaultCulture {
            get { return culture; }
            set
                {
                value = value ?? CultureInfo.InstalledUICulture;
                culture = value;
                }
            }
        #endregion
        }
    }
using System;
using System.Globalization;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI
    {
    public class MultiValueHelper
        {
        public static T CheckValue<T>(Object[] values, Int32 index) {
            if (!(values[index] is T) && values[index] != null) {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Resources.Error_ValueAtOffsetNotOfType, index, typeof(T).FullName));
                }
            return (T)values[index];
            }

        public static void CheckType<T>(Type[] types, Int32 index) {
            if (!types[index].IsAssignableFrom(typeof(T))) {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.Error_TargetAtOffsetNotExtendingType, index, typeof(T).FullName));
                }
            }
        }
    }
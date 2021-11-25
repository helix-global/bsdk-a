using System;
using System.Globalization;
using BinaryStudio.PlatformUI.Properties;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class Validate
        {
        public static void IsNotNull(Object o, String paramName)
            {
            if (o == null)
                throw new ArgumentNullException(paramName);
            }

        public static void IsNull(Object o, String paramName)
            {
            if (o != null)
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_InvalidOperation, paramName));
            }

        public static void IsNotEmpty(String s, String paramName)
            {
            if (s == String.Empty)
                throw new ArgumentException(Resources.ValidateError_StringEmpty, paramName);
            }

        public static void IsNotEmpty(Guid g, String paramName)
            {
            if (g == Guid.Empty)
                throw new ArgumentException(Resources.ValidateError_GuidEmpty, paramName);
            }

        public static void IsNotWhiteSpace(String s, String paramName)
            {
            if (s != null && String.IsNullOrWhiteSpace(s))
                throw new ArgumentException(Resources.ValidateError_StringWhiteSpace, paramName);
            }

        public static void IsNotNullAndNotEmpty(String s, String paramName)
            {
            IsNotNull(s, paramName);
            IsNotEmpty(s, paramName);
            }

        public static void IsNotNullAndNotWhiteSpace(String s, String paramName)
            {
            IsNotNull(s, paramName);
            IsNotWhiteSpace(s, paramName);
            }

        public static void IsEqual(Int32 value, Int32 expectedValue, String paramName)
            {
            if (value != expectedValue)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_InvalidValue_Format, value, expectedValue), paramName);
            }

        public static void IsEqual(UInt32 value, UInt32 expectedValue, String paramName)
            {
            if ((Int32)value != (Int32)expectedValue)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_InvalidValue_Format, value, expectedValue), paramName);
            }

        public static void IsNotEqual(Int32 value, Int32 unexpectedValue, String paramName)
            {
            if (value == unexpectedValue)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_UnexpectedValue_Format, value, paramName), paramName);
            }

        public static void IsNotEqual(UInt32 value, UInt32 unexpectedValue, String paramName)
            {
            if ((Int32)value == (Int32)unexpectedValue)
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_UnexpectedValue_Format, value, paramName), paramName);
            }

        public static void IsWithinRange(Int32 value, Int32 min, Int32 max, String paramName)
            {
            if (value < min || value > max)
                {
                var message = String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_OutOfRange_Format, value, min, max);
                throw new ArgumentOutOfRangeException(paramName, message);
                }
            }

        public static void IsWithinRange(Int64 value, Int64 min, Int64 max, String paramName)
            {
            if (value < min || value > max)
                {
                var message = String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_OutOfRange_Format, value, min, max);
                throw new ArgumentOutOfRangeException(paramName, message);
                }
            }

        public static void IsWithinRange(UInt32 value, UInt32 min, UInt32 max, String paramName)
            {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_OutOfRange_Format, value, min, max), paramName);
            }

        public static void IsWithinRange(UInt64 value, UInt64 min, UInt64 max, String paramName)
            {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_OutOfRange_Format, value, min, max), paramName);
            }

        public static void IsNormalized(String path, String paramName)
            {
            if (!PathUtil.IsNormalized(path))
                throw new ArgumentException(String.Format(CultureInfo.CurrentUICulture, Resources.ValidateError_PathNotNormalized_Format, path), paramName);
            }
        }
    }
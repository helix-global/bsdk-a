using System;

namespace BinaryStudio.PlatformComponents
    {
    public static class Extensions
        {
        internal static T Add<T>(this T e, String key, Object value)
            where T: Exception
            {
            if (value != null)
                {
                e.Data[key] = value;
                }
            return e;
            }
        }
    }
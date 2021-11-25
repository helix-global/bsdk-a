// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
#if !NET_4_7
using System.Globalization;
using System.Security;

namespace System
{
    internal class Environment
    {
        [SecuritySafeCritical]
        internal static string GetResourceString(string key, params object[] values)
            {
            string resourceString = GetResourceString(key);
            return string.Format(CultureInfo.CurrentCulture, resourceString, values);
            }

        [SecuritySafeCritical]
        internal static string GetResourceString(string key)
            {
            return "";
            }
        }
}
#endif
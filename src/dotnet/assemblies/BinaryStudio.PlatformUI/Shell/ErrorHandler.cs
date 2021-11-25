using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformUI
    {
    public class ErrorHandler
        {
        public static Boolean Succeeded(Int32 hr)
            {
            return hr >= 0;
            }

        public static Boolean IsRejectedRpcCall(Int32 hr)
            {
            if (hr != -2147417845 && hr != -2147417846)
                return hr == -2147417847;
            return true;
            }

        public static Boolean Failed(Int32 hr)
            {
            return hr < 0;
            }
        public static Int32 ThrowOnFailure(Int32 hr)
            {
            return ThrowOnFailure(hr, null);
            }

        public static Int32 ThrowOnFailure(Int32 hr, params Int32[] expectedHRFailure)
            {
            if (Failed(hr) && (expectedHRFailure == null || Array.IndexOf(expectedHRFailure, hr) < 0))
                Marshal.ThrowExceptionForHR(hr);
            return hr;
            }
        }
    }
using System;
using System.Security;

namespace BinaryStudio.PlatformComponents
    {
    public class PrincipalPermissionException : SecurityException
        {
        public PrincipalPermissionException(String message, Exception e)
            :base(message, e)
            {
            }
        }
    }
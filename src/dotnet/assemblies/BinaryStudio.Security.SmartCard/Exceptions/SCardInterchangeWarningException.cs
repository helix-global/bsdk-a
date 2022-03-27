using System;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardInterchangeWarningException : SCardInterchangeException
        {
        internal SCardInterchangeWarningException(String message):
            base(message)
            {
            }
        }
    }
using System;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardInterchangeCheckingErrorException : SCardInterchangeException
        {
        internal SCardInterchangeCheckingErrorException(String message):
            base(message)
            {
            }
        }
    }
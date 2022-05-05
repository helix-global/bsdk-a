using System;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardInterchangeExecutionErrorException : SCardInterchangeException
        {
        internal SCardInterchangeExecutionErrorException(String message):
            base(message)
            {
            }
        }
    }
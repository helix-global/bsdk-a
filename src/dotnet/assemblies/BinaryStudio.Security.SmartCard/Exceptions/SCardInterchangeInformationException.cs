using System;

namespace BinaryStudio.Security.SmartCard
    {
    public class SCardInterchangeInformationException : SCardInterchangeException
        {
        internal SCardInterchangeInformationException(String message):
            base(message)
            {
            }
        }
    }
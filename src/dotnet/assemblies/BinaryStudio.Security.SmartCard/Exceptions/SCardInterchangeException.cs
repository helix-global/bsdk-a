using System;
using System.Text;

namespace BinaryStudio.Security.SmartCard
    {
    public abstract class SCardInterchangeException : Exception
        {
        protected SCardInterchangeException(String message):
            base(message)
            {
            }

        public static Exception GetExceptionForCode(Int32 code)
            {
            var HI = (code >> 8) & 0xff;
            var LO = (code     ) & 0xff;
            switch (HI) {
                case 0x90:
                    {
                    if (LO == 0x00) { return new SCardInterchangeInformationException("Normal processing."); }
                    }
                    break;
                case 0x61: { return new SCardInterchangeInformationException($"{LO} data bytes still available."); }
                case 0x62:
                    {
                    var r = new StringBuilder("State of non-volatile memory is unchanged.");
                    switch (LO)
                        {
                        case 0x00: r.Append(" No information given."); break;
                        case 0x81: r.Append(" Part of returned data may be corrupted."); break;
                        case 0x82: r.Append(" End of file or record reached before reading N{e} bytes."); break;
                        case 0x83: r.Append(" Selected file deactivated."); break;
                        case 0x84: r.Append(" File control information not formatted."); break;
                        case 0x85: r.Append(" Selected file in termination state."); break;
                        case 0x86: r.Append(" No input data available from a sensor on the card."); break;
                        default: r.AppendFormat("Triggering by the card {{{0:x4}}}.", code); break;
                        }
                    return new SCardInterchangeWarningException(r.ToString());
                    }
                case 0x63:
                    {
                    var r = new StringBuilder("State of non-volatile memory has changed.");
                    switch (LO)
                        {
                        case 0x00: r.Append(" No information given."); break;
                        case 0x81: r.Append(" File filled up by the last write."); break;
                        default:
                            {
                            if ((LO >= 0xc0) && (LO <= 0xcf)) {
                                r.AppendFormat(" Counter:{{{0}}}", LO - 0xc0);
                                }
                            }
                            break;
                        }
                    return new SCardInterchangeWarningException(r.ToString());
                    }
                case 0x64:
                    {
                    var r = new StringBuilder("State of non-volatile memory is unchanged.");
                    switch (LO)
                        {
                        case 0x00: r.Append(" No information given."); break;
                        case 0x01: r.Append(" Immediate response required by the card."); break;
                        default: r.AppendFormat("Triggering by the card {{{0:x4}}}.", code); break;
                        }
                    return new SCardInterchangeExecutionErrorException(r.ToString());
                    }
                case 0x65:
                    {
                    var r = new StringBuilder("State of non-volatile memory has changed.");
                    switch (LO)
                        {
                        case 0x00: r.Append(" No information given."); break;
                        case 0x81: r.Append(" Memory failure."); break;
                        }
                    return new SCardInterchangeExecutionErrorException(r.ToString());
                    }
                case 0x66: { return new SCardInterchangeExecutionErrorException($"Security-related issues {{{LO.ToString("x4")}}}."); }
                case 0x6c: { return new SCardInterchangeCheckingErrorException($"Wrong L{{e}} field. Available {LO} data bytes."); }
                case 0x67:
                    {
                    return new SCardInterchangeCheckingErrorException((LO == 0)
                        ? "Wrong length."
                        : $"Checking error {{{code.ToString("x4")}}}.");
                    }
                case 0x68:
                    {
                    var r = new StringBuilder("Functions in CLA not supported");
                    switch (LO)
                        {
                        case 0x00: r.Append(". No information given."); break;
                        case 0x81: r.Append(". Logical channel not supported."); break;
                        case 0x82: r.Append(". Secure messaging not supported."); break;
                        case 0x83: r.Append(". Last command of the chain expected."); break;
                        case 0x84: r.Append(". Command chaining not supported."); break;
                        default: r.AppendFormat(" {{{0:x4}}}.", code); break;
                        }
                    return new SCardInterchangeCheckingErrorException(r.ToString());
                    }
                case 0x69:
                    {
                    var r = new StringBuilder("Command not allowed");
                    switch (LO)
                        {
                        case 0x00: r.Append(". No information given."); break;
                        case 0x81: r.Append(". Command incompatible with file structure."); break;
                        case 0x82: r.Append(". Security status not satisfied."); break;
                        case 0x83: r.Append(". Authentication method blocked."); break;
                        case 0x84: r.Append(". Reference data not usable."); break;
                        case 0x85: r.Append(". Conditions of use not satisfied."); break;
                        case 0x86: r.Append(". Command not allowed (no current EF)."); break;
                        case 0x87: r.Append(". Expected secure messaging data objects missing."); break;
                        case 0x88: r.Append(". Incorrect secure messaging data objects."); break;
                        default: r.AppendFormat(" {{{0:x4}}}.", code); break;
                        }
                    return new SCardInterchangeCheckingErrorException(r.ToString());
                    }
                case 0x6a:
                    {
                    var r = new StringBuilder("Wrong parameters {P1}-{P2}");
                    switch (LO)
                        {
                        case 0x00: r.Append(". No information given."); break;
                        case 0x80: r.Append(". Incorrect parameters in the command data field."); break;
                        case 0x81: r.Append(". Function not supported."); break;
                        case 0x82: r.Append(". File or application not found."); break;
                        case 0x83: r.Append(". Record not found."); break;
                        case 0x84: r.Append(". Not enough memory space in the file."); break;
                        case 0x85: r.Append(". N{c} inconsistent with TLV structure."); break;
                        case 0x86: r.Append(". Incorrect parameters {P1}-{P2}."); break;
                        case 0x87: r.Append(". N{c} inconsistent with parameters {P1}-{P2}."); break;
                        case 0x88: r.Append(". Referenced data or reference data not found."); break;
                        case 0x89: r.Append(". File already exists."); break;
                        case 0x8a: r.Append(". DF name already exists."); break;
                        default: r.AppendFormat(" {{{0:x4}}}.", code); break;
                        }
                    return new SCardInterchangeCheckingErrorException(r.ToString());
                    }
                case 0x6b:
                    {
                    return new SCardInterchangeCheckingErrorException((LO == 0)
                        ? "Wrong parameters {P1}-{P2}."
                        : $"Checking error {{{code.ToString("x4")}}}.");
                    }
                case 0x6d:
                    {
                    return new SCardInterchangeCheckingErrorException((LO == 0)
                        ? "Instruction code not supported or invalid."
                        : $"Checking error {{{code.ToString("x4")}}}.");
                    }
                case 0x6e:
                    {
                    return new SCardInterchangeCheckingErrorException((LO == 0)
                        ? "Class not supported."
                        : $"Checking error {{{code.ToString("x4")}}}.");
                    }
                case 0x6f:
                    {
                    return new SCardInterchangeCheckingErrorException((LO == 0)
                        ? "No precise diagnosis."
                        : $"Checking error {{{code.ToString("x4")}}}.");
                    }
                }
            return new SCardInterchangeCheckingErrorException($"Smart card error {{{code.ToString("x4")}}}");
            }
        }
    }
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public class LocalCache
        {
        public readonly HashSet<DObjectIdentifier> ObjectIdentifiers = new HashSet<DObjectIdentifier>();
        public readonly HashSet<DString> Strings = new HashSet<DString>();
        public readonly HashSet<DRelativeDistinguishedName> RelativeDistinguishedNames = new HashSet<DRelativeDistinguishedName>();
        public readonly HashSet<DRelativeDistinguishedNameSequence> RelativeDistinguishedNameSequences = new HashSet<DRelativeDistinguishedNameSequence>();
        public readonly HashSet<DGeneralName> GeneralNames = new HashSet<DGeneralName>();
        }
    }
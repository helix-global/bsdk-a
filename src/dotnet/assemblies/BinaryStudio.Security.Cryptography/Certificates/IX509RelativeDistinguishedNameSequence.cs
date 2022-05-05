using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509RelativeDistinguishedNameSequence : IX509GeneralName, IEquatable<IX509GeneralName>
        {
        Boolean TryGetValue(String key, out String r);
        }
    }
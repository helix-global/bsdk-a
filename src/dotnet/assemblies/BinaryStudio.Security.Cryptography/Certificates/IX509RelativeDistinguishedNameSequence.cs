using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509RelativeDistinguishedNameSequence
        {
        Boolean TryGetValue(String key, out String r);
        }
    }
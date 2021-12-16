using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DRelativeDistinguishedNameSequence :
        IEquatable<Asn1RelativeDistinguishedNameSequence>
        {
        public Boolean Equals(Asn1RelativeDistinguishedNameSequence other)
            {
            if (other == null) { return false; }
            var x = other.Select(i => new KeyValuePair<String,String>(i.Key.ToString(), i.Value.ToString())).OrderBy(i => i.Key).ToArray();
            var y = RelativeDistinguishedNameSequenceMapping.
                Select(i => new KeyValuePair<String,String>(i.RelativeDistinguishedName.ObjectIdentifier.Value, i.RelativeDistinguishedName.String.Value)).OrderBy(i=> i.Key).ToArray();
            if (x.Length != y.Length) { return false; }
            for (var i = 0; i < x.Length; i++) {
                if (x[i].Key != y[i].Key) { return false; }
                if (!String.Equals(x[i].Value, y[i].Value)) { return false; }
                }
            return true;
            }
        }
    }
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BinaryStudio.Security.Cryptography.Interchange
{
    using System;
    using System.Collections.Generic;
    
    public partial class DCertificate
    {
        public int ObjectId { get; set; }
        public string Country { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public int Issuer { get; set; }
        public int Subject { get; set; }
        public System.DateTime NotBefore { get; set; }
        public Nullable<System.DateTime> NotAfter { get; set; }
    
        public virtual DObject Object { get; set; }
        public virtual DRelativeDistinguishedNameSequence RelativeDistinguishedNameSequence { get; set; }
        public virtual DRelativeDistinguishedNameSequence RelativeDistinguishedNameSequence1 { get; set; }
    }
}
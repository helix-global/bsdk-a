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
    
    public partial class DObject
    {
        public DObject()
        {
            this.Extension = new HashSet<DExtension>();
        }
    
        public int ObjectId { get; set; }
        public byte Type { get; set; }
    
        public virtual DCertificate Certificate { get; set; }
        public virtual DCertificateRevocationList CertificateRevocationList { get; set; }
        public virtual ICollection<DExtension> Extension { get; set; }
    }
}

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
    
    public partial class DExtension
    {
        public DExtension()
        {
            this.AlternativeName = new HashSet<DAlternativeName>();
        }
    
        public int ExtensionId { get; set; }
        public Nullable<int> ObjectId { get; set; }
        public int Type { get; set; }
        public bool IsCritical { get; set; }
    
        public virtual ICollection<DAlternativeName> AlternativeName { get; set; }
        public virtual DAuthorityKeyIdentifier AuthorityKeyIdentifier { get; set; }
        public virtual DObject Object { get; set; }
        public virtual DObjectIdentifier ObjectIdentifier { get; set; }
        public virtual DIssuerAlternativeName IssuerAlternativeName { get; set; }
        public virtual DSubjectAlternativeName SubjectAlternativeName { get; set; }
        public virtual DSubjectKeyIdentifier SubjectKeyIdentifier { get; set; }
    }
}

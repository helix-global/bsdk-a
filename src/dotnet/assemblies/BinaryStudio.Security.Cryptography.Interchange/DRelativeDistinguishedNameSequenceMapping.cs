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
    
    public partial class DRelativeDistinguishedNameSequenceMapping
    {
        public int MappingId { get; set; }
        public int RelativeDistinguishedNameSequenceId { get; set; }
        public int RelativeDistinguishedNameId { get; set; }
    
        public virtual DRelativeDistinguishedName RelativeDistinguishedName { get; set; }
        public virtual DRelativeDistinguishedNameSequence RelativeDistinguishedNameSequence { get; set; }
    }
}
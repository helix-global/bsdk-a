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
    
    public partial class DAlternativeName
    {
        public int MappingId { get; set; }
        public Nullable<int> ExtensionId { get; set; }
        public Nullable<int> GeneralNameId { get; set; }
    
        public virtual DExtension Extension { get; set; }
        public virtual DGeneralName GeneralName { get; set; }
    }
}
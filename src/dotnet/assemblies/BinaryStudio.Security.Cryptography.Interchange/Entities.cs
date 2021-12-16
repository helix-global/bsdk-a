using System;

namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class Entities
        {
        public Entities(String connection)
            : base(connection)
            {
            this.Configuration.LazyLoadingEnabled = false;
            }
        }
    }
using System.Security.Cryptography;

namespace Options
    {
    internal class AlgId : Option<Oid>
        {
        public AlgId(Oid value)
            : base(value)
            {
            }
        }
    }
using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public struct UnlimitedNatural
        {
        public static readonly UnlimitedNatural Zero = new UnlimitedNatural();
        public static readonly UnlimitedNatural Unlimited = new UnlimitedNatural(true);

        private UInt64 Value {get;}
        public Boolean IsUnlimited {get;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public UnlimitedNatural(UInt64 source) {
            Value = source;
            IsUnlimited = false;
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        private UnlimitedNatural(Boolean source) {
            Value = 0;
            IsUnlimited = source;
            }
        }
    }
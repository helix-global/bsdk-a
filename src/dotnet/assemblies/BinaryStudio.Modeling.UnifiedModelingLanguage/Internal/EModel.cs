using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EModel : EPackage, Model
        {
        public String Viewpoint { get; }

        public EModel(String identifer)
            : base(identifer)
            {
            }
        }
    }
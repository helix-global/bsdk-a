using System;

namespace BinaryStudio.Modeling.UnifiedModelingLanguage.Internal
    {
    public class EComponent : EClass, Component
        {
        public EComponent(String identifer)
            : base(identifer)
            {
            }

        public PackageableElement[] PackagedElement { get; }
        public Boolean IsIndirectlyInstantiated { get; }
        public Interface[] Provided { get; }
        public ComponentRealization[] Realization { get; }
        public Interface[] Required { get; }
        }
    }
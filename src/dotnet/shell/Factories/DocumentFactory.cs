using System;

namespace shell.Factories
    {
    internal abstract class DocumentFactory
        {
        public abstract Object LoadDocument(Object source);
        }
    }
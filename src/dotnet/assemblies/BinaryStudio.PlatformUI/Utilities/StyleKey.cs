using System;
using System.Reflection;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    public sealed class StyleKey<T> : ResourceKey
        {
        private Assembly assembly;

        public override Assembly Assembly
            {
            get
                {
                var assembly = this.assembly;
                if ((Object)assembly != null)
                    return assembly;
                return this.assembly = typeof(T).Assembly;
                }
            }
        }
    }
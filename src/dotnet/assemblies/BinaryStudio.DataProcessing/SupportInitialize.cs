using System;
using System.ComponentModel;

namespace BinaryStudio.DataProcessing
    {
    public class SupportInitialize : IDisposable
        {
        public ISupportInitialize Source { get;private set; }
        public SupportInitialize(ISupportInitialize source) {
            Source = source;
            if (Source != null) {
                Source.BeginInit();
                }
            }

        public void Dispose() {
            if (Source != null) {
                Source.EndInit();
                Source = null;
                }
            }
        }
    }
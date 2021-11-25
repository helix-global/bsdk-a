using System;
using System.IO;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    internal class TypeLibraryTextWriter : TextWriter
        {
        private TextWriter Writer { get; }
        public Int32 IndentLevel { get;private set; }
        public override Encoding Encoding { get; }

        public TypeLibraryTextWriter(TextWriter writer)
            {
            Writer = writer;
            }

        public override void WriteLine()
            {
            Writer.WriteLine();
            }

        //override 

        public IDisposable IndentedScope()
            {
            return new IndentedScopeInternal(this); 
            }

        private class IndentedScopeInternal : IDisposable
            {
            private TypeLibraryTextWriter source;
            public IndentedScopeInternal(TypeLibraryTextWriter source)
                {
                this.source = source;
                this.source.IndentLevel += 1;
                }

            public void Dispose()
                {
                source.IndentLevel -= 1;
                source = null;
                }
            }
        }
    }
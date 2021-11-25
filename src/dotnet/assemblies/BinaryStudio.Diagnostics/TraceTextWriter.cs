using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BinaryStudio.Diagnostics
    {
    public class TraceTextWriter : TextWriter
        {
        public override Encoding Encoding { get { return Encoding.Unicode; }}

        #region M:Write(String)
        public override void Write(String value)
            {
            Trace.Write(value);
            }
        #endregion
        #region M:Write(Char)
        public override void Write(Char value)
            {
            Trace.Write(value);
            }
        #endregion
        #region M:WriteLine(Char)
        public override void WriteLine(Char value)
            {
            Trace.WriteLine(value);
            }
        #endregion
        #region M:WriteLine
        public override void WriteLine()
            {
            Trace.WriteLine(String.Empty);
            }
        #endregion
        #region M:WriteLine(String)
        public override void WriteLine(String value)
            {
            Trace.WriteLine(value);
            }
        #endregion
        }
    }
using System;
using System.IO;
using System.Text;

namespace BinaryStudio.Diagnostics
    {
    internal class ExceptionWriter : TextWriter, IExceptionWriter
        {
        private class Context
            {

            }

        public override Encoding Encoding { get; }
        public ExceptionWriter(TextWriter source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Encoding = source.Encoding;
            this.source = source;
            }

        /// <summary>Writes a line terminator to the text string or stream.</summary>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void WriteLine()
            {
            source.WriteLine();
            }

        /// <summary>Writes a string followed by a line terminator to the text string or stream.</summary>
        /// <param name="value">The string to write. If <paramref name="value"/> is <see langword="null"/>, only the line terminator is written.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void WriteLine(String value)
            {
            source.WriteLine(value);
            }

        /// <summary>Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.</summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format and write.</param>
        /// <exception cref="T:System.ArgumentNullException">A string or object is passed in as <see langword="null"/>.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        /// <exception cref="T:System.FormatException"><paramref name="format"/> is not a valid composite format string. -or-
        /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.</exception>
        public override void WriteLine(String format, params Object[] args)
            {
            source.WriteLine(format, args);
            }

        /// <summary>Writes a string to the text string or stream.</summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
        public override void Write(String value)
            {
            source.Write(value);
            }

        public IDisposable IndentScope()
            {
            throw new NotImplementedException();
            }

        private readonly TextWriter source;
        private readonly Context context = new Context();
        }
    }
using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    internal class LocalFile : IFileService
        {
        public String FileName { get; }
        public LocalFile(String filename) {
            FileName = filename;
            }

        public Byte[] ReadAllBytes()
            {
            return File.ReadAllBytes(FileName);
            }

        public Stream OpenRead()
            {
            return File.OpenRead(FileName);
            }

        /// <summary>
        /// Moves a local file to a new location with new name.
        /// </summary>
        /// <param name="target">The new path and name for the file.</param>
        public void MoveTo(String target)
            {
            MoveTo(target, false);
            }

        public void MoveTo(String target, Boolean overwrite) {
            if (File.Exists(target)) {
                if (!overwrite) { throw new IOException(); }
                File.Delete(target);
                }
            File.Move(FileName, target);
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return FileName;
            }
        }
    }
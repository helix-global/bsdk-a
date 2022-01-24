using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    internal class LocalFolder : IDirectoryService
        {
        public String Folder { get; }
        public LocalFolder(String folder)
            {
            Folder = folder;
            }

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption)
            {
            if (searchpattern == null) { throw new ArgumentNullException(nameof(searchpattern)); }
            foreach (var file in Directory.GetFiles(Folder,searchpattern,
                searchoption.HasFlag(DirectoryServiceSearchOptions.Recursive)
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly))
                {
                yield return new LocalFile(file);
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Folder;
            }
        }
    }
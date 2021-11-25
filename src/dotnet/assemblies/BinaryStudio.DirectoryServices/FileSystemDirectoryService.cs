using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    public class FileSystemDirectoryService : IDirectoryService
        {
        public String Folder { get; }
        public FileSystemDirectoryService(String folder) {
            if (folder == null) { throw new ArgumentNullException(nameof(folder)); }
            Folder = folder;
            }

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            if (searchpattern == null) { throw new ArgumentNullException(nameof(searchpattern)); }
            foreach (var file in Directory.GetFiles(Folder,searchpattern,
                searchoption.HasFlag(DirectoryServiceSearchOptions.Recursive)
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly))
                {
                yield return new FileItem(file);
                }
            }
        }
    }
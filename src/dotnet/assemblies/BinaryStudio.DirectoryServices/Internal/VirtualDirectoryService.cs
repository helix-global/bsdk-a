using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryStudio.DirectoryServices.Internal
    {
    internal class VirtualDirectoryService : IDirectoryService
        {
        public String[] Entries { get; }

        public VirtualDirectoryService(String[] entries) {
            Entries = entries;
            }

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            if (searchpattern == null) { throw new ArgumentNullException(nameof(searchpattern)); }
            searchpattern = searchpattern.Trim();
            var entries = Entries;
            if (entries != null) {
                if ((searchpattern == "*") || (searchpattern == "*.*")) {
                    foreach (var entry in entries) {
                        if (Path.GetFileNameWithoutExtension(entry).Contains("*")) {

                            }
                        else if (File.Exists(entry))
                            {
                            yield return new LocalFile(entry);
                            }
                        else if (Directory.Exists(entry))
                            {
                            foreach (var i in (new FileSystemDirectoryService(entry)).GetFiles(searchpattern, searchoption)) {
                                yield return i;
                                }
                            }
                        }
                    }
                }
            yield break;
            }
        }
    }
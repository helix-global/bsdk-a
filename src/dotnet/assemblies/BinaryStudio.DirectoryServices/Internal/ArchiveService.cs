using System;
using System.Collections.Generic;
using SharpCompress.Archives;

namespace BinaryStudio.DirectoryServices.Internal
    {
    internal class ArchiveService : IDirectoryService
        {
        public IArchive Archive { get; }
        public String FileName { get; }
        public ArchiveService(String filename, IArchive archive)
            {
            FileName = filename;
            Archive = archive;
            }

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            searchpattern = searchpattern ?? "*.*";
            foreach (var entry in Archive.Entries) {
                if (!entry.IsDirectory) {
                    if (PathUtils.IsMatch(searchpattern, entry.Key)) {
                        yield return new ArchiveEntryService(FileName, entry);
                        }
                    }
                }
            }
        }
    }
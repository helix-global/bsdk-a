using System;
using System.Collections.Generic;
using System.IO;
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

        private static Boolean IsMatchPart(String pattern, String value) {
            if (String.IsNullOrWhiteSpace(pattern)) { return true; }
            if ((pattern == "*") || (pattern == "*.*")) { return true; }
            return String.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);
            }

        private static Boolean IsMatch(String pattern, String filename)
            {
            if ((pattern == "*.*") || (pattern == "*")) { return true; }
            var pD = Path.GetDirectoryName(pattern);
            var pF = Path.GetFileNameWithoutExtension(pattern);
            var pE = Path.GetExtension(pattern);
            var iD = Path.GetDirectoryName(filename);
            var iF = Path.GetFileNameWithoutExtension(filename);
            var iE = Path.GetExtension(filename);
            return IsMatchPart(pD,iD)
                && IsMatchPart(pF,iF)
                && IsMatchPart(pE,iE);
            }

        public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            searchpattern = searchpattern ?? "*.*";
            foreach (var entry in Archive.Entries) {
                if (!entry.IsDirectory) {
                    if (IsMatch(searchpattern, entry.Key)) {
                        yield return new ArchiveEntryService(FileName, entry);
                        }
                    }
                }
            }
        }
    }
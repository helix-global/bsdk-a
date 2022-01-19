using System;
using System.IO;
using BinaryStudio.DirectoryServices.Internal;
using SharpCompress.Archives.Rar;

namespace BinaryStudio.DirectoryServices
    {
    public class DirectoryService
        {
        public static Object GetService(Object source, Type service)
            {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            if (source == null) { return null; }
            if (service == typeof(IDirectoryService)) {
                if (source is Uri uri) {
                    if (uri.Scheme == "file") { return new LocalFolder(uri.LocalPath); }
                    }
                else if (source is IFileService file) {
                    switch (Path.GetExtension(file.FileName).ToLower()) {
                        case ".rar":
                            {
                            return new ArchiveService(file.FileName, RarArchive.Open(file.FileName));
                            }
                        default:
                            {
                            return null;
                            }
                        }
                    }
                }
            else if (service == typeof(IFileService))
                {
                if (source is Uri uri) {
                    if (uri.Scheme == "file") { return new LocalFile(uri.LocalPath); }
                    }
                }
            else throw new ArgumentOutOfRangeException(nameof(service));
            return null;
            }

        public static T GetService<T>(Object source)
            {
            return (T)GetService(source, typeof(T));
            }

        public static Boolean GetService<T>(Object source, out T r)
            {
            r = (T)GetService(source, typeof(T));
            return r != null;
            }

        public static IDirectoryService GetService(String[] entries) {
            return new VirtualDirectoryService(entries);
            }
        }
    }
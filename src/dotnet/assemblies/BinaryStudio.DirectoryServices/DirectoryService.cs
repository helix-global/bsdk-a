using System;
using System.IO;
using System.Runtime.Serialization;
using BinaryStudio.DirectoryServices.Internal;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;

namespace BinaryStudio.DirectoryServices
    {
    public class DirectoryService
        {
        public static Object GetService(Object source, Type service)
            {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            if (source == null) { return null; }
            try
                {
                if (service == typeof(IDirectoryService)) {
                    if (source is IDirectoryService folder) { return folder; }
                    if (source is Uri uri) {
                        if (uri.Scheme == "file") {
                            if (Directory.Exists(uri.LocalPath)) return new LocalFolder(uri.LocalPath);
                            if (File.Exists(uri.LocalPath)) {
                                switch (Path.GetExtension(uri.LocalPath).ToLower()) {
                                    case ".rar": { return new ArchiveService(uri.LocalPath, RarArchive.Open(uri.LocalPath));      }
                                    case ".jar":
                                    case ".zip": { return new ArchiveService(uri.LocalPath, ZipArchive.Open(uri.LocalPath));      }
                                    case ".7z" : { return new ArchiveService(uri.LocalPath, SevenZipArchive.Open(uri.LocalPath)); }
                                    default:
                                        {
                                        return null;
                                        }
                                    }
                                }
                            }
                        }
                    else if (source is String stringvalue) {
                        if (Directory.Exists(stringvalue)) return new LocalFolder(stringvalue);
                        if (File.Exists(stringvalue)) {
                            switch (Path.GetExtension(stringvalue).ToLower()) {
                                case ".rar": { return new ArchiveService(stringvalue, RarArchive.Open(stringvalue));      }
                                case ".jar":
                                case ".zip": { return new ArchiveService(stringvalue, ZipArchive.Open(stringvalue));      }
                                case ".7z" : { return new ArchiveService(stringvalue, SevenZipArchive.Open(stringvalue)); }
                                default:
                                    {
                                    return null;
                                    }
                                }
                            }
                        }
                    else if (source is IFileService file) {
                        var filename = file.FileName;
                        if (Uri.TryCreate($"file://{filename}", UriKind.Absolute, out uri)) { return GetService(uri, service); }
                        if (Uri.TryCreate($"file://{filename}", UriKind.Relative, out uri)) { return GetService(uri, service); }
                        return GetService(filename, service);
                        }
                    return (source as IServiceProvider)?.GetService(service);
                    }
                else if (service == typeof(IFileService))
                    {
                    if (source is IFileService file) { return file; }
                    if (source is Uri uri) {
                        if (uri.Scheme == "file") { return new LocalFile(uri.LocalPath); }
                        }
                    return (source as IServiceProvider)?.GetService(service);
                    }
                else throw new ArgumentOutOfRangeException(nameof(service));
                return (source as IServiceProvider)?.GetService(service);
                }
            catch (Exception e)
                {
                e.Data["ServiceType"] = service.FullName;
                if (source is ISerializable)
                    {
                    e.Data["Source"] = source;
                    }
                else
                    {
                    e.Data["Source"] = source.ToString();
                    }
                throw;
                }
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
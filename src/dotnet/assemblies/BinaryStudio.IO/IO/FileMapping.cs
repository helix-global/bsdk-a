using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using BinaryStudio.PlatformComponents.Win32;
using Microsoft.Win32.SafeHandles;

namespace BinaryStudio.IO
    {
    public class FileMapping : IDisposable
        {
        private Boolean Disposed;
        public Int64 Size { get; private set; }
        public String FileName { get; }
        #if UBUNTU_16_4
        internal SafeHandle Mapping { get;private set; }
        #else
        internal FileMappingHandle Mapping { get; private set; }
        private SafeFileHandle FileHandle { get;set; }
        #endif

        public FileMapping(String filename)
            {
            if (!File.Exists(filename)) { throw new FileNotFoundException(filename); }
            FileName = filename;
            var security = new SecurityAttributes();
            try
                {
                #if UBUNTU_16_4
                var handle = Open(filename, OpenFlags.O_RDONLY, S_IROTH | S_IRUSR | S_IRGRP);
                if (handle.IsInvalid) { throw PlatformException.Create((PosixError)Marshal.GetLastWin32Error()); }
                FileStatus output;
                if (FStat(handle, out output) != 0)
                    {
                    handle.Dispose();
                    throw PlatformException.Create((PosixError)Marshal.GetLastWin32Error());
                    }
                Mapping = handle;
                Size = output.Size;
                #else
                FileHandle = CreateFile(filename, FileGenericAccess.Read, FileShare.Read|FileShare.Delete, null,
                    FileMode.Open, flags: FILE_ATTRIBUTE_TEMPORARY, templatefile: IntPtr.Zero);
                if (FileHandle.IsInvalid) { throw new HResultException(Marshal.GetLastWin32Error()); }
                var i = default(LargeInteger);
                if (!GetFileSizeEx(FileHandle, ref i)) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
                var sz = i.QuadPart;
                if (sz == 0L) { throw new IOException($"{new Uri(filename)}"); }
                Mapping = CreateFileMapping(FileHandle, security, PageProtection.ReadOnly, 0u, 0u, null);
                if (Mapping.IsInvalid) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
                Size = sz;
                #endif
                }
            finally
                {
                security.Release();
                }
            }

        public FileMapping(SafeFileHandle file)
            {
            if (file == null) { throw new ArgumentNullException(nameof(file)); }
            if (file.IsInvalid || file.IsClosed) { throw new ArgumentOutOfRangeException(nameof(file)); }
            var security = new SecurityAttributes();
            try
                {
                var i = default(LargeInteger);
                if (!GetFileSizeEx(file, ref i)) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
                var sz = i.QuadPart;
                if (sz == 0L) { throw new IOException(); }
                Mapping = CreateFileMapping(file, security, PageProtection.ReadOnly, 0u, 0u, null);
                if (Mapping.IsInvalid) { throw new Win32Exception(Marshal.GetLastWin32Error()); }
                Size = sz;
                }
            finally
                {
                security.Release();
                }
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (!Disposed) {
                if (Mapping != null) {
                    Mapping.Dispose();
                    Mapping = null;
                    }
                if (FileHandle != null) {
                    FileHandle.Dispose();
                    FileHandle = null;
                    }
                Size = 0;
                Disposed = true;
                }
            }
        #endregion
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~FileMapping()
            {
            Dispose(false);
            }
        #endregion

        #if UBUNTU_16_4
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TIMESPEC32
            {
            public readonly Int32 Seconds;
            public readonly Int32 NanoSeconds;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TIMESPEC64
            {
            public readonly Int64 Seconds;
            public readonly Int64 NanoSeconds;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct STAT32
            {
            [FieldOffset( 0)]  public readonly Int64 Device;
            [FieldOffset( 8)]  public readonly Int64 FileSerialNumber;
            [FieldOffset(16)]  public readonly Int64 LinkCount;
            [FieldOffset(24)]  public readonly Int32 FileMode;
            [FieldOffset(28)]  public readonly Int32 UserId;
            [FieldOffset(32)]  public readonly Int32 GroupId;
            [FieldOffset(40)]  public readonly Int64 DeviceNumber;
            [FieldOffset(48)]  public readonly Int64 FileSize;
            [FieldOffset(56)]  public readonly Int64 OptimalBlockSize;
            [FieldOffset(64)]  public readonly Int64 AllocatedBlocks;
            [FieldOffset(72)]  public readonly TIMESPEC32 LastAccess;
            [FieldOffset(88)]  public readonly TIMESPEC32 LastModification;
            [FieldOffset(104)] public readonly TIMESPEC32 LastStatusChange;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 144)]
        private struct STAT64
            {
            [FieldOffset( 0)] public readonly Int64 Device;
            [FieldOffset(16)] public readonly Int32 FileMode;
            [FieldOffset(20)] public readonly Int32 LinkCount;
            [FieldOffset(24)] public readonly Int32 UserId;
            [FieldOffset(28)] public readonly Int32 GroupId;
            [FieldOffset(32)] public readonly Int64 DeviceNumber;
            [FieldOffset(44)] public readonly Int64 FileSize;
            [FieldOffset(52)] public readonly Int32 OptimalBlockSize;
            [FieldOffset(56)] public readonly Int64 AllocatedBlocks;
            [FieldOffset(64)] public readonly TIMESPEC64 LastAccess;
            [FieldOffset(72)] public readonly TIMESPEC64 LastModification;
            [FieldOffset(80)] public readonly TIMESPEC64 LastStatusChange;
            [FieldOffset(88)] public readonly Int64 FileSerialNumber;
            }

        [Flags]
        internal enum OpenFlags
            {
            O_RDONLY    = 0x000,
            O_WRONLY    = 0x001,
            O_RDWR      = 0x002,
            O_CLOEXEC   = 0x010,
            O_CREAT     = 0x020,
            O_EXCL      = 0x040,
            O_TRUNC     = 0x080,
            O_SYNC      = 0x100
            }

        [Flags]
        internal enum FileStatusFlags
            {
            None = 0x0,
            HasBirthTime = 0x1
            }

        internal struct FileStatus
            {
            internal FileStatusFlags Flags;
            internal int Mode;
            internal uint Uid;
            internal uint Gid;
            internal long Size;
            internal long ATime;
            internal long ATimeNsec;
            internal long MTime;
            internal long MTimeNsec;
            internal long CTime;
            internal long CTimeNsec;
            internal long BirthTime;
            internal long BirthTimeNsec;
            internal long Dev;
            internal long Ino;
            }

        [DllImport("System.Native", EntryPoint = "SystemNative_Open", SetLastError = true)] private static extern SafeFileHandle Open(string filename, OpenFlags flags, int mode);
        [DllImport("System.Native", EntryPoint = "SystemNative_FStat2", SetLastError = true)] internal static extern int FStat(SafeFileHandle fd, out FileStatus output);
        [DllImport("c", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.Cdecl, EntryPoint ="__fxstat64")] private static extern Int32 Stat(Int32 version, Int32 handle, ref STAT32 r);
        [DllImport("c", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.Cdecl, EntryPoint ="__fxstat64")] private static extern Int32 Stat(Int32 version, Int32 handle, ref STAT64 r);

        private static void Validate(Int32 r) {
            if (r == -1) {
                var e = Marshal.GetLastWin32Error();
                throw PlatformException.Create((PosixError)e);
                }
            }

        private static Int64 GetFileSize(Int32 handle)
            {
            if (IntPtr.Size == sizeof(Int32))
                {
                var r = new STAT32();
                Validate(Stat(3, handle, ref r));
                return r.FileSize;
                }
            else
                {
                var r = new STAT64();
                Validate(Stat(3, handle, ref r));
                return r.FileSize;
                }
            }
        private const Int32 S_IRWXU = 0x00700;
        private const Int32 S_IRUSR = 0x00400;
        private const Int32 S_IWUSR = 0x00200;
        private const Int32 S_IXUSR = 0x00100;
        private const Int32 S_IRWXG = 0x00070;
        private const Int32 S_IRGRP = 0x00040;
        private const Int32 S_IWGRP = 0x00020;
        private const Int32 S_IXGRP = 0x00010;
        private const Int32 S_IRWXO = 0x00007;
        private const Int32 S_IROTH = 0x00004;
        private const Int32 S_IWOTH = 0x00002;
        private const Int32 S_IXOTH = 0x00001;
        #else
        [DllImport("kernel32.dll", SetLastError = true)] [SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern Boolean GetFileSizeEx(SafeFileHandle file, ref LargeInteger filesize);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern SafeFileHandle CreateFile(String filename, FileGenericAccess desiredaccess, FileShare dwShareMode, SecurityAttributes security, FileMode creationdisposition, Int32 flags, IntPtr templatefile);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)] [SecurityCritical, SuppressUnmanagedCodeSecurity] internal static extern FileMappingHandle CreateFileMapping(SafeFileHandle file, SecurityAttributes filemappingattributes, PageProtection protection, UInt32 maximumsizehigh, UInt32 maximumsizelow, String name);
        [DllImport("kernel32.dll", SetLastError = true)] [SecurityCritical, SuppressUnmanagedCodeSecurity] private static extern ViewOfFileHandle MapViewOfFile(FileMappingHandle filemappingobject, FileMappingAccess desiredaccess, UInt32 fileoffsethigh, UInt32 fileoffsetlow, IntPtr numberofbytestomap);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] private static extern Boolean DeleteFileW([MarshalAs(UnmanagedType.LPWStr)] String FileName);
        #endif

        private const Int32 FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
        private const Int32 FILE_ATTRIBUTE_TEMPORARY  = 0x00000100;
        }
    }

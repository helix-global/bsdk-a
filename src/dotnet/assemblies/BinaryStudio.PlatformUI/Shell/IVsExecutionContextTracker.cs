using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformUI.Shell
    {
    [CompilerGenerated, Guid("5C7E7029-A00C-4F57-BE15-6AC5D43E78DC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeIdentifier]
    [ComImport]
    public interface IVsExecutionContextTracker
        {
        [MethodImpl(MethodImplOptions.InternalCall)]
        void SetContextElement([In] Guid contextTypeGuid, [In] Guid contextElementGuid);

        [MethodImpl(MethodImplOptions.InternalCall)]
        Guid SetAndGetContextElement([In] Guid contextTypeGuid, [In] Guid contextElementGuid);

        [MethodImpl(MethodImplOptions.InternalCall)]
        Guid GetContextElement([In] Guid contextTypeGuid);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void PushContext([In] UInt32 contextCookie);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void PopContext([In] UInt32 contextCookie);

        [MethodImpl(MethodImplOptions.InternalCall)]
        UInt32 GetCurrentContext();

        [MethodImpl(MethodImplOptions.InternalCall)]
        void ReleaseContext([In] UInt32 contextCookie);

        [MethodImpl(MethodImplOptions.InternalCall)]
        void PushContextEx([In] UInt32 contextCookie, [In] Boolean fDontTrackAsyncWork);
        }
    }
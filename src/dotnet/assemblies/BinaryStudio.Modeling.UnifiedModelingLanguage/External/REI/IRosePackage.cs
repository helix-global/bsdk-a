using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(4096)]
    [InterfaceType(2)]
    [Guid("47D975C1-8A8D-11D0-A214-444553540000")]
    [ComImport]
    public interface IRosePackage : IRoseControllableUnit
        {
        [DispId(621)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean IsRootPackage();
        }
    }
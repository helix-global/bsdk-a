﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("D5352FC2-346C-11D1-883B-3C8B00C10000")]
    [TypeLibType(4096)]
    [InterfaceType(2)]
    [ComImport]
    public interface IRoseAddInManager : IRoseObject
        {
        [DispId(12529)]
        RoseAddInCollection AddIns { [DispId(12529), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12529), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12692)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int32 DisableEvents(Int32 theEvents);

        [DispId(12693)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int32 EnableEvents(Int32 theEvents);
        }
    }

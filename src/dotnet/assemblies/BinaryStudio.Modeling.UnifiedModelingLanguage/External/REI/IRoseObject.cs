﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <img src="data:image/png;base64, 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"/>
    /// </remarks>
    [Guid("7D8474B2-2C33-11D0-BBDA-00A024C67143")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IRoseObject
    {
        [DispId(12668)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String IdentifyClass();

        [DispId(12669)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);
    }
}
﻿// Decompiled with JetBrains decompiler
// Type: RationalRose.IRoseInstanceViewCollection
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [TypeLibType(4096)]
  [InterfaceType(2)]
  [Guid("C640C864-F2D3-11D0-883A-3C8B00C10000")]
  [ComImport]
  public interface IRoseInstanceViewCollection
  {
    [DispId(202)]
    Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseInstanceView GetAt(Int16 Index);

    [DispId(204)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean Exists([MarshalAs(UnmanagedType.Interface)] RoseInstanceView pObject);

    [DispId(205)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(206)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(207)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 IndexOf([MarshalAs(UnmanagedType.Interface)] RoseInstanceView theObject);

    [DispId(208)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Add([MarshalAs(UnmanagedType.Interface)] RoseInstanceView theObject);

    [DispId(209)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddCollection([MarshalAs(UnmanagedType.Interface)] RoseInstanceViewCollection theCollection);

    [DispId(210)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Remove([MarshalAs(UnmanagedType.Interface)] RoseInstanceView theObject);

    [DispId(211)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveAll();

    [DispId(212)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseInstanceView GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(213)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseInstanceView GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
  }
}
// Decompiled with JetBrains decompiler
// Type: RationalRose.IRoseProcessorCollection
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [Guid("97B3835C-A4E3-11D0-BFF0-00AA003DEF5B")]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [TypeLibType(4096)]
  [ComImport]
  public interface IRoseProcessorCollection
  {
    [DispId(202)]
    Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseProcessor GetAt(Int16 Index);

    [DispId(204)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean Exists([MarshalAs(UnmanagedType.Interface)] RoseProcessor pObject);

    [DispId(205)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(206)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(207)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 IndexOf([MarshalAs(UnmanagedType.Interface)] RoseProcessor theObject);

    [DispId(208)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Add([MarshalAs(UnmanagedType.Interface)] RoseProcessor theObject);

    [DispId(209)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddCollection([MarshalAs(UnmanagedType.Interface)] RoseProcessorCollection theCollection);

    [DispId(210)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Remove([MarshalAs(UnmanagedType.Interface)] RoseProcessor theObject);

    [DispId(211)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveAll();

    [DispId(212)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseProcessor GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(213)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseProcessor GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
  }
}

// Decompiled with JetBrains decompiler
// Type: RationalRose.RoseStateMachineCollectionClass
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [ClassInterface(ClassInterfaceType.None)]
  [Guid("BA376EEB-A44E-11D0-BC02-00A024C67143")]
  [TypeLibType(2)]
  [ComImport]
  public class RoseStateMachineCollectionClass : IRoseStateMachineCollection, RoseStateMachineCollection
  {
    //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    //public extern RoseStateMachineCollectionClass();

    [DispId(202)]
    public virtual extern Int16 Count { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseStateMachine GetAt(Int16 Index);

    [DispId(204)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Exists([MarshalAs(UnmanagedType.Interface)] RoseStateMachine pObject);

    [DispId(205)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int16 FindFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(206)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int16 FindNext(Int16 iCurID, [MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(207)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int16 IndexOf([MarshalAs(UnmanagedType.Interface)] RoseStateMachine theObject);

    [DispId(208)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] RoseStateMachine theObject);

    [DispId(209)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void AddCollection([MarshalAs(UnmanagedType.Interface)] RoseStateMachineCollection theCollection);

    [DispId(210)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Remove([MarshalAs(UnmanagedType.Interface)] RoseStateMachine theObject);

    [DispId(211)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RemoveAll();

    [DispId(212)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseStateMachine GetFirst([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(213)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseStateMachine GetWithUniqueID([MarshalAs(UnmanagedType.BStr)] String UniqueID);
  }
}

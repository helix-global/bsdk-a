// Decompiled with JetBrains decompiler
// Type: RationalRose.RoseAddInManagerClass
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [TypeLibType(2)]
  [ClassInterface(ClassInterfaceType.None)]
  [Guid("D5352FC3-346C-11D1-883B-3C8B00C10000")]
  [ComImport]
  public class RoseAddInManagerClass : IRoseAddInManager, RoseAddInManager
  {
    //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    //public extern RoseAddInManagerClass();

    [DispId(12529)]
    public virtual extern RoseAddInCollection AddIns { [DispId(12529), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12529), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);

    [DispId(12692)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int32 DisableEvents(Int32 theEvents);

    [DispId(12693)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int32 EnableEvents(Int32 theEvents);
  }
}

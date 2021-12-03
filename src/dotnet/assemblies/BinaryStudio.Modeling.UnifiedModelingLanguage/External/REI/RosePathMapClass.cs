// Decompiled with JetBrains decompiler
// Type: RationalRose.RosePathMapClass
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [ClassInterface(ClassInterfaceType.None)]
  [TypeLibType(TypeLibTypeFlags.FCanCreate)]
  [Guid("97B38395-A4E3-11D0-BFF0-00AA003DEF5B")]
  [ComImport]
  public class RosePathMapClass : IRosePathMap, RosePathMap
  {
    //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    //public extern RosePathMapClass();

    [DispId(50)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteEntry([MarshalAs(UnmanagedType.BStr)] String Symbol);

    [DispId(51)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetActualPath([MarshalAs(UnmanagedType.BStr)] String VirtualPath);

    [DispId(52)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetVirtualPath([MarshalAs(UnmanagedType.BStr)] String ActualPath);

    [DispId(53)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean HasEntry([MarshalAs(UnmanagedType.BStr)] String Symbol);

    [DispId(54)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean AddEntry([MarshalAs(UnmanagedType.BStr)] String Symbol, [MarshalAs(UnmanagedType.BStr)] String Path, [MarshalAs(UnmanagedType.BStr)] String Comment);

    [DispId(12674)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetActualPathWithContext([MarshalAs(UnmanagedType.BStr)] String VirtualPath, [MarshalAs(UnmanagedType.BStr)] String Context);

    [DispId(12675)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetVirtualPathWithContext([MarshalAs(UnmanagedType.BStr)] String ActualPath, [MarshalAs(UnmanagedType.BStr)] String Context);
  }
}

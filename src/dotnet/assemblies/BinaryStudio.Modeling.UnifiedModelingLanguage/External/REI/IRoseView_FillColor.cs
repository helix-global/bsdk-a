// Decompiled with JetBrains decompiler
// Type: RationalRose.IRoseView_FillColor
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [TypeLibType(4096)]
  [Guid("CE5BE563-0380-11D1-BC11-00A024C67143")]
  [InterfaceType(2)]
  [ComImport]
  public interface IRoseView_FillColor
  {
    [DispId(12493)]
    Int16 Red { [DispId(12493), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12493), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12494)]
    Boolean Transparent { [DispId(12494), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12494), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12495)]
    Int16 Blue { [DispId(12495), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12495), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12496)]
    Int16 Green { [DispId(12496), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12496), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);
  }
}

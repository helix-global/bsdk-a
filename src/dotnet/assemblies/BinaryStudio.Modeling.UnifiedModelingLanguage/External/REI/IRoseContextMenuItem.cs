// Decompiled with JetBrains decompiler
// Type: RationalRose.IRoseContextMenuItem
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [Guid("EE0B16E0-FF91-11D1-9FAD-0060975306FE")]
  [InterfaceType(2)]
  [TypeLibType(4096)]
  [ComImport]
  public interface IRoseContextMenuItem
  {
    [DispId(12682)]
    String Caption { [DispId(12682), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12682), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12683)]
    String InternalName { [DispId(12683), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12683), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12686)]
    Int16 MenuID { [DispId(12686), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12686), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12687)]
    Int16 MenuState { [DispId(12687), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12687), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);
  }
}

// Decompiled with JetBrains decompiler
// Type: RationalRose.RoseMessageClass
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
  [Guid("8665227C-EBF7-11D0-BC10-00A024C67143")]
  [ComImport]
  public class RoseMessageClass : IRoseMessage, RoseMessage
  {
    //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    //public extern RoseMessageClass();

    [DispId(100)]
    public virtual extern String Name { [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    public virtual extern String Documentation { [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(212)]
    public virtual extern String Stereotype { [DispId(212), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(212), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(213)]
    public virtual extern RoseExternalDocumentCollection ExternalDocuments { [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12523)]
    public virtual extern Object Application { [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12524)]
    public virtual extern RoseModel Model { [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12554)]
    public virtual extern String LocalizedStereotype { [DispId(12554), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12554), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12652)]
    public virtual extern Int16 Synchronization { [DispId(12652), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12652), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12653)]
    public virtual extern Int16 Frequency { [DispId(12653), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12653), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12790)]
    public virtual extern RoseStateMachineOwner StateMachineOwner { [DispId(12790), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12790), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(102)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetUniqueID();

    [DispId(109)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetCurrentPropertySetName([MarshalAs(UnmanagedType.BStr)] String ToolName);

    [DispId(110)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean OverrideProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName, [MarshalAs(UnmanagedType.BStr)] String theValue);

    [DispId(111)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean InheritProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(119)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetPropertyValue([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(120)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetDefaultPropertyValue([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(121)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseProperty FindProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(122)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RosePropertyCollection GetAllProperties();

    [DispId(123)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RosePropertyCollection GetToolProperties([MarshalAs(UnmanagedType.BStr)] String theToolName);

    [DispId(124)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsOverriddenProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(125)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsDefaultProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(126)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseProperty FindDefaultProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(127)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean CreateProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName, [MarshalAs(UnmanagedType.BStr)] String theValue, [MarshalAs(UnmanagedType.BStr)] String theType);

    [DispId(128)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetPropertyClassName();

    [DispId(129)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseStringCollection GetDefaultSetNames([MarshalAs(UnmanagedType.BStr)] String ToolName);

    [DispId(130)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseStringCollection GetToolNames();

    [DispId(131)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean SetCurrentPropertySetName([MarshalAs(UnmanagedType.BStr)] String ToolName, [MarshalAs(UnmanagedType.BStr)] String SetName);

    [DispId(207)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseItem GetRoseItem();

    [DispId(214)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseExternalDocument AddExternalDocument([MarshalAs(UnmanagedType.BStr)] String szName, Int16 iType);

    [DispId(215)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteExternalDocument([MarshalAs(UnmanagedType.Interface)] RoseExternalDocument pIDispatch);

    [DispId(216)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean OpenSpecification();

    [DispId(412)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseObjectInstance GetSenderObject();

    [DispId(413)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseObjectInstance GetReceiverObject();

    [DispId(414)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsMessageToSelf();

    [DispId(415)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsOperation();

    [DispId(416)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseOperation GetOperation();

    [DispId(417)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseLink GetLink();

    [DispId(12555)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetQualifiedName();

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);

    [DispId(12728)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean OpenCustomSpecification();

    [DispId(12820)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean RenderIconToClipboard();

    [DispId(12824)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int16 GetIconIndex();

    [DispId(12825)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetSequenceInformation();

    [DispId(12872)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseItem GetContext();

    [DispId(12886)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RosePropertyCollection GetUserOverriddenProperties([MarshalAs(UnmanagedType.BStr)] String theToolName);
  }
}

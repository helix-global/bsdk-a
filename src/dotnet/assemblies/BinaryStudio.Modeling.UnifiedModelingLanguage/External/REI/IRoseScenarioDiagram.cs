﻿// Decompiled with JetBrains decompiler
// Type: RationalRose.IRoseScenarioDiagram
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [Guid("F819833A-FC55-11CF-BBD3-00A024C67143")]
  [TypeLibType(4096)]
  [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
  [ComImport]
  public interface IRoseScenarioDiagram
  {
    [DispId(100)]
    String Name { [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(202)]
    RoseItemViewCollection ItemViews { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    Boolean Visible { [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(208)]
    RoseItemCollection Items { [DispId(208), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(208), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(213)]
    RoseExternalDocumentCollection ExternalDocuments { [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(423)]
    RoseInstanceViewCollection InstanceViews { [DispId(423), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(423), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12523)]
    Object Application { [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12524)]
    RoseModel Model { [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12656)]
    String Documentation { [DispId(12656), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12656), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12690)]
    Int16 ZoomFactor { [DispId(12690), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12690), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(102)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetUniqueID();

    [DispId(109)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetCurrentPropertySetName([MarshalAs(UnmanagedType.BStr)] String ToolName);

    [DispId(110)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean OverrideProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName, [MarshalAs(UnmanagedType.BStr)] String theValue);

    [DispId(111)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean InheritProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(119)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetPropertyValue([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(120)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetDefaultPropertyValue([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(121)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseProperty FindProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(122)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RosePropertyCollection GetAllProperties();

    [DispId(123)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RosePropertyCollection GetToolProperties([MarshalAs(UnmanagedType.BStr)] String theToolName);

    [DispId(124)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsOverriddenProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(125)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsDefaultProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(126)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseProperty FindDefaultProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName);

    [DispId(127)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean CreateProperty([MarshalAs(UnmanagedType.BStr)] String theToolName, [MarshalAs(UnmanagedType.BStr)] String thePropName, [MarshalAs(UnmanagedType.BStr)] String theValue, [MarshalAs(UnmanagedType.BStr)] String theType);

    [DispId(128)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetPropertyClassName();

    [DispId(129)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseStringCollection GetDefaultSetNames([MarshalAs(UnmanagedType.BStr)] String ToolName);

    [DispId(130)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseStringCollection GetToolNames();

    [DispId(131)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean SetCurrentPropertySetName([MarshalAs(UnmanagedType.BStr)] String ToolName, [MarshalAs(UnmanagedType.BStr)] String SetName);

    [DispId(204)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Layout();

    [DispId(205)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Invalidate();

    [DispId(206)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Update();

    [DispId(207)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseItemView GetViewFrom([MarshalAs(UnmanagedType.Interface)] RoseItem theItem);

    [DispId(209)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsActive();

    [DispId(210)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean Exists([MarshalAs(UnmanagedType.Interface)] RoseItem theItem);

    [DispId(211)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Activate();

    [DispId(214)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseExternalDocument AddExternalDocument([MarshalAs(UnmanagedType.BStr)] String szName, Int16 iType);

    [DispId(215)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean DeleteExternalDocument([MarshalAs(UnmanagedType.Interface)] RoseExternalDocument pIDispatch);

    [DispId(217)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Render([MarshalAs(UnmanagedType.BStr)] String FileName);

    [DispId(218)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseNoteView AddNoteView([MarshalAs(UnmanagedType.BStr)] String szNoteText, Int16 nType);

    [DispId(219)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean RemoveNoteView([MarshalAs(UnmanagedType.Interface)] RoseNoteView pIDispNoteView);

    [DispId(220)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseNoteViewCollection GetNoteViews();

    [DispId(221)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RenderEnhanced([MarshalAs(UnmanagedType.BStr)] String FileName);

    [DispId(222)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RenderToClipboard();

    [DispId(223)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RenderEnhancedToClipboard();

    [DispId(411)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseObjectInstanceCollection GetObjects();

    [DispId(412)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseObjectInstanceCollection GetSelectedObjects();

    [DispId(413)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseMessageCollection GetMessages();

    [DispId(414)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseMessageCollection GetSelectedMessages();

    [DispId(416)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseMessage CreateMessage([MarshalAs(UnmanagedType.BStr)] String theName, [MarshalAs(UnmanagedType.Interface)] RoseObjectInstance theSender, [MarshalAs(UnmanagedType.Interface)] RoseObjectInstance theReceiver, Int16 theSequence);

    [DispId(417)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseLinkCollection GetSelectedLinks();

    [DispId(418)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 GetDiagramType();

    [DispId(419)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseInstanceView AddInstanceView([MarshalAs(UnmanagedType.Interface)] RoseObjectInstance theInstance, Boolean AsClassInstance);

    [DispId(420)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean RemoveInstanceView([MarshalAs(UnmanagedType.Interface)] RoseInstanceView theView);

    [DispId(421)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean DeleteInstance([MarshalAs(UnmanagedType.Interface)] RoseObjectInstance theInstance);

    [DispId(422)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseObjectInstance AddInstance([MarshalAs(UnmanagedType.BStr)] String theName, [MarshalAs(UnmanagedType.BStr)] String theClassName);

    [DispId(12525)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseItemCollection GetSelectedItems();

    [DispId(12555)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String GetQualifiedName();

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);

    [DispId(12787)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean AddRelationView([MarshalAs(UnmanagedType.Interface)] RoseRelation theRelation);

    [DispId(12820)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean RenderIconToClipboard();

    [DispId(12823)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RoseItem GetParentContext();

    [DispId(12824)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Int16 GetIconIndex();

    [DispId(12832)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean RemoveItemView([MarshalAs(UnmanagedType.Interface)] RoseItemView theItemView);

    [DispId(12861)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean CenterDiagramToView([MarshalAs(UnmanagedType.Interface)] RoseItemView theView);

    [DispId(12862)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AutosizeAll();

    [DispId(12873)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Boolean LayoutSelectedViews(Boolean StackClasses, Int16 ClassesPerRow, Int16 DistX, Int16 DistY, Int16 UpperLeftX, Int16 UpperLeftY);

    [DispId(12886)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    RosePropertyCollection GetUserOverriddenProperties([MarshalAs(UnmanagedType.BStr)] String theToolName);
  }
}

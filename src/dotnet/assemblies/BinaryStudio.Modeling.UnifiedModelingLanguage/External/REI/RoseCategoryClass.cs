// Decompiled with JetBrains decompiler
// Type: RationalRose.RoseCategoryClass
// Assembly: Interop.RationalRose, Version=4.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E0CF27B3-8D49-4F96-A07A-EC194F467799
// Assembly location: C:\Users\maistrenko\Documents\Visual Studio 2017\Projects\BinaryStudio\BinaryStudio.Modeling.UnifiedModelingLanguage\obj\Debug\Interop.RationalRose.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
{
  [TypeLibType(TypeLibTypeFlags.FCanCreate)]
  [ClassInterface(ClassInterfaceType.None)]
  [Guid("4782FBA9-ECD5-11D0-BFF0-00AA003DEF5B")]
  [ComImport]
  public class RoseCategoryClass : IREICOMCategory, REICOMCategory
  {
    //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    //public extern RoseCategoryClass();

    [DispId(100)]
    public virtual extern String Name { [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(100), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(203)]
    public virtual extern String Documentation { [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(203), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(212)]
    public virtual extern String Stereotype { [DispId(212), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(212), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(213)]
    public virtual extern REICOMExternalDocumentCollection ExternalDocuments { [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(213), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(412)]
    public virtual extern Boolean Global { [DispId(412), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(412), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(413)]
    public virtual extern IREICOMClassCollection Classes { [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(414)]
    public virtual extern IREICOMCategoryCollection Categories { [DispId(414), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(414), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(415)]
    public virtual extern REICOMCategory ParentCategory { [DispId(415), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(415), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(416)]
    public virtual extern REICOMClassDiagramCollection ClassDiagrams { [DispId(416), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(416), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(417)]
    public virtual extern REICOMAssociationCollection Associations { [DispId(417), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(417), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(418)]
    public virtual extern RoseScenarioDiagramCollection ScenarioDiagrams { [DispId(418), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(418), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(446)]
    public virtual extern RoseUseCaseCollection UseCases { [DispId(446), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(446), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12523)]
    public virtual extern Object Application { [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12523), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12524)]
    public virtual extern RoseModel Model { [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12524), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12554)]
    public virtual extern String LocalizedStereotype { [DispId(12554), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12554), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12790)]
    public virtual extern RoseStateMachineOwner StateMachineOwner { [DispId(12790), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12790), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(12881)]
    public virtual extern Int32 PetalVersion { [DispId(12881), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12881), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

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
    public virtual extern REICOMItem GetRoseItem();

    [DispId(214)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMExternalDocument AddExternalDocument([MarshalAs(UnmanagedType.BStr)] String szName, Int16 iType);

    [DispId(215)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteExternalDocument([MarshalAs(UnmanagedType.Interface)] REICOMExternalDocument pIDispatch);

    [DispId(216)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean OpenSpecification();

    [DispId(419)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClassCollection GetAllClasses();

    [DispId(420)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMCategoryCollection GetAllCategories();

    [DispId(421)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClass AddClass([MarshalAs(UnmanagedType.BStr)] String theName);

    [DispId(422)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClassDiagram AddClassDiagram([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(423)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMCategory AddCategory([MarshalAs(UnmanagedType.BStr)] String theName);

    [DispId(424)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RelocateClass([MarshalAs(UnmanagedType.Interface)] REICOMClass theClass);

    [DispId(425)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RelocateCategory([MarshalAs(UnmanagedType.Interface)] REICOMCategory theCategory);

    [DispId(426)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RelocateClassDiagram([MarshalAs(UnmanagedType.Interface)] REICOMClassDiagram theClsDiagram);

    [DispId(427)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteCategory([MarshalAs(UnmanagedType.Interface)] REICOMCategory theCategory);

    [DispId(428)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteClass([MarshalAs(UnmanagedType.Interface)] REICOMClass theClass);

    [DispId(429)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteClassDiagram([MarshalAs(UnmanagedType.Interface)] REICOMClassDiagram theClassDiagram);

    [DispId(430)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseScenarioDiagram AddScenarioDiagram([MarshalAs(UnmanagedType.BStr)] String Name, Int16 Type);

    [DispId(431)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteScenarioDiagram([MarshalAs(UnmanagedType.Interface)] RoseScenarioDiagram theScenarioDiagram);

    [DispId(432)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RelocateScenarioDiagram([MarshalAs(UnmanagedType.Interface)] RoseScenarioDiagram theScenarioDiagram);

    [DispId(433)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseSubsystem GetAssignedSubsystem();

    [DispId(434)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetAssignedSubsystem([MarshalAs(UnmanagedType.Interface)] RoseSubsystem newValue);

    [DispId(435)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean HasAssignedSubsystem();

    [DispId(436)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteUseCase([MarshalAs(UnmanagedType.Interface)] RoseUseCase theUseCase);

    [DispId(437)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean TopLevel();

    [DispId(447)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseUseCaseCollection GetAllUseCases();

    [DispId(448)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RoseUseCase AddUseCase([MarshalAs(UnmanagedType.BStr)] String szName);

    [DispId(621)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsRootPackage();

    [DispId(12433)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsControlled();

    [DispId(12434)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Control([MarshalAs(UnmanagedType.BStr)] String Path);

    [DispId(12435)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsLoaded();

    [DispId(12436)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Load();

    [DispId(12438)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsModifiable();

    [DispId(12439)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Unload();

    [DispId(12440)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Modifiable(Boolean Modifiable);

    [DispId(12441)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetFileName();

    [DispId(12442)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Save();

    [DispId(12443)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean SaveAs([MarshalAs(UnmanagedType.BStr)] String Path);

    [DispId(12555)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String GetQualifiedName();

    [DispId(12654)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsModified();

    [DispId(12655)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean Uncontrol();

    [DispId(12659)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMCategoryDependency AddCategoryDependency([MarshalAs(UnmanagedType.BStr)] String theName, [MarshalAs(UnmanagedType.BStr)] String theSupplierCategoryName);

    [DispId(12660)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMCategoryDependencyCollection GetCategoryDependencies();

    [DispId(12661)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean DeleteCategoryDependency([MarshalAs(UnmanagedType.Interface)] REICOMCategoryDependency theDependency);

    [DispId(12668)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern String IdentifyClass();

    [DispId(12669)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsClass([MarshalAs(UnmanagedType.BStr)] String theClassName);

    [DispId(12677)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RelocateAssociation([MarshalAs(UnmanagedType.Interface)] REICOMAssociation theAssociation);

    [DispId(12701)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Refresh();

    [DispId(12702)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMControllableUnitCollection GetSubUnitItems();

    [DispId(12703)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean IsLocked();

    [DispId(12704)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean NeedsRefreshing();

    [DispId(12707)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMControllableUnitCollection GetAllSubUnitItems();

    [DispId(12708)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Lock();

    [DispId(12709)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void Unlock();

    [DispId(12728)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean OpenCustomSpecification();

    [DispId(12820)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Boolean RenderIconToClipboard();

    [DispId(12824)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern Int16 GetIconIndex();

    [DispId(12833)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClassDiagram AddDataModelDiagram([MarshalAs(UnmanagedType.BStr)] String Name);

    [DispId(12857)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClassCollection GetAllClassesEx(Boolean Recursive, Boolean Nested);

    [DispId(12869)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMClassDiagram AddUseCaseDiagram([MarshalAs(UnmanagedType.BStr)] String theName);

    [DispId(12872)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern REICOMItem GetContext();

    [DispId(12886)]
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern RosePropertyCollection GetUserOverriddenProperties([MarshalAs(UnmanagedType.BStr)] String theToolName);
  }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [TypeLibType(4096)]
    [Guid("C78E702C-86E4-11CF-B3D4-00A0241DB1D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IRoseSubsystem : IRosePackage
        {
        [DispId(412)]
        RoseModuleCollection Modules { [DispId(412), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(412), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(413)]
        RoseSubsystemCollection Subsystems { [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(413), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(414)]
        RoseSubsystem ParentSubsystem { [DispId(414), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(414), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(415)]
        RoseModuleDiagramCollection ModuleDiagrams { [DispId(415), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(415), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(416)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModule AddModule([MarshalAs(UnmanagedType.BStr)] String theName);

        [DispId(418)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModuleDiagram AddModuleDiagram([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(419)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSubsystem AddSubsystem([MarshalAs(UnmanagedType.BStr)] String theName);

        [DispId(420)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RelocateModule([MarshalAs(UnmanagedType.Interface)] RoseModule theModule);

        [DispId(421)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RelocateSubsystem([MarshalAs(UnmanagedType.Interface)] RoseSubsystem theSubsystem);

        [DispId(422)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RelocateModuleDiagram([MarshalAs(UnmanagedType.Interface)] RoseModuleDiagram theModDiagram);

        [DispId(423)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModuleCollection GetAllModules();

        [DispId(424)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSubsystemCollection GetAllSubsystems();

        [DispId(425)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseCategoryCollection GetAssignedCategories();

        [DispId(426)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseClassCollection GetAssignedClasses();

        [DispId(427)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSubsystemCollection GetVisibleSubsystems();

        [DispId(428)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModuleVisibilityRelationshipCollection GetSubsystemDependencies([MarshalAs(UnmanagedType.Interface)] RoseSubsystem theSubsystem);

        [DispId(429)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean TopLevel();

        [DispId(430)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModuleVisibilityRelationship AddVisibilityRelationship([MarshalAs(UnmanagedType.Interface)] RoseModule theModule);

        [DispId(433)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteVisibilityRelationship([MarshalAs(UnmanagedType.Interface)] RoseModuleVisibilityRelationship theVisibilityRelationship);

        [DispId(434)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.IDispatch)]
        Object AddSubsystemVisibilityRelation([MarshalAs(UnmanagedType.IDispatch)] Object theSubsystem);

        [DispId(449)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteModule([MarshalAs(UnmanagedType.Interface)] RoseModule pIDispatch);

        [DispId(450)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteSubSystem([MarshalAs(UnmanagedType.Interface)] RoseSubsystem pIDispatch);

        [DispId(12705)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseModuleVisibilityRelationshipCollection GetVisibilityRelations();
        }
    }

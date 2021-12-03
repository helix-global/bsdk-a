using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("D7BC1B40-8618-11CF-B3D4-00A0241DB1D0")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMApplication
        {
        [DispId(202)]
        Boolean Visible { [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(202), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(205)]
        Int16 Top { [DispId(205), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(205), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(206)]
        Int16 Left { [DispId(206), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(206), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(207)]
        Int16 Height { [DispId(207), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(207), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(208)]
        Int16 Width { [DispId(208), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(208), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(209)]
        IREICOMModel CurrentModel { [DispId(209), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(209), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(224)]
        IREICOMPathMap PathMap { [DispId(224), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(224), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(231)]
        String Version { [DispId(231), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(231), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(232)]
        String ProductName { [DispId(232), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(232), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(233)]
        String ApplicationPath { [DispId(233), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(233), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12544)]
        IREICOMAddInManager AddInManager { [DispId(12544), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12544), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12586)]
        String CommandLine { [DispId(12586), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12586), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12809)]
        Boolean IsInitialized { [DispId(12809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(210)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMModel OpenModel([MarshalAs(UnmanagedType.BStr)] String theModel);

        [DispId(211)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMModel NewModel();

        [DispId(212)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Exit();

        [DispId(213)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WriteErrorLog([MarshalAs(UnmanagedType.BStr)] String theMsg);

        [DispId(214)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Save(Boolean bSaveUnits);

        [DispId(215)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SaveAs([MarshalAs(UnmanagedType.BStr)] String theFile, Boolean bSaveUnits);

        [DispId(218)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CompileScriptFile([MarshalAs(UnmanagedType.BStr)] String FileName, [MarshalAs(UnmanagedType.BStr)] String BinaryName, Boolean bDebug);

        [DispId(221)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SelectObjectInBrowser([MarshalAs(UnmanagedType.Interface)] IREICOMObject theRoseObject);

        [DispId(223)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMModel OpenModelAsTemplate([MarshalAs(UnmanagedType.BStr)] String szFileName);

        [DispId(225)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OpenScript([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(226)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void NewScript();

        [DispId(235)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMApplication GetLicensedApplication([MarshalAs(UnmanagedType.BStr)] String theKey);

        [DispId(236)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ExecuteScript([MarshalAs(UnmanagedType.BStr)] String pFileName);

        [DispId(12587)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean OpenURL([MarshalAs(UnmanagedType.BStr)] String theURL);

        [DispId(12588)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean OpenExternalDocument([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12589)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetProfileString([MarshalAs(UnmanagedType.BStr)] String Section, [MarshalAs(UnmanagedType.BStr)] String Entry, [MarshalAs(UnmanagedType.BStr)] String Default);

        [DispId(12590)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean WriteProfileString([MarshalAs(UnmanagedType.BStr)] String Section, [MarshalAs(UnmanagedType.BStr)] String Entry, [MarshalAs(UnmanagedType.BStr)] String Value);

        [DispId(12679)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean UpdateBrowserOverlayImage([MarshalAs(UnmanagedType.Interface)] IREICOMItem theItem);

        [DispId(12688)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean UpdateBrowserDocOverlayImage([MarshalAs(UnmanagedType.Interface)] IREICOMExternalDocument theDocument);

        [DispId(12697)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMModel OpenRoseModel([MarshalAs(UnmanagedType.BStr)] String theModel, Boolean promptSubUnits);

        [DispId(12698)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String GetRoseIniPath();

        [DispId(12856)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Int32 SaveModel(Boolean bSaveUnits);

        [DispId(12859)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EnableUserEditOfItem([MarshalAs(UnmanagedType.Interface)] IREICOMItem theItem);

        [DispId(12860)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EnableUserEditOfDiagram([MarshalAs(UnmanagedType.Interface)] IREICOMDiagram theDiagram);

        [DispId(12861)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EditorOpenFile([MarshalAs(UnmanagedType.BStr)] String FileName, [MarshalAs(UnmanagedType.BStr)] String fileDomain);

        [DispId(12862)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String EditorDomainOf([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12863)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMStringCollection EditorGetOpenFiles();

        [DispId(12864)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EditorIsDirty([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12865)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EditorIsVisible([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12866)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EditorRefreshFile([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12867)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean EditorDisplayFile([MarshalAs(UnmanagedType.BStr)] String FileName);

        [DispId(12868)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean LogCreateTab([MarshalAs(UnmanagedType.BStr)] String tabName, [MarshalAs(UnmanagedType.BStr)] String domain);

        [DispId(12869)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean LogCloseTab([MarshalAs(UnmanagedType.BStr)] String tabName, [MarshalAs(UnmanagedType.BStr)] String domain);

        [DispId(12870)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean LogClearTab([MarshalAs(UnmanagedType.BStr)] String tabName, [MarshalAs(UnmanagedType.BStr)] String domain);

        [DispId(12871)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean LogSetActiveTab([MarshalAs(UnmanagedType.BStr)] String tabName, [MarshalAs(UnmanagedType.BStr)] String domain);

        [DispId(12872)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean LogWriteTab([MarshalAs(UnmanagedType.BStr)] String tabName, [MarshalAs(UnmanagedType.BStr)] String domain, [MarshalAs(UnmanagedType.BStr)] String Text);
        }
    }

﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("A69CAB23-9179-11D0-A214-00A024FFFE40")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMState : IREICOMStateVertex
        {
        [DispId(421)]
        Boolean History { [DispId(421), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(421), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(423)]
        RoseRichType StateKind { [DispId(423), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(423), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(444)]
        RoseStateCollection SubStates { [DispId(444), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(444), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12802)]
        REICOMActivityCollection SubActivities { [DispId(12802), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12802), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12803)]
        REICOMDecisionCollection SubDecisions { [DispId(12803), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12803), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(12804)]
        RoseSyncItemCollection SubSynchronizations { [DispId(12804), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(12804), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

        [DispId(425)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseStateCollection GetAllSubStates();

        [DispId(428)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseState AddState([MarshalAs(UnmanagedType.BStr)] String Name);

        [DispId(429)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteState([MarshalAs(UnmanagedType.Interface)] RoseState State);

        [DispId(430)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean RelocateState([MarshalAs(UnmanagedType.Interface)] RoseState State);

        [DispId(446)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteAction([MarshalAs(UnmanagedType.Interface)] REICOMAction theAction);

        [DispId(12623)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMEventCollection GetUserDefinedEvents();

        [DispId(12624)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMAction GetAction([MarshalAs(UnmanagedType.Interface)] REICOMEvent theEvent);

        [DispId(12625)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMActionCollection GetEntryActions();

        [DispId(12626)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMActionCollection GetExitActions();

        [DispId(12627)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMActionCollection GetDoActions();

        [DispId(12635)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMEvent AddUserDefinedEvent([MarshalAs(UnmanagedType.BStr)] String EventName, [MarshalAs(UnmanagedType.BStr)] String ActionName);

        [DispId(12636)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteUserDefinedEvent([MarshalAs(UnmanagedType.Interface)] REICOMEvent theEvent);

        [DispId(12637)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMAction AddEntryAction([MarshalAs(UnmanagedType.BStr)] String ActionName);

        [DispId(12638)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMAction AddExitAction([MarshalAs(UnmanagedType.BStr)] String ActionName);

        [DispId(12639)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMAction AddDoAction([MarshalAs(UnmanagedType.BStr)] String ActionName);

        [DispId(12767)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMActionCollection GetActions();

        [DispId(12768)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseStateMachineCollection GetStateMachines();

        [DispId(12774)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseStateMachine AddStateMachine([MarshalAs(UnmanagedType.BStr)] String theName);

        [DispId(12775)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean DeleteStateMachine([MarshalAs(UnmanagedType.Interface)] RoseStateMachine theStateMachine);

        [DispId(12805)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMActivityCollection GetAllSubActivities();

        [DispId(12806)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        REICOMDecisionCollection GetAllSubDecisions();

        [DispId(12807)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        RoseSyncItemCollection GetAllSubSynchronizations();
        }
    }

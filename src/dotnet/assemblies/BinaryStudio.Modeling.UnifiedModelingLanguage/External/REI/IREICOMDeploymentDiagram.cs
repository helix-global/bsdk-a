using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [Guid("C2C15EC4-E028-11CF-B091-00A0241E3F73")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMDeploymentDiagram : IREICOMDiagram
        {
        [DispId(411)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMProcessorCollection GetProcessors();

        [DispId(412)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMDeviceCollection GetDevices();

        [DispId(413)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMItemView AddProcessor([MarshalAs(UnmanagedType.Interface)] IREICOMProcessor theProcessor, Int16 x, Int16 y);

        [DispId(414)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMItemView AddDevice([MarshalAs(UnmanagedType.Interface)] IREICOMDevice theDevice, Int16 x, Int16 y);

        [DispId(415)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean RemoveProcessor([MarshalAs(UnmanagedType.Interface)] IREICOMProcessor theProcessor);

        [DispId(416)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Boolean RemoveDevice([MarshalAs(UnmanagedType.Interface)] IREICOMDevice theDevice);
        }
    }

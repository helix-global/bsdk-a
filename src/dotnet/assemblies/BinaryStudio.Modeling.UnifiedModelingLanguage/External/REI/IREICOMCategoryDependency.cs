using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RationalRose
    {
    [Guid("4ACE189B-6CD3-11D1-BC1E-00A024C67143")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [TypeLibType(TypeLibTypeFlags.FDispatchable)]
    [ComImport]
    public interface IREICOMCategoryDependency : IREICOMRelation
        {
        [DispId(12657)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMCategory GetContextCategory();

        [DispId(12658)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [return: MarshalAs(UnmanagedType.Interface)]
        IREICOMCategory GetSupplierCategory();
        }
    }

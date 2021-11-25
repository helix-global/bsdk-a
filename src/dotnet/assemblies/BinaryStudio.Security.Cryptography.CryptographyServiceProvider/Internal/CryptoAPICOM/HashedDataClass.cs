using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
{
	[DefaultMember("Value"), ClassInterface(ClassInterfaceType.None), Guid("CE32ABF6-475D-41F6-BF82-D27F03E3D38B"), TypeLibType(2)]
	[ComImport]
	public class HashedDataClass : IHashedData
	{
		[DispId(0)]
		public virtual extern String Value
		{
			[DispId(0)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		[DispId(1)]
		public virtual extern CAPICOM_HASH_ALGORITHM Algorithm
		{
			[DispId(1)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[DispId(1)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[DispId(2)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern void Hash([MarshalAs(UnmanagedType.BStr)] [In] String newVal);
	}
}

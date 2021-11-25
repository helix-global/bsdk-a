
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates.Internal;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    using CRYPT_INTEGER_BLOB = CRYPT_BLOB;
    using CERT_NAME_BLOB = CRYPT_BLOB;

    public abstract class X509Object : IX509Object, IDisposable, ICustomTypeDescriptor
        {
        private LocalMemoryManager manager;
        public abstract X509ObjectType ObjectType { get; }
        public abstract IntPtr Handle { get; }

        IntPtr IX509Object.Handle { get { return Handle; }}

        protected const UInt32 X509_ASN_ENCODING           = 0x00000001;
        protected const UInt32 PKCS_7_ASN_ENCODING         = 0x00010000;

        static X509Object()
            {
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing)
            {
            }
        #endregion
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~X509Object()
            {
            Dispose(false);
            }
        #endregion
        #region M:Validate(Boolean)
        protected virtual void Validate(Boolean status) {
            if (!status) {
                Exception e;
                var i = (Int32)GetLastWin32Error();
                if ((i >= 0xFFFF) || (i < 0))
                    {
                    e = new COMException($"{FormatMessage(i)} [HRESULT:{(HRESULT)i}]");
                    }
                else
                    {
                    e = new COMException($"{FormatMessage(i)} [{(Win32ErrorCode)i}]");
                    }
                throw e;
                }
            }
        #endregion
        #region M:FormatMessage(Int32):String
        protected internal static unsafe String FormatMessage(Int32 source) {
            void* r;
            if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER|FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, source,
                ((((UInt32)(SUBLANG_DEFAULT)) << 10) | (UInt32)(LANG_NEUTRAL)),
                &r, 0, new IntPtr[0])) {
                try
                    {
                    #if CAPILITE
                    var o = Marshal.PtrToStringAnsi((IntPtr)r);
                    return (o != null)
                        ? o.Trim().TrimEnd('\n')
                        : null;
                    #else
                    return (new String((Char*)r)).Trim();
                    #endif
                    }
                finally
                    {
                    LocalFree(r);
                    }
                }
            return null;
            }
        #endregion
        #region M:GetLastWin32Error:Win32ErrorCode
        protected static Win32ErrorCode GetLastWin32Error()
            {
            #if CAPILITE
            return (Win32ErrorCode)GetLastError();
            #else
            return (Win32ErrorCode)Marshal.GetLastWin32Error();
            #endif
            }
        #endregion
        #region M:GetHRForLastWin32Error:Int32
        protected static Int32 GetHRForLastWin32Error()
            {
            #if CAPILITE
            var e = (Int32)GetLastWin32Error();
            if (((long)e & 0x80000000L) == 0x80000000L) { return e; }
            return e & (Int32)UInt16.MaxValue | (unchecked((Int32)0x80070000));
            #else
            return (Int32)(Marshal.GetHRForLastWin32Error());
            #endif
            }
        #endregion
        #region M:GetSigningStreamInternal(Stream):IEnumerable<Byte>
        protected static IEnumerable<Byte> GetSigningStreamInternal(Stream source) {
            lock (source)
                {
                var buffer = new Byte[1024];
                source.Seek(0, SeekOrigin.Begin);
                for (;;) {
                    var size = source.Read(buffer, 0, buffer.Length);
                    if (size == 0) { break; }
                    for (var i = 0; i < size; ++i) {
                        yield return buffer[i];
                        }
                    }
                }
            }
        #endregion

        #if CAPILITE
        [DllImport("capi20", EntryPoint = "GetLastError")]   private static extern Int32 GetLastError();
        [DllImport("capi20", EntryPoint = "FormatMessageA")] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("capi20")] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("capi20", CharSet = CharSet.Auto)] private static extern UInt32 CertNameToStr([In] UInt32 dwCertEncodingType, [In] ref CERT_NAME_BLOB pName, [In] UInt32 dwStrType, [In] [Out] IntPtr psz, [In] UInt32 csz);
        #else
        [DllImport("kernel32.dll", SetLastError = true)] internal static extern unsafe IntPtr LocalFree(void* handle);
        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern UInt32 CertNameToStr([In] UInt32 dwCertEncodingType, [In] ref CERT_NAME_BLOB pName, [In] UInt32 dwStrType, [In] [Out] IntPtr psz, [In] UInt32 csz);
        #endif

        private   const UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        private   const UInt32 FORMAT_MESSAGE_FROM_SYSTEM     = 0x00001000;
        private   const UInt32 FORMAT_MESSAGE_FROM_HMODULE    = 0x00000800;
        private   const UInt32 LANG_NEUTRAL                   = 0x00;
        private   const UInt32 SUBLANG_DEFAULT                = 0x01;

        #region M:ICustomTypeDescriptor.GetAttributes:AttributeCollection
        protected virtual AttributeCollection GetAttributes() { return TypeDescriptor.GetAttributes(GetType()); }
        /**
         * <summary>Returns a collection of custom attributes for this instance of a component.</summary>
         * <returns>An <see cref="AttributeCollection"/> containing the attributes for this object.</returns>
         * */
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
            return GetAttributes();
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetClassName:String
        /**
         * <summary>Returns the class name of this instance of a component.</summary>
         * <returns>The class name of the object, or null if the class does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetClassName()
            {
            return TypeDescriptor.GetClassName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetComponentName:String
        /**
         * <summary>Returns the name of this instance of a component.</summary>
         * <returns>The name of the object, or null if the object does not have a name.</returns>
         * */
        String ICustomTypeDescriptor.GetComponentName()
            {
            return TypeDescriptor.GetComponentName(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetConverter:TypeConverter
        /**
         * <summary>Returns a type converter for this instance of a component.</summary>
         * <returns>A <see cref="TypeConverter"/> that is the converter for this object, or null if there is no <see cref="TypeConverter"/> for this object.</returns>
         * */
        TypeConverter ICustomTypeDescriptor.GetConverter()
            {
            return TypeDescriptor.GetConverter(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultEvent:EventDescriptor
        /**
         * <summary>Returns the default event for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptor"/> that represents the default event for this object, or null if this object does not have events.</returns>
         * */
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
            return TypeDescriptor.GetDefaultEvent(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetDefaultProperty:PropertyDescriptor
        /**
         * <summary>Returns the default property for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptor"/> that represents the default property for this object, or null if this object does not have properties.</returns>
         * */
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
            return TypeDescriptor.GetDefaultProperty(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEditor(Type):Object
        /**
         * <summary>Returns an editor of the specified type for this instance of a component.</summary>
         * <param name="editortype">A <see cref="Type"/> that represents the editor for this object. </param>
         * <returns>An <see cref="Object"/> of the specified type that is the editor for this object, or null if the editor cannot be found.</returns>
         * */
        Object ICustomTypeDescriptor.GetEditor(Type editortype)
            {
            return TypeDescriptor.GetEditor(GetType(), editortype);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents:EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component.</summary>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
            return TypeDescriptor.GetEvents(GetType());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetEvents(Attribute[]):EventDescriptorCollection
        /**
         * <summary>Returns the events for this instance of a component using the specified attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>An <see cref="EventDescriptorCollection"/> that represents the filtered events for this component instance.</returns>
         * */
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
            return TypeDescriptor.GetEvents(GetType(), attributes);
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties:PropertyDescriptorCollection
        /**
         * <summary>Returns the properties for this instance of a component.</summary>
         * */
        protected virtual IEnumerable<PropertyDescriptor> GetProperties() { return TypeDescriptor.GetProperties(GetType()).OfType<PropertyDescriptor>(); }
        /**
         * <summary>Returns the properties for this instance of a component.</summary>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
            return new PropertyDescriptorCollection(GetProperties().ToArray());
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor):Object
        /**
         * <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
         * <param name="descriptor">A <see cref="PropertyDescriptor"/> that represents the property whose owner is to be found. </param>
         * <returns>An <see cref="Object"/> that represents the owner of the specified property.</returns>
         * */
        Object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor descriptor)
            {
            return this;
            }
        #endregion
        #region M:ICustomTypeDescriptor.GetProperties(Attribute[]):PropertyDescriptorCollection
        protected virtual IEnumerable<PropertyDescriptor> GetProperties(Attribute[] attributes) {
            return TypeDescriptor.GetProperties(GetType(), attributes).OfType<PropertyDescriptor>().OrderBy(i => {
                var dispid = i.Attributes[typeof(DispIdAttribute)] as DispIdAttribute;
                return (dispid != null)
                    ? dispid.Value
                    : Int32.MaxValue;
                });
            }
        /**
         * <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
         * <param name="attributes">An array of type <see cref="Attribute"/> that is used as a filter. </param>
         * <returns>A <see cref="PropertyDescriptorCollection"/> that represents the filtered properties for this component instance.</returns>
         * */
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
            return new PropertyDescriptorCollection(GetProperties(attributes).ToArray());
            }
        #endregion

        private void EnsureMemoryManager() {
            if (manager == null) {
                manager = new LocalMemoryManager();
                }
            }

        protected unsafe void* LocalAlloc(Int32 size)
            {
            EnsureMemoryManager();
            return manager.Alloc(size);
            }
        }
    }

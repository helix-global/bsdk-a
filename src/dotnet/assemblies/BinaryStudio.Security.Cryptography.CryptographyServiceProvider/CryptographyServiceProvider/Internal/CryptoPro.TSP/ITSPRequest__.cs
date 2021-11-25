using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FNonExtensible|TypeLibTypeFlags.FDual)]
    [Guid("D493E84E-4055-4691-AE63-8B6309AAB3AB")]
    public interface ITSPRequest__
        {
        #region P:TSAAddress:String
        /// <summary>
        /// Адрес службы штампов времени.
        /// </summary>
        [DispId(1)]
        String TSAAddress
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(1)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(1)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:CertReq:Boolean
        /// <summary>
        /// Флаг включения сертификата службы штампов в штамп.
        /// </summary>
        [DispId(2)]
        Boolean CertReq
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(2)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(2)][param: In]set;
            }
        #endregion
        #region P:UseNonce:Boolean
        /// <summary>
        /// Флаг для включения в запрос поля "Nonce".
        /// </summary>
        [DispId(3)]
        Boolean UseNonce
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(3)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(3)][param: In]set;
            }
        #endregion
        #region P:TSAPassword:String
        /// <summary>
        /// Пароль для использования при аутентификации.
        /// </summary>
        [DispId(4)]
        String TSAPassword
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(4)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(4)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:PolicyID:String
        /// <summary>
        /// Идентификатор политики службы штампов.
        /// </summary>
        [DispId(5)]
        String PolicyID
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(5)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(5)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:ProxyAddress:String
        /// <summary>
        /// Адрес прокси-сервера.
        /// </summary>
        [DispId(6)]
        String ProxyAddress
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(6)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(6)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:ProxyPassword:String
        /// <summary>
        /// Пароль для использования при аутентификации для прокси-сервера.
        /// </summary>
        [DispId(7)]
        String ProxyPassword
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(7)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(7)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:ProxyUserName:String
        /// <summary>
        /// Имя пользователя для использования прокси-сервера.
        /// </summary>
        [DispId(8)]
        String ProxyUserName
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(8)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(8)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:TSAUserName:String
        /// <summary>
        /// Имя пользователя для использования при аутентификации.
        /// </summary>
        [DispId(9)]
        String TSAUserName
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(9)][return: MarshalAs(UnmanagedType.BStr)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(9)][param: In][param: MarshalAs(UnmanagedType.BStr)]set;
            }
        #endregion
        #region P:Hash:IHashedData
        /// <summary>
        /// Задает хэш данных, включаемый в запрос на штамп времени и его алгоритм (только запись).
        /// </summary>
        [DispId(11)]
        IHashedData Hash
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(11)][param: In]
            [param: MarshalAs(UnmanagedType.Interface)]set;
            }
        #endregion
        #region P:ClientCertificate:ICertContext
        /// <summary>
        /// Клиентский сертификат для подключения к службе штампов.
        /// </summary>
        [DispId(13)]
        ICertContext ClientCertificate
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(13)][return: MarshalAs(UnmanagedType.Interface)]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(13)][param: In][param: MarshalAs(UnmanagedType.Interface)]set;
            }
        #endregion
        #region P:TSAAuthType:TSPCOM_AUTH_TYPE
        /// <summary>
        /// Тип аутентификации.
        /// </summary>
        [DispId(14)]
        [ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
        TSPCOM_AUTH_TYPE TSAAuthType
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(14)][return: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(14)][param: In][param: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]set;
            }
        #endregion
        #region P:ProxyAuthType:TSPCOM_AUTH_TYPE
        /// <summary>
        /// Тип аутентификации для прокси-сервера.
        /// </summary>
        [ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]
        [DispId(15)]
        TSPCOM_AUTH_TYPE ProxyAuthType
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(15)][return: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]get;
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(15)][param: In][param: ComAliasName("TSPCOM.TSPCOM_AUTH_TYPE")]set;
            }
        #endregion
        #region P:HashValue:String
        /// <summary>
        /// Хэш данных, включаемый в запрос на штамп времени (только чтение).
        /// </summary>
        [DispId(17)]
        String HashValue
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(17)][return: MarshalAs(UnmanagedType.BStr)]get;
            }
        #endregion
        #region P:HashAlgorithm:IOID
        /// <summary>
        /// Алгоритм хэширования данных (только чтение).
        /// </summary>
        [DispId(18)]
        IOID HashAlgorithm
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(18)][return: MarshalAs(UnmanagedType.Interface)]get;
            }
        #endregion
        #region P:HTTPStatus:Int32
        /// <summary>
        /// Статус HTTP-запроса (только чтение).
        /// </summary>
        [DispId(19)]
        Int32 HTTPStatus
            {
            [MethodImpl(MethodImplOptions.InternalCall)][DispId(19)]get;
            }
        #endregion

        #region M:Import(String)
        /// <summary>
        /// Импорт существующего запроса.
        /// </summary>
        /// <param name="strRequest"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(16)]
        void Import([In] [MarshalAs(UnmanagedType.BStr)] String strRequest);
        #endregion
        #region M:Export:String
        /// <summary>
        /// Экспорт созданного запроса к службе штампов времени.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(12)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Export();
        #endregion
        #region M:Send(Boolean):ITSPStamp
        /// <summary>
        /// Отправка запроса службе штампов и получение штампа.
        /// </summary>
        /// <param name="Verify"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(10)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ITSPStamp Send([In] Boolean Verify = true);
        #endregion
        #region M:Display(Int32,String)
        /// <summary>
        /// Отображение информации о запросе в диалоговом окне.
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="Title"></param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(20)]
        void Display([In] Int32 hwndParent = 0, [In] [MarshalAs(UnmanagedType.BStr)] String Title = "");
        #endregion
        }
    }
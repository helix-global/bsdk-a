#include "hdrstop.h"
#include "module.h"
#include "json.h"
#include "logging.h"
#include "feature.h"
#include "scard.h"
#include "scode.h"
#include "trace.h"

#undef FormatMessage

#undef  THIS_FILE
#define THIS_FILE "detours.cc"
#undef  nameof
#define nameof(E) #E

#ifdef USE_MSDETOURS

#define H(E) H##E
#define O(E) O##E
#define T(E) T##E
#define TRY       __try     {
#define FINALLY } __finally {
#define END     }
#define GetProcAddressT(Module,E)  (T(E))GetProcAddress(Module,#E)

ModuleSafeHandle WinSCard(LoadLibraryA("winscard.dll"));

VOID DetDetach(PVOID*,PVOID,const char*);
VOID DetAttach(PVOID*,PVOID,const char*);

typedef LONG(WINAPI* T(SCardAddReaderToGroupA))(SCARDCONTEXT,LPCSTR ,LPCSTR );
typedef LONG(WINAPI* T(SCardAddReaderToGroupW))(SCARDCONTEXT,LPCWSTR,LPCWSTR);
typedef LONG(WINAPI* T(SCardAudit))(SCARDCONTEXT,DWORD);
typedef LONG(WINAPI* T(SCardBeginTransaction))(SCARDHANDLE);
typedef LONG(WINAPI* T(SCardCancel))(SCARDHANDLE);
typedef LONG(WINAPI* T(SCardConnectW))(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
typedef LONG(WINAPI* T(SCardConnectA))(SCARDCONTEXT,LPCSTR ,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
typedef LONG(WINAPI* T(SCardControl))(SCARDHANDLE,DWORD,LPCVOID,DWORD,LPVOID,DWORD,LPDWORD);
typedef LONG(WINAPI* T(SCardDisconnect))(SCARDHANDLE,DWORD);
typedef LONG(WINAPI* T(SCardEndTransaction))(SCARDHANDLE,DWORD);
typedef LONG(WINAPI* T(SCardEstablishContext))(DWORD,LPCVOID,LPCVOID,LPSCARDCONTEXT);
typedef LONG(WINAPI* T(SCardForgetCardTypeA))(SCARDCONTEXT,LPCSTR );
typedef LONG(WINAPI* T(SCardForgetCardTypeW))(SCARDCONTEXT,LPCWSTR);
typedef LONG(WINAPI* T(SCardForgetReaderA))(SCARDCONTEXT,LPCSTR );
typedef LONG(WINAPI* T(SCardForgetReaderW))(SCARDCONTEXT,LPCWSTR);
typedef LONG(WINAPI* T(SCardForgetReaderGroupA))(SCARDCONTEXT,LPCSTR );
typedef LONG(WINAPI* T(SCardForgetReaderGroupW))(SCARDCONTEXT,LPCWSTR);
typedef LONG(WINAPI* T(SCardFreeMemory))(SCARDCONTEXT,LPCVOID);
typedef LONG(WINAPI* T(SCardGetAttrib))(SCARDHANDLE,DWORD,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardGetCardTypeProviderNameA))(SCARDCONTEXT,LPCSTR ,DWORD, CHAR*,LPDWORD);
typedef LONG(WINAPI* T(SCardGetCardTypeProviderNameW))(SCARDCONTEXT,LPCWSTR,DWORD,WCHAR*,LPDWORD);
typedef LONG(WINAPI* T(SCardGetDeviceTypeIdA))(SCARDCONTEXT,LPCSTR ,LPDWORD);
typedef LONG(WINAPI* T(SCardGetDeviceTypeIdW))(SCARDCONTEXT,LPCWSTR,LPDWORD);
typedef LONG(WINAPI* T(SCardGetProviderIdA))(SCARDCONTEXT,LPCSTR ,LPGUID);
typedef LONG(WINAPI* T(SCardGetProviderIdW))(SCARDCONTEXT,LPCWSTR,LPGUID);
typedef LONG(WINAPI* T(SCardGetReaderDeviceInstanceIdA))(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
typedef LONG(WINAPI* T(SCardGetReaderDeviceInstanceIdW))(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
typedef LONG(WINAPI* T(SCardGetReaderIconA))(SCARDCONTEXT,LPCSTR ,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardGetReaderIconW))(SCARDCONTEXT,LPCWSTR,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardGetStatusChangeA))(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEA,DWORD);
typedef LONG(WINAPI* T(SCardGetStatusChangeW))(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEW,DWORD);
typedef LONG(WINAPI* T(SCardGetTransmitCount))(SCARDHANDLE,LPDWORD);
typedef LONG(WINAPI* T(SCardIntroduceCardTypeA))(SCARDCONTEXT,LPCSTR ,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD);
typedef LONG(WINAPI* T(SCardIntroduceCardTypeW))(SCARDCONTEXT,LPCWSTR,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD);
typedef LONG(WINAPI* T(SCardIntroduceReaderA))(SCARDCONTEXT,LPCSTR ,LPCSTR );
typedef LONG(WINAPI* T(SCardIntroduceReaderW))(SCARDCONTEXT,LPCWSTR,LPCWSTR);
typedef LONG(WINAPI* T(SCardIntroduceReaderGroupA))(SCARDCONTEXT,LPCSTR );
typedef LONG(WINAPI* T(SCardIntroduceReaderGroupW))(SCARDCONTEXT,LPCWSTR);
typedef LONG(WINAPI* T(SCardIsValidContext))(SCARDCONTEXT);
typedef LONG(WINAPI* T(SCardListCardsA))(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD, CHAR*,LPDWORD);
typedef LONG(WINAPI* T(SCardListCardsW))(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD,WCHAR*,LPDWORD);
typedef LONG(WINAPI* T(SCardListInterfacesA))(SCARDCONTEXT,LPCSTR ,LPGUID,LPDWORD);
typedef LONG(WINAPI* T(SCardListInterfacesW))(SCARDCONTEXT,LPCWSTR,LPGUID,LPDWORD);
typedef LONG(WINAPI* T(SCardListReaderGroupsA))(SCARDCONTEXT,LPSTR ,LPDWORD);
typedef LONG(WINAPI* T(SCardListReaderGroupsW))(SCARDCONTEXT,LPWSTR,LPDWORD);
typedef LONG(WINAPI* T(SCardListReadersA))(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
typedef LONG(WINAPI* T(SCardListReadersW))(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
typedef LONG(WINAPI* T(SCardListReadersWithDeviceInstanceIdA))(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
typedef LONG(WINAPI* T(SCardListReadersWithDeviceInstanceIdW))(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
typedef LONG(WINAPI* T(SCardLocateCardsA))(SCARDCONTEXT,LPCSTR ,LPSCARD_READERSTATEA,DWORD);
typedef LONG(WINAPI* T(SCardLocateCardsW))(SCARDCONTEXT,LPCWSTR,LPSCARD_READERSTATEW,DWORD);
typedef LONG(WINAPI* T(SCardLocateCardsByATRA))(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEA,DWORD);
typedef LONG(WINAPI* T(SCardLocateCardsByATRW))(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEW,DWORD);
typedef LONG(WINAPI* T(SCardReadCacheA))(SCARDCONTEXT,UUID*,DWORD,LPSTR ,PBYTE,DWORD*);
typedef LONG(WINAPI* T(SCardReadCacheW))(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD*);
typedef LONG(WINAPI* T(SCardReconnect))(SCARDHANDLE,DWORD,DWORD,DWORD,LPDWORD);
typedef LONG(WINAPI* T(SCardReleaseContext))(SCARDCONTEXT);
typedef LONG(WINAPI* T(SCardRemoveReaderFromGroupA))(SCARDCONTEXT,LPCSTR ,LPCSTR );
typedef LONG(WINAPI* T(SCardRemoveReaderFromGroupW))(SCARDCONTEXT,LPCWSTR,LPCWSTR);
typedef LONG(WINAPI* T(SCardSetAttrib))(SCARDHANDLE,DWORD,LPCBYTE,DWORD);
typedef LONG(WINAPI* T(SCardSetCardTypeProviderNameA))(SCARDCONTEXT,LPCSTR ,DWORD,LPCSTR );
typedef LONG(WINAPI* T(SCardSetCardTypeProviderNameW))(SCARDCONTEXT,LPCWSTR,DWORD,LPCWSTR);
typedef LONG(WINAPI* T(SCardStatusA))(SCARDHANDLE,LPSTR ,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardStatusW))(SCARDHANDLE,LPWSTR,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardTransmit))(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardWriteCacheA))(SCARDCONTEXT,UUID*,DWORD,LPSTR ,PBYTE,DWORD);
typedef LONG(WINAPI* T(SCardWriteCacheW))(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD);

#define ATTACH(E)                   DetAttach(&(PVOID&)O##E,H##E,#E)
#define DETACH(E)                   DetDetach(&(PVOID&)O##E,H##E,#E)
#define ToString(E)                 E##ToString
#define D(E) T(E) O(E) = nullptr

D(SCardAddReaderToGroupA);
D(SCardAddReaderToGroupW);
D(SCardAudit);
D(SCardBeginTransaction);
D(SCardCancel);
D(SCardConnectA);
D(SCardConnectW);
D(SCardControl);
D(SCardDisconnect);
D(SCardEndTransaction);
D(SCardEstablishContext);
D(SCardForgetCardTypeA);
D(SCardForgetCardTypeW);
D(SCardForgetReaderA);
D(SCardForgetReaderW);
D(SCardForgetReaderGroupA);
D(SCardForgetReaderGroupW);
D(SCardFreeMemory);
D(SCardGetAttrib);
D(SCardGetCardTypeProviderNameA);
D(SCardGetCardTypeProviderNameW);
D(SCardGetDeviceTypeIdA);
D(SCardGetDeviceTypeIdW);
D(SCardGetProviderIdA);
D(SCardGetProviderIdW);
D(SCardGetReaderDeviceInstanceIdA);
D(SCardGetReaderDeviceInstanceIdW);
D(SCardGetReaderIconA);
D(SCardGetReaderIconW);
D(SCardGetStatusChangeA);
D(SCardGetStatusChangeW);
D(SCardGetTransmitCount);
D(SCardIntroduceCardTypeA);
D(SCardIntroduceCardTypeW);
D(SCardIntroduceReaderA);
D(SCardIntroduceReaderW);
D(SCardIntroduceReaderGroupA);
D(SCardIntroduceReaderGroupW);
D(SCardIsValidContext);
D(SCardListCardsA);
D(SCardListCardsW);
D(SCardListInterfacesA);
D(SCardListInterfacesW);
D(SCardListReaderGroupsA);
D(SCardListReaderGroupsW);
D(SCardListReadersA);
D(SCardListReadersW);
D(SCardListReadersWithDeviceInstanceIdA);
D(SCardListReadersWithDeviceInstanceIdW);
D(SCardLocateCardsA);
D(SCardLocateCardsW);
D(SCardLocateCardsByATRA);
D(SCardLocateCardsByATRW);
D(SCardReadCacheA);
D(SCardReadCacheW);
D(SCardReconnect);
D(SCardReleaseContext);
D(SCardRemoveReaderFromGroupA);
D(SCardRemoveReaderFromGroupW);
D(SCardSetAttrib);
D(SCardSetCardTypeProviderNameA);
D(SCardSetCardTypeProviderNameW);
D(SCardStatusA);
D(SCardStatusW);
D(SCardTransmit);
D(SCardWriteCacheA);
D(SCardWriteCacheW);

#undef D

LONG WINAPI H(SCardAddReaderToGroupA)(SCARDCONTEXT,LPCSTR ,LPCSTR );
LONG WINAPI H(SCardAddReaderToGroupW)(SCARDCONTEXT,LPCWSTR,LPCWSTR);
LONG WINAPI H(SCardAudit)(SCARDCONTEXT,DWORD);
LONG WINAPI H(SCardBeginTransaction)(SCARDHANDLE);
LONG WINAPI H(SCardCancel)(SCARDHANDLE);
LONG WINAPI H(SCardConnectW)(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardConnectA)(SCARDCONTEXT,LPCSTR ,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardControl)(SCARDHANDLE,DWORD,LPCVOID,DWORD,LPVOID,DWORD,LPDWORD);
LONG WINAPI H(SCardDisconnect)(SCARDHANDLE,DWORD);
LONG WINAPI H(SCardEndTransaction)(SCARDHANDLE,DWORD);
LONG WINAPI H(SCardEstablishContext)(DWORD,LPCVOID,LPCVOID,LPSCARDCONTEXT);
LONG WINAPI H(SCardForgetCardTypeA)(SCARDCONTEXT,LPCSTR );
LONG WINAPI H(SCardForgetCardTypeW)(SCARDCONTEXT,LPCWSTR);
LONG WINAPI H(SCardForgetReaderA)(SCARDCONTEXT,LPCSTR );
LONG WINAPI H(SCardForgetReaderW)(SCARDCONTEXT,LPCWSTR);
LONG WINAPI H(SCardForgetReaderGroupA)(SCARDCONTEXT,LPCSTR );
LONG WINAPI H(SCardForgetReaderGroupW)(SCARDCONTEXT,LPCWSTR);
LONG WINAPI H(SCardFreeMemory)(SCARDCONTEXT,LPCVOID);
LONG WINAPI H(SCardGetAttrib)(SCARDHANDLE,DWORD,LPBYTE,LPDWORD);
LONG WINAPI H(SCardGetCardTypeProviderNameA)(SCARDCONTEXT,LPCSTR ,DWORD, CHAR*,LPDWORD);
LONG WINAPI H(SCardGetCardTypeProviderNameW)(SCARDCONTEXT,LPCWSTR,DWORD,WCHAR*,LPDWORD);
LONG WINAPI H(SCardGetDeviceTypeIdA)(SCARDCONTEXT,LPCSTR ,LPDWORD);
LONG WINAPI H(SCardGetDeviceTypeIdW)(SCARDCONTEXT,LPCWSTR,LPDWORD);
LONG WINAPI H(SCardGetProviderIdA)(SCARDCONTEXT,LPCSTR ,LPGUID);
LONG WINAPI H(SCardGetProviderIdW)(SCARDCONTEXT,LPCWSTR,LPGUID);
LONG WINAPI H(SCardGetReaderDeviceInstanceIdA)(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
LONG WINAPI H(SCardGetReaderDeviceInstanceIdW)(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
LONG WINAPI H(SCardGetReaderIconA)(SCARDCONTEXT,LPCSTR ,LPBYTE,LPDWORD);
LONG WINAPI H(SCardGetReaderIconW)(SCARDCONTEXT,LPCWSTR,LPBYTE,LPDWORD);
LONG WINAPI H(SCardGetStatusChangeA)(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEA,DWORD);
LONG WINAPI H(SCardGetStatusChangeW)(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEW,DWORD);
LONG WINAPI H(SCardGetTransmitCount)(SCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardIntroduceCardTypeA)(SCARDCONTEXT,LPCSTR ,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD);
LONG WINAPI H(SCardIntroduceCardTypeW)(SCARDCONTEXT,LPCWSTR,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD);
LONG WINAPI H(SCardIntroduceReaderA)(SCARDCONTEXT,LPCSTR ,LPCSTR );
LONG WINAPI H(SCardIntroduceReaderW)(SCARDCONTEXT,LPCWSTR,LPCWSTR);
LONG WINAPI H(SCardIntroduceReaderGroupA)(SCARDCONTEXT,LPCSTR );
LONG WINAPI H(SCardIntroduceReaderGroupW)(SCARDCONTEXT,LPCWSTR);
LONG WINAPI H(SCardIsValidContext)(SCARDCONTEXT);
LONG WINAPI H(SCardListCardsA)(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD, CHAR*,LPDWORD);
LONG WINAPI H(SCardListCardsW)(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD,WCHAR*,LPDWORD);
LONG WINAPI H(SCardListInterfacesA)(SCARDCONTEXT,LPCSTR ,LPGUID,LPDWORD);
LONG WINAPI H(SCardListInterfacesW)(SCARDCONTEXT,LPCWSTR,LPGUID,LPDWORD);
LONG WINAPI H(SCardListReaderGroupsA)(SCARDCONTEXT,LPSTR ,LPDWORD);
LONG WINAPI H(SCardListReaderGroupsW)(SCARDCONTEXT,LPWSTR,LPDWORD);
LONG WINAPI H(SCardListReadersA)(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
LONG WINAPI H(SCardListReadersW)(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
LONG WINAPI H(SCardListReadersWithDeviceInstanceIdA)(SCARDCONTEXT,LPCSTR ,LPSTR ,LPDWORD);
LONG WINAPI H(SCardListReadersWithDeviceInstanceIdW)(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD);
LONG WINAPI H(SCardLocateCardsA)(SCARDCONTEXT,LPCSTR ,LPSCARD_READERSTATEA,DWORD);
LONG WINAPI H(SCardLocateCardsW)(SCARDCONTEXT,LPCWSTR,LPSCARD_READERSTATEW,DWORD);
LONG WINAPI H(SCardLocateCardsByATRA)(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEA,DWORD);
LONG WINAPI H(SCardLocateCardsByATRW)(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEW,DWORD);
LONG WINAPI H(SCardReadCacheA)(SCARDCONTEXT,UUID*,DWORD,LPSTR ,PBYTE,DWORD*);
LONG WINAPI H(SCardReadCacheW)(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD*);
LONG WINAPI H(SCardReconnect)(SCARDHANDLE,DWORD,DWORD,DWORD,LPDWORD);
LONG WINAPI H(SCardReleaseContext)(SCARDCONTEXT);
LONG WINAPI H(SCardRemoveReaderFromGroupA)(SCARDCONTEXT,LPCSTR ,LPCSTR );
LONG WINAPI H(SCardRemoveReaderFromGroupW)(SCARDCONTEXT,LPCWSTR,LPCWSTR);
LONG WINAPI H(SCardSetAttrib)(SCARDHANDLE,DWORD,LPCBYTE,DWORD);
LONG WINAPI H(SCardSetCardTypeProviderNameA)(SCARDCONTEXT,LPCSTR ,DWORD,LPCSTR );
LONG WINAPI H(SCardSetCardTypeProviderNameW)(SCARDCONTEXT,LPCWSTR,DWORD,LPCWSTR);
LONG WINAPI H(SCardStatusA)(SCARDHANDLE,LPSTR ,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD);
LONG WINAPI H(SCardStatusW)(SCARDHANDLE,LPWSTR,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD);
LONG WINAPI H(SCardTransmit)(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);
LONG WINAPI H(SCardWriteCacheA)(SCARDCONTEXT,UUID*,DWORD,LPSTR ,PBYTE,DWORD);
LONG WINAPI H(SCardWriteCacheW)(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD);
#endif

HRESULT Module::AttachDetours()
    {
    #ifdef USE_MSDETOURS
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourSetIgnoreTooSmall(TRUE);
    #define I(E) O(E) = GetProcAddressT(WinSCard,E)

    I(SCardAddReaderToGroupA);
    I(SCardAddReaderToGroupW);
    I(SCardAudit);
    I(SCardBeginTransaction);
    I(SCardCancel);
    I(SCardConnectA);
    I(SCardConnectW);
    I(SCardControl);
    I(SCardDisconnect);
    I(SCardEndTransaction);
    I(SCardEstablishContext);
    I(SCardForgetCardTypeA);
    I(SCardForgetCardTypeW);
    I(SCardForgetReaderA);
    I(SCardForgetReaderW);
    I(SCardForgetReaderGroupA);
    I(SCardForgetReaderGroupW);
    I(SCardFreeMemory);
    I(SCardGetAttrib);
    I(SCardGetCardTypeProviderNameA);
    I(SCardGetCardTypeProviderNameW);
    I(SCardGetDeviceTypeIdA);
    I(SCardGetDeviceTypeIdW);
    I(SCardGetProviderIdA);
    I(SCardGetProviderIdW);
    I(SCardGetReaderDeviceInstanceIdA);
    I(SCardGetReaderDeviceInstanceIdW);
    I(SCardGetReaderIconA);
    I(SCardGetReaderIconW);
    I(SCardGetStatusChangeA);
    I(SCardGetStatusChangeW);
    I(SCardGetTransmitCount);
    I(SCardIntroduceCardTypeA);
    I(SCardIntroduceCardTypeW);
    I(SCardIntroduceReaderA);
    I(SCardIntroduceReaderW);
    I(SCardIntroduceReaderGroupA);
    I(SCardIntroduceReaderGroupW);
    I(SCardIsValidContext);
    I(SCardListCardsA);
    I(SCardListCardsW);
    I(SCardListInterfacesA);
    I(SCardListInterfacesW);
    I(SCardListReaderGroupsA);
    I(SCardListReaderGroupsW);
    I(SCardListReadersA);
    I(SCardListReadersW);
    I(SCardListReadersWithDeviceInstanceIdA);
    I(SCardListReadersWithDeviceInstanceIdW);
    I(SCardLocateCardsA);
    I(SCardLocateCardsW);
    I(SCardLocateCardsByATRA);
    I(SCardLocateCardsByATRW);
    I(SCardReadCacheA);
    I(SCardReadCacheW);
    I(SCardReconnect);
    I(SCardReleaseContext);
    I(SCardRemoveReaderFromGroupA);
    I(SCardRemoveReaderFromGroupW);
    I(SCardSetAttrib);
    I(SCardSetCardTypeProviderNameA);
    I(SCardSetCardTypeProviderNameW);
    I(SCardStatusA);
    I(SCardStatusW);
    I(SCardTransmit);
    I(SCardWriteCacheA);
    I(SCardWriteCacheW);

    ATTACH(SCardConnectW);
    ATTACH(SCardConnectA);
    ATTACH(SCardTransmit);
    ATTACH(SCardCancel);
    ATTACH(SCardControl);
    ATTACH(SCardDisconnect);
    ATTACH(SCardEndTransaction);
    ATTACH(SCardEstablishContext);

    PVOID *ppbFailedPointer = nullptr;
    return DetourTransactionCommitEx(&ppbFailedPointer);
    #else
    return S_OK;
    #endif
    }

HRESULT Module::DetachDetours()
    {
    #ifdef USE_MSDETOURS
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourSetIgnoreTooSmall(TRUE);

    DETACH(SCardConnectW);
    DETACH(SCardConnectA);
    DETACH(SCardTransmit);
    DETACH(SCardBeginTransaction);
    DETACH(SCardCancel);
    DETACH(SCardControl);
    DETACH(SCardDisconnect);
    DETACH(SCardEndTransaction);
    DETACH(SCardEstablishContext);

    if (DetourTransactionCommit()) {
        PVOID *ppbFailedPointer = nullptr;
        return DetourTransactionCommitEx(&ppbFailedPointer);
        }
    #endif
    return S_OK;
    }

#ifdef USE_MSDETOURS

string ScardShareFlagsToString(const DWORD value)
    {
    stringstream r;
    switch (value) {
        case SCARD_SHARE_EXCLUSIVE: r << "SCARD_SHARE_EXCLUSIVE"; break;
        case SCARD_SHARE_SHARED:    r << "SCARD_SHARE_SHARED";    break;
        case SCARD_SHARE_DIRECT:    r << "SCARD_SHARE_DIRECT";    break;
        default: r << Any::FormatMessage("%016I32x", value); break;
        }
    return r.str();
    }

VOID DetDetach(PVOID *o, PVOID mine, const char* psz)
    {
    LONG l = DetourDetach(o,mine);
    }

VOID DetAttach(PVOID *o, PVOID mine, const char* psz)
    {
    PVOID pvReal = nullptr;
    if (o == nullptr) {
        o = &pvReal;
        }

    LONG l = DetourAttach(o, mine);
    if (l != 0) {
        //Decode((PBYTE)*ppvReal, 3);
        }
    }

LONG WINAPI H(SCardConnectW)(const SCARDCONTEXT Context, const LPCWSTR Reader,
    const DWORD ShareMode, const DWORD PreferredProtocols,
    LPSCARDHANDLE Card, LPDWORD ActiveProtocol)
    {
    TraceDescriptor D(nameof(SCardConnectW),"SCardConnectW(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,Reader,ShareMode,PreferredProtocols);
        r = O(SCardConnectW)(Context,Reader,ShareMode,PreferredProtocols,Card,ActiveProtocol);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context,Card,ActiveProtocol)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardConnectA)(const SCARDCONTEXT Context, const LPCSTR Reader,
    const DWORD ShareMode, const DWORD PreferredProtocols,
    LPSCARDHANDLE Card, LPDWORD ActiveProtocol)
    {
    TraceDescriptor D(nameof(SCardConnectW),"SCardConnectA(SCARDCONTEXT,LPCSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,Reader,ShareMode,PreferredProtocols);
        r = O(SCardConnectA)(Context,Reader,ShareMode,PreferredProtocols,Card,ActiveProtocol);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context,Card,ActiveProtocol)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardTransmit)(const SCARDHANDLE Card,LPCSCARD_IO_REQUEST SendPci,
    LPCBYTE SendBuffer,const DWORD SendLength, LPSCARD_IO_REQUEST RecvPci,
    LPBYTE RecvBuffer,LPDWORD RecvLength)
    {
    TraceDescriptor D(nameof(SCardTransmit),"SCardTransmit(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,SendPci,vector<uint8_t>(SendBuffer,SendBuffer + SendLength));
        r = O(SCardTransmit)(Card,SendPci,SendBuffer,SendLength,RecvPci,RecvBuffer,RecvLength);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card,RecvPci,vector<uint8_t>(RecvBuffer,RecvBuffer + *RecvLength))
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardBeginTransaction)(const SCARDCONTEXT Context)
    {
    TraceDescriptor D(nameof(SCardBeginTransaction),"SCardBeginTransaction(SCARDCONTEXT):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context);
        r = O(SCardBeginTransaction)(Context);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardCancel)(const SCARDCONTEXT Context)
    {
    TraceDescriptor D(nameof(SCardCancel),"SCardCancel(SCARDCONTEXT):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context);
        r = O(SCardCancel)(Context);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardControl)(SCARDHANDLE Card,DWORD ControlCode,LPCVOID InBuffer,
    DWORD InBufferSize,LPVOID OutBuffer,DWORD OutBufferSize,LPDWORD BytesReturned)
    {
    TraceDescriptor D(nameof(SCardControl),"SCardControl(SCARDHANDLE,DWORD,LPCVOID,DWORD,LPVOID,DWORD,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,ControlCode,vector<uint8_t>(LPCBYTE(InBuffer),LPCBYTE(InBuffer) + InBufferSize));
        r = O(SCardControl)(Card,ControlCode,InBuffer,InBufferSize,OutBuffer,OutBufferSize,BytesReturned);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card,vector<uint8_t>(LPBYTE(OutBuffer),LPBYTE(OutBuffer) + OutBufferSize),BytesReturned)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardDisconnect)(SCARDHANDLE Card,DWORD Disposition)
    {
    TraceDescriptor D(nameof(SCardDisconnect),"SCardDisconnect(SCARDHANDLE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,Disposition);
        r = O(SCardDisconnect)(Card,Disposition);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardEndTransaction)(SCARDHANDLE Card,DWORD Disposition)
    {
    TraceDescriptor D(nameof(SCardEndTransaction),"SCardEndTransaction(SCARDHANDLE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,Disposition);
        r = O(SCardEndTransaction)(Card,Disposition);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardEstablishContext)(DWORD Scope,LPCVOID,LPCVOID,LPSCARDCONTEXT Context)
    {
    TraceDescriptor D(nameof(SCardEstablishContext),"SCardEstablishContext(DWORD,LPCVOID,LPCVOID,LPSCARDCONTEXT):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Scope);
        r = O(SCardEstablishContext)(Scope,nullptr,nullptr,Context);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Scope,Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }
#endif
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
#define try       __try     {
#define finally } __finally {
#define end     }
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

    ATTACH(SCardGetReaderDeviceInstanceIdA);
    ATTACH(SCardGetReaderDeviceInstanceIdW);
    ATTACH(SCardAddReaderToGroupA);
    ATTACH(SCardAddReaderToGroupW);
    ATTACH(SCardAudit);
    ATTACH(SCardBeginTransaction);
    ATTACH(SCardCancel);
    ATTACH(SCardConnectA);
    ATTACH(SCardConnectW);
    ATTACH(SCardControl);
    ATTACH(SCardDisconnect);
    ATTACH(SCardEndTransaction);
    ATTACH(SCardEstablishContext);
    ATTACH(SCardForgetCardTypeA);
    ATTACH(SCardForgetCardTypeW);
    ATTACH(SCardForgetReaderA);
    ATTACH(SCardForgetReaderW);
    ATTACH(SCardForgetReaderGroupA);
    ATTACH(SCardForgetReaderGroupW);
    ATTACH(SCardFreeMemory);
    ATTACH(SCardGetAttrib);
    ATTACH(SCardGetCardTypeProviderNameA);
    ATTACH(SCardGetCardTypeProviderNameW);
    ATTACH(SCardGetDeviceTypeIdA);
    ATTACH(SCardGetDeviceTypeIdW);
    ATTACH(SCardGetProviderIdA);
    ATTACH(SCardGetProviderIdW);
    ATTACH(SCardGetReaderIconA);
    ATTACH(SCardGetReaderIconW);
    ATTACH(SCardGetStatusChangeA);
    ATTACH(SCardGetStatusChangeW);
    ATTACH(SCardGetTransmitCount);
    ATTACH(SCardIntroduceCardTypeA);
    ATTACH(SCardIntroduceCardTypeW);
    ATTACH(SCardIntroduceReaderA);
    ATTACH(SCardIntroduceReaderW);
    ATTACH(SCardIntroduceReaderGroupA);
    ATTACH(SCardIntroduceReaderGroupW);
    ATTACH(SCardIsValidContext);
    ATTACH(SCardListCardsA);
    ATTACH(SCardListCardsW);
    ATTACH(SCardListInterfacesA);
    ATTACH(SCardListInterfacesW);
    ATTACH(SCardListReaderGroupsA);
    ATTACH(SCardListReaderGroupsW);
    ATTACH(SCardListReadersA);
    ATTACH(SCardListReadersW);
    ATTACH(SCardListReadersWithDeviceInstanceIdA);
    ATTACH(SCardListReadersWithDeviceInstanceIdW);
    ATTACH(SCardLocateCardsA);
    ATTACH(SCardLocateCardsW);
    ATTACH(SCardLocateCardsByATRA);
    ATTACH(SCardLocateCardsByATRW);
    ATTACH(SCardReadCacheA);
    ATTACH(SCardReadCacheW);
    ATTACH(SCardReconnect);
    ATTACH(SCardReleaseContext);
    ATTACH(SCardRemoveReaderFromGroupA);
    ATTACH(SCardRemoveReaderFromGroupW);
    ATTACH(SCardSetAttrib);
    ATTACH(SCardSetCardTypeProviderNameA);
    ATTACH(SCardSetCardTypeProviderNameW);
    ATTACH(SCardStatusA);
    ATTACH(SCardStatusW);
    ATTACH(SCardTransmit);
    ATTACH(SCardWriteCacheA);
    ATTACH(SCardWriteCacheW);

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

    DETACH(SCardGetReaderDeviceInstanceIdA);
    DETACH(SCardGetReaderDeviceInstanceIdW);
    DETACH(SCardAddReaderToGroupA);
    DETACH(SCardAddReaderToGroupW);
    DETACH(SCardAudit);
    DETACH(SCardBeginTransaction);
    DETACH(SCardCancel);
    DETACH(SCardConnectA);
    DETACH(SCardConnectW);
    DETACH(SCardControl);
    DETACH(SCardDisconnect);
    DETACH(SCardEndTransaction);
    DETACH(SCardEstablishContext);
    DETACH(SCardForgetCardTypeA);
    DETACH(SCardForgetCardTypeW);
    DETACH(SCardForgetReaderA);
    DETACH(SCardForgetReaderW);
    DETACH(SCardForgetReaderGroupA);
    DETACH(SCardForgetReaderGroupW);
    DETACH(SCardFreeMemory);
    DETACH(SCardGetAttrib);
    DETACH(SCardGetCardTypeProviderNameA);
    DETACH(SCardGetCardTypeProviderNameW);
    DETACH(SCardGetDeviceTypeIdA);
    DETACH(SCardGetDeviceTypeIdW);
    DETACH(SCardGetProviderIdA);
    DETACH(SCardGetProviderIdW);
    DETACH(SCardGetReaderIconA);
    DETACH(SCardGetReaderIconW);
    DETACH(SCardGetStatusChangeA);
    DETACH(SCardGetStatusChangeW);
    DETACH(SCardGetTransmitCount);
    DETACH(SCardIntroduceCardTypeA);
    DETACH(SCardIntroduceCardTypeW);
    DETACH(SCardIntroduceReaderA);
    DETACH(SCardIntroduceReaderW);
    DETACH(SCardIntroduceReaderGroupA);
    DETACH(SCardIntroduceReaderGroupW);
    DETACH(SCardIsValidContext);
    DETACH(SCardListCardsA);
    DETACH(SCardListCardsW);
    DETACH(SCardListInterfacesA);
    DETACH(SCardListInterfacesW);
    DETACH(SCardListReaderGroupsA);
    DETACH(SCardListReaderGroupsW);
    DETACH(SCardListReadersA);
    DETACH(SCardListReadersW);
    DETACH(SCardListReadersWithDeviceInstanceIdA);
    DETACH(SCardListReadersWithDeviceInstanceIdW);
    DETACH(SCardLocateCardsA);
    DETACH(SCardLocateCardsW);
    DETACH(SCardLocateCardsByATRA);
    DETACH(SCardLocateCardsByATRW);
    DETACH(SCardReadCacheA);
    DETACH(SCardReadCacheW);
    DETACH(SCardReconnect);
    DETACH(SCardReleaseContext);
    DETACH(SCardRemoveReaderFromGroupA);
    DETACH(SCardRemoveReaderFromGroupW);
    DETACH(SCardSetAttrib);
    DETACH(SCardSetCardTypeProviderNameA);
    DETACH(SCardSetCardTypeProviderNameW);
    DETACH(SCardStatusA);
    DETACH(SCardStatusW);
    DETACH(SCardTransmit);
    DETACH(SCardWriteCacheA);
    DETACH(SCardWriteCacheW);

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
    if ((o != nullptr) && (*o == nullptr)) { return; }
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
    TraceDescriptor D(nameof(SCardConnectA),"SCardConnectA(SCARDCONTEXT,LPCSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD):LONG");
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
            ? D.Leave(scope, S_OK, r, Card)
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
    try
        D.Enter(scope,Scope);
        r = O(SCardEstablishContext)(Scope,nullptr,nullptr,Context);
    finally
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK,r,Scope,Context)
            : D.Leave(scope, r,    r);
    end
    return r;
    }

LONG WINAPI H(SCardAddReaderToGroupA)(SCARDCONTEXT Context,LPCSTR ReaderName,LPCSTR GroupName)
    {
    TraceDescriptor D(nameof(SCardAddReaderToGroupA),"SCardAddReaderToGroupA(SCARDCONTEXT,LPCSTR,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,ReaderName,GroupName);
        r = O(SCardAddReaderToGroupA)(Context,ReaderName,GroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardAddReaderToGroupW)(SCARDCONTEXT Context,LPCWSTR ReaderName,LPCWSTR GroupName)
    {
    TraceDescriptor D(nameof(SCardAddReaderToGroupW),"SCardAddReaderToGroupA(SCARDCONTEXT,LPCWSTR,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,ReaderName,GroupName);
        r = O(SCardAddReaderToGroupW)(Context,ReaderName,GroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardAudit)(SCARDCONTEXT Context,DWORD Event)
    {
    TraceDescriptor D(nameof(SCardAudit),"SCardAudit(SCARDCONTEXT,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,Event);
        r = O(SCardAudit)(Context,Event);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetCardTypeA)(SCARDCONTEXT Context,LPCSTR CardName)
    {
    TraceDescriptor D(nameof(SCardForgetCardTypeA),"SCardForgetCardTypeA(SCARDCONTEXT,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,CardName);
        r = O(SCardForgetCardTypeA)(Context,CardName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetCardTypeW)(SCARDCONTEXT Context,LPCWSTR CardName)
    {
    TraceDescriptor D(nameof(SCardForgetCardTypeW),"SCardForgetCardTypeW(SCARDCONTEXT,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,CardName);
        r = O(SCardForgetCardTypeW)(Context,CardName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetReaderA)(SCARDCONTEXT Context,LPCSTR ReaderName)
    {
    TraceDescriptor D(nameof(SCardForgetReaderA),"SCardForgetReaderA(SCARDCONTEXT,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,ReaderName);
        r = O(SCardForgetReaderA)(Context,ReaderName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetReaderW)(SCARDCONTEXT Context,LPCWSTR ReaderName)
    {
    TraceDescriptor D(nameof(SCardForgetReaderW),"SCardForgetReaderW(SCARDCONTEXT,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,ReaderName);
        r = O(SCardForgetReaderW)(Context,ReaderName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetReaderGroupA)(SCARDCONTEXT Context,LPCSTR GroupName)
    {
    TraceDescriptor D(nameof(SCardForgetReaderGroupA),"SCardForgetReaderGroupA(SCARDCONTEXT,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,GroupName);
        r = O(SCardForgetReaderGroupA)(Context,GroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardForgetReaderGroupW)(SCARDCONTEXT Context,LPCWSTR GroupName)
    {
    TraceDescriptor D(nameof(SCardForgetReaderGroupW),"SCardForgetReaderGroupW(SCARDCONTEXT,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,GroupName);
        r = O(SCardForgetReaderGroupW)(Context,GroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardFreeMemory)(SCARDCONTEXT Context,LPCVOID Mem)
    {
    TraceDescriptor D(nameof(SCardFreeMemory),"SCardFreeMemory(SCARDCONTEXT,LPCVOID):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Context,(DWORD_PTR)Mem);
        r = O(SCardFreeMemory)(Context,Mem);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Context)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetAttrib)(SCARDHANDLE Card,DWORD AttrId,LPBYTE Attr,LPDWORD AttrLen)
    {
    TraceDescriptor D(nameof(SCardGetAttrib),"SCardGetAttrib(SCARDHANDLE,DWORD,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,AttrId);
        r = O(SCardGetAttrib)(Card,AttrId,Attr,AttrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetCardTypeProviderNameA)(SCARDCONTEXT hContext,LPCSTR szCardName,DWORD dwProviderId,CHAR *szProvider,LPDWORD pcchProvider)
    {
    TraceDescriptor D(nameof(SCardGetCardTypeProviderNameA),"SCardGetCardTypeProviderNameA(SCARDCONTEXT,LPCSTR,DWORD,CHAR*,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwProviderId,szProvider);
        r = O(SCardGetCardTypeProviderNameA)(hContext,szCardName,dwProviderId,szProvider,pcchProvider);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pcchProvider)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetCardTypeProviderNameW)(SCARDCONTEXT hContext,LPCWSTR szCardName,DWORD dwProviderId,WCHAR *szProvider,LPDWORD pcchProvider)
    {
    TraceDescriptor D(nameof(SCardGetCardTypeProviderNameW),"SCardGetCardTypeProviderNameW(SCARDCONTEXT,LPCWSTR,DWORD,WCHAR*,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwProviderId,szProvider);
        r = O(SCardGetCardTypeProviderNameW)(hContext,szCardName,dwProviderId,szProvider,pcchProvider);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pcchProvider)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetDeviceTypeIdA)(SCARDCONTEXT hContext,LPCSTR szReaderName, LPDWORD pdwDeviceTypeId)
    {
    TraceDescriptor D(nameof(SCardGetDeviceTypeIdA),"SCardGetDeviceTypeIdA(SCARDCONTEXT,LPCSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName);
        r = O(SCardGetDeviceTypeIdA)(hContext,szReaderName,pdwDeviceTypeId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pdwDeviceTypeId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetDeviceTypeIdW)(SCARDCONTEXT hContext,LPCWSTR szReaderName, LPDWORD pdwDeviceTypeId)
    {
    TraceDescriptor D(nameof(SCardGetDeviceTypeIdW),"SCardGetDeviceTypeIdW(SCARDCONTEXT,LPCWSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName);
        r = O(SCardGetDeviceTypeIdW)(hContext,szReaderName,pdwDeviceTypeId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pdwDeviceTypeId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetProviderIdA)(SCARDCONTEXT hContext,LPCSTR szCard,LPGUID pguidProviderId)
    {
    TraceDescriptor D(nameof(SCardGetProviderIdA),"SCardGetProviderIdA(SCARDCONTEXT,LPCSTR,LPGUID):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCard);
        r = O(SCardGetProviderIdA)(hContext,szCard,pguidProviderId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pguidProviderId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetProviderIdW)(SCARDCONTEXT hContext,LPCWSTR szCard,LPGUID pguidProviderId)
    {
    TraceDescriptor D(nameof(SCardGetProviderIdW),"SCardGetProviderIdW(SCARDCONTEXT,LPCWSTR,LPGUID):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCard);
        r = O(SCardGetProviderIdW)(hContext,szCard,pguidProviderId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pguidProviderId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetReaderDeviceInstanceIdA)(SCARDCONTEXT hContext,LPCSTR szReaderName,LPSTR szDeviceInstanceId,LPDWORD pcchDeviceInstanceId)
    {
    TraceDescriptor D(nameof(SCardGetReaderDeviceInstanceIdA),"SCardGetReaderDeviceInstanceIdA(SCARDCONTEXT,LPCSTR,LPSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szDeviceInstanceId);
        r = O(SCardGetReaderDeviceInstanceIdA)(hContext,szReaderName,szDeviceInstanceId,pcchDeviceInstanceId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pcchDeviceInstanceId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetReaderDeviceInstanceIdW)(SCARDCONTEXT hContext,LPCWSTR szReaderName,LPWSTR szDeviceInstanceId,LPDWORD pcchDeviceInstanceId)
    {
    TraceDescriptor D(nameof(SCardGetReaderDeviceInstanceIdW),"SCardGetReaderDeviceInstanceIdW(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szDeviceInstanceId);
        r = O(SCardGetReaderDeviceInstanceIdW)(hContext,szReaderName,szDeviceInstanceId,pcchDeviceInstanceId);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,pcchDeviceInstanceId)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetReaderIconA)(SCARDCONTEXT hContext,LPCSTR szReaderName,LPBYTE pbIcon,LPDWORD pcbIcon)
    {
    TraceDescriptor D(nameof(SCardGetReaderIconA),"SCardGetReaderIconA(SCARDCONTEXT,LPCSTR,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName);
        r = O(SCardGetReaderIconA)(hContext,szReaderName,pbIcon,pcbIcon);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetReaderIconW)(SCARDCONTEXT hContext,LPCWSTR szReaderName,LPBYTE pbIcon,LPDWORD pcbIcon)
    {
    TraceDescriptor D(nameof(SCardGetReaderIconW),"SCardGetReaderIconW(SCARDCONTEXT,LPCWSTR,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName);
        r = O(SCardGetReaderIconW)(hContext,szReaderName,pbIcon,pcbIcon);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetStatusChangeA)(SCARDCONTEXT hContext,DWORD dwTimeout,LPSCARD_READERSTATEA rgReaderStates,DWORD cReaders)
    {
    TraceDescriptor D(nameof(SCardGetStatusChangeA),"SCardGetStatusChangeA(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEA,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,dwTimeout,cReaders);
        r = O(SCardGetStatusChangeA)(hContext,dwTimeout,rgReaderStates,cReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetStatusChangeW)(SCARDCONTEXT hContext,DWORD dwTimeout,LPSCARD_READERSTATEW rgReaderStates,DWORD cReaders)
    {
    TraceDescriptor D(nameof(SCardGetStatusChangeW),"SCardGetStatusChangeW(SCARDCONTEXT,DWORD,LPSCARD_READERSTATEW,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,dwTimeout,cReaders);
        r = O(SCardGetStatusChangeW)(hContext,dwTimeout,rgReaderStates,cReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardGetTransmitCount)(SCARDHANDLE hCard,LPDWORD pcTransmitCount)
    {
    TraceDescriptor D(nameof(SCardGetTransmitCount),"SCardGetTransmitCount(SCARDHANDLE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hCard);
        r = O(SCardGetTransmitCount)(hCard,pcTransmitCount);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hCard)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceCardTypeA)(
    SCARDCONTEXT hContext,
    LPCSTR szCardName,
    LPCGUID pguidPrimaryProvider,
    LPCGUID rgguidInterfaces,
    DWORD dwInterfaceCount,
    LPCBYTE pbAtr,
    LPCBYTE pbAtrMask,
    DWORD cbAtrLen)
    {
    TraceDescriptor D(nameof(SCardIntroduceCardTypeA),"SCardIntroduceCardTypeA(SCARDCONTEXT,LPCSTR ,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwInterfaceCount,cbAtrLen);
        r = O(SCardIntroduceCardTypeA)(hContext,szCardName,pguidPrimaryProvider,rgguidInterfaces,dwInterfaceCount,pbAtr,pbAtrMask,cbAtrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceCardTypeW)(
    SCARDCONTEXT hContext,
    LPCWSTR szCardName,
    LPCGUID pguidPrimaryProvider,
    LPCGUID rgguidInterfaces,
    DWORD dwInterfaceCount,
    LPCBYTE pbAtr,
    LPCBYTE pbAtrMask,
    DWORD cbAtrLen)
    {
    TraceDescriptor D(nameof(SCardIntroduceCardTypeW),"SCardIntroduceCardTypeW(SCARDCONTEXT,LPCWSTR,LPCGUID,LPCGUID,DWORD,LPCBYTE,LPCBYTE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwInterfaceCount,cbAtrLen);
        r = O(SCardIntroduceCardTypeW)(hContext,szCardName,pguidPrimaryProvider,rgguidInterfaces,dwInterfaceCount,pbAtr,pbAtrMask,cbAtrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceReaderA)(SCARDCONTEXT hContext,LPCSTR szReaderName,LPCSTR szDeviceName)
    {
    TraceDescriptor D(nameof(SCardIntroduceReaderA),"SCardIntroduceReaderA(SCARDCONTEXT,LPCSTR,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szDeviceName);
        r = O(SCardIntroduceReaderA)(hContext,szReaderName,szDeviceName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceReaderW)(SCARDCONTEXT hContext,LPCWSTR szReaderName,LPCWSTR szDeviceName)
    {
    TraceDescriptor D(nameof(SCardIntroduceReaderW),"SCardIntroduceReaderW(SCARDCONTEXT,LPCWSTR,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szDeviceName);
        r = O(SCardIntroduceReaderW)(hContext,szReaderName,szDeviceName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceReaderGroupA)(SCARDCONTEXT hContext,LPCSTR szGroupName)
    {
    TraceDescriptor D(nameof(SCardIntroduceReaderGroupA),"SCardIntroduceReaderGroupA(SCARDCONTEXT,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szGroupName);
        r = O(SCardIntroduceReaderGroupA)(hContext,szGroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIntroduceReaderGroupW)(SCARDCONTEXT hContext,LPCWSTR szGroupName)
    {
    TraceDescriptor D(nameof(SCardIntroduceReaderGroupW),"SCardIntroduceReaderGroupW(SCARDCONTEXT,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szGroupName);
        r = O(SCardIntroduceReaderGroupW)(hContext,szGroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardIsValidContext)(SCARDCONTEXT hContext)
    {
    TraceDescriptor D(nameof(SCardIsValidContext),"SCardIsValidContext(SCARDCONTEXT):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardIsValidContext)(hContext);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListCardsA)(
    SCARDCONTEXT hContext,
    LPCBYTE pbAtr,
    LPCGUID rgquidInterfaces,
    DWORD cguidInterfaceCount,
    CHAR *mszCards,
    LPDWORD pcchCards)
    {
    TraceDescriptor D(nameof(SCardListCardsA),"SCardListCardsA(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD,CHAR*,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardListCardsA)(hContext,pbAtr,rgquidInterfaces,cguidInterfaceCount,mszCards,pcchCards);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListCardsW)(
    SCARDCONTEXT hContext,
    LPCBYTE pbAtr,
    LPCGUID rgquidInterfaces,
    DWORD cguidInterfaceCount,
    WCHAR *mszCards,
    LPDWORD pcchCards)
    {
    TraceDescriptor D(nameof(SCardListCardsW),"SCardListCardsW(SCARDCONTEXT,LPCBYTE,LPCGUID,DWORD,WCHAR*,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardListCardsW)(hContext,pbAtr,rgquidInterfaces,cguidInterfaceCount,mszCards,pcchCards);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListInterfacesA)(
    SCARDCONTEXT hContext,
    LPCSTR szCard,
    LPGUID pguidInterfaces,
    LPDWORD pcguidInterfaces)
    {
    TraceDescriptor D(nameof(SCardListInterfacesA),"SCardListInterfacesA(SCARDCONTEXT,LPCSTR,LPGUID,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCard);
        r = O(SCardListInterfacesA)(hContext,szCard,pguidInterfaces,pcguidInterfaces);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListInterfacesW)(
    SCARDCONTEXT hContext,
    LPCWSTR szCard,
    LPGUID pguidInterfaces,
    LPDWORD pcguidInterfaces)
    {
    TraceDescriptor D(nameof(SCardListInterfacesW),"SCardListInterfacesW(SCARDCONTEXT,LPCWSTR,LPGUID,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCard);
        r = O(SCardListInterfacesW)(hContext,szCard,pguidInterfaces,pcguidInterfaces);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListReaderGroupsA)(
    SCARDCONTEXT hContext,
    LPSTR mszGroups,
    LPDWORD pcchGroups)
    {
    TraceDescriptor D(nameof(SCardListReaderGroupsA),"SCardListReaderGroupsA(SCARDCONTEXT,LPSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,mszGroups);
        r = O(SCardListReaderGroupsA)(hContext,mszGroups,pcchGroups);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListReaderGroupsW)(
    SCARDCONTEXT hContext,
    LPWSTR mszGroups,
    LPDWORD pcchGroups)
    {
    TraceDescriptor D(nameof(SCardListReaderGroupsW),"SCardListReaderGroupsW(SCARDCONTEXT,LPWSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,mszGroups);
        r = O(SCardListReaderGroupsW)(hContext,mszGroups,pcchGroups);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

template<class T, class E>
LONG SCardListReadersT(const TraceDescriptor& D, T EntryPoint, SCARDCONTEXT hContext,const E* mszGroups,E* mszReaders,LPDWORD pcchReaders)
    {
    LONG Scope = 0;
    LONG r = 0;
    auto IsAutoAllocate = false;
    try
        D.Enter(Scope,hContext,mszGroups,mszReaders,pcchReaders);
        IsAutoAllocate = (pcchReaders != nullptr)
            ? *pcchReaders == SCARD_AUTOALLOCATE
            : false;
        r = EntryPoint(hContext,mszGroups,mszReaders,pcchReaders);
    finally
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(Scope, S_OK, r,hContext, ((pcchReaders != nullptr) && (mszReaders != nullptr))
                ? (IsAutoAllocate)
                    ? vector<E>(*(E**)mszReaders,*(E**)mszReaders + *pcchReaders)
                    : vector<E>(mszReaders,mszReaders + *pcchReaders)
                : vector<E>())
            : D.Leave(Scope, r, r);
    end
    return r;
    }
#pragma region SCardListReadersA
LONG WINAPI H(SCardListReadersA)(SCARDCONTEXT hContext,LPCSTR mszGroups,LPSTR mszReaders,LPDWORD pcchReaders)
    {
    return SCardListReadersT(
        TraceDescriptor(nameof(SCardListReadersA),"SCardListReadersA(SCARDCONTEXT,LPCSTR,LPSTR,LPDWORD):LONG"),
        O(SCardListReadersA),hContext,mszGroups,mszReaders,pcchReaders);
    }
#pragma endregion
#pragma region SCardListReadersW
LONG WINAPI H(SCardListReadersW)(SCARDCONTEXT hContext,LPCWSTR mszGroups,LPWSTR mszReaders,LPDWORD pcchReaders)
    {
    return SCardListReadersT(
        TraceDescriptor(nameof(SCardListReadersW),"SCardListReadersW(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD):LONG"),
        O(SCardListReadersW),hContext,mszGroups,mszReaders,pcchReaders);
    }
#pragma endregion

LONG WINAPI H(SCardListReadersWithDeviceInstanceIdA)(
    SCARDCONTEXT hContext,
    LPCSTR szDeviceInstanceId,
    LPSTR mszReaders,
    LPDWORD pcchReaders)
    {
    TraceDescriptor D(nameof(SCardListReadersWithDeviceInstanceIdA),"SCardListReadersWithDeviceInstanceIdA(SCARDCONTEXT,LPCSTR,LPSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szDeviceInstanceId,mszReaders);
        r = O(SCardListReadersWithDeviceInstanceIdA)(hContext,szDeviceInstanceId,mszReaders,pcchReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardListReadersWithDeviceInstanceIdW)(
    SCARDCONTEXT hContext,
    LPCWSTR szDeviceInstanceId,
    LPWSTR mszReaders,
    LPDWORD pcchReaders)
    {
    TraceDescriptor D(nameof(SCardListReadersWithDeviceInstanceIdW),"SCardListReadersWithDeviceInstanceIdW(SCARDCONTEXT,LPCWSTR,LPWSTR,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szDeviceInstanceId,mszReaders);
        r = O(SCardListReadersWithDeviceInstanceIdW)(hContext,szDeviceInstanceId,mszReaders,pcchReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

#pragma region SCardLocateCardsA
LONG WINAPI H(SCardLocateCardsA)(SCARDCONTEXT hContext,LPCSTR mszCards,LPSCARD_READERSTATEA rgReaderStates,DWORD cReaders) {
    TraceDescriptor D(nameof(SCardLocateCardsA),"SCardLocateCardsA(SCARDCONTEXT,LPCSTR,LPSCARD_READERSTATEA,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    list<LPSCARD_READERSTATEA> States;
    try
        if (rgReaderStates != nullptr) {
            for (DWORD i = 0; i < cReaders; i++) {
                States.push_back(&rgReaderStates[i]);
                }
            }
        D.Enter(scope,hContext,mszCards,States,cReaders);
        r = O(SCardLocateCardsA)(hContext,mszCards,rgReaderStates,cReaders);
    finally
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext,States,cReaders)
            : D.Leave(scope, r,    r);
    end
    return r;
    }
#pragma endregion
#pragma region SCardLocateCardsW
LONG WINAPI H(SCardLocateCardsW)(SCARDCONTEXT hContext,LPCWSTR mszCards,LPSCARD_READERSTATEW rgReaderStates,DWORD cReaders) {
    TraceDescriptor D(nameof(SCardLocateCardsW),"SCardLocateCardsW(SCARDCONTEXT,LPCWSTR,LPSCARD_READERSTATEW,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    try
        D.Enter(scope,hContext,mszCards);
        r = O(SCardLocateCardsW)(hContext,mszCards,rgReaderStates,cReaders);
    finally
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    end
    return r;
    }
#pragma endregion

LONG WINAPI H(SCardLocateCardsByATRA)(
    SCARDCONTEXT hContext,
    LPSCARD_ATRMASK rgAtrMasks,
    DWORD cAtrs,
    LPSCARD_READERSTATEA rgReaderStates,
    DWORD cReaders)
    {
    TraceDescriptor D(nameof(SCardLocateCardsByATRA),"SCardLocateCardsByATRA(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEA,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardLocateCardsByATRA)(hContext,rgAtrMasks,cAtrs,rgReaderStates,cReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardLocateCardsByATRW)(
    SCARDCONTEXT hContext,
    LPSCARD_ATRMASK rgAtrMasks,
    DWORD cAtrs,
    LPSCARD_READERSTATEW rgReaderStates,
    DWORD cReaders)
    {
    TraceDescriptor D(nameof(SCardLocateCardsByATRW),"SCardLocateCardsByATRW(SCARDCONTEXT,LPSCARD_ATRMASK,DWORD,LPSCARD_READERSTATEW,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardLocateCardsByATRW)(hContext,rgAtrMasks,cAtrs,rgReaderStates,cReaders);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardReadCacheA)(
    SCARDCONTEXT hContext,
    UUID *CardIdentifier,
    DWORD FreshnessCounter,
    LPSTR LookupName,
    PBYTE Data,
    DWORD *DataLen)
    {
    TraceDescriptor D(nameof(SCardReadCacheA),"SCardReadCacheA(SCARDCONTEXT,UUID*,DWORD,LPSTR,PBYTE,DWORD*):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardReadCacheA)(hContext,CardIdentifier,FreshnessCounter,LookupName,Data,DataLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardReadCacheW)(
    SCARDCONTEXT hContext,
    UUID *CardIdentifier,
    DWORD FreshnessCounter,
    LPWSTR LookupName,
    PBYTE Data,
    DWORD *DataLen)
    {
    TraceDescriptor D(nameof(SCardReadCacheW),"SCardReadCacheW(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD*):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardReadCacheW)(hContext,CardIdentifier,FreshnessCounter,LookupName,Data,DataLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardReconnect)(
    SCARDHANDLE hCard,
    DWORD dwShareMode,
    DWORD dwPreferredProtocols,
    DWORD dwInitialization,
    LPDWORD pdwActiveProtocol)
    {
    TraceDescriptor D(nameof(SCardReconnect),"SCardReconnect(SCARDHANDLE,DWORD,DWORD,DWORD,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hCard,dwShareMode,dwPreferredProtocols,dwInitialization);
        r = O(SCardReconnect)(hCard,dwShareMode,dwPreferredProtocols,dwInitialization,pdwActiveProtocol);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hCard)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardReleaseContext)(SCARDCONTEXT hContext)
    {
    TraceDescriptor D(nameof(SCardReleaseContext),"SCardReleaseContext(SCARDCONTEXT):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardReleaseContext)(hContext);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardRemoveReaderFromGroupA)(
    _In_ SCARDCONTEXT hContext,
    _In_ LPCSTR szReaderName,
    _In_ LPCSTR szGroupName)
    {
    TraceDescriptor D(nameof(SCardRemoveReaderFromGroupA),"SCardRemoveReaderFromGroupA(SCARDCONTEXT,LPCSTR,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szGroupName);
        r = O(SCardRemoveReaderFromGroupA)(hContext,szReaderName,szGroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardRemoveReaderFromGroupW)(
    _In_ SCARDCONTEXT hContext,
    _In_ LPCWSTR szReaderName,
    _In_ LPCWSTR szGroupName)
    {
    TraceDescriptor D(nameof(SCardRemoveReaderFromGroupW),"SCardRemoveReaderFromGroupW(SCARDCONTEXT,LPCWSTR,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szReaderName,szGroupName);
        r = O(SCardRemoveReaderFromGroupW)(hContext,szReaderName,szGroupName);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardSetAttrib)(
    SCARDHANDLE hCard,
    DWORD dwAttrId,
    LPCBYTE pbAttr,
    DWORD cbAttrLen)
    {
    TraceDescriptor D(nameof(SCardSetAttrib),"SCardSetAttrib(SCARDHANDLE,DWORD,LPCBYTE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hCard,dwAttrId);
        r = O(SCardSetAttrib)(hCard,dwAttrId,pbAttr,cbAttrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hCard)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardSetCardTypeProviderNameA)(
    SCARDCONTEXT hContext,
    LPCSTR szCardName,
    DWORD dwProviderId,
    LPCSTR szProvider)
    {
    TraceDescriptor D(nameof(SCardSetCardTypeProviderNameA),"SCardSetCardTypeProviderNameA(SCARDCONTEXT,LPCSTR,DWORD,LPCSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwProviderId,szProvider);
        r = O(SCardSetCardTypeProviderNameA)(hContext,szCardName,dwProviderId,szProvider);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardSetCardTypeProviderNameW)(
    SCARDCONTEXT hContext,
    LPCWSTR szCardName,
    DWORD dwProviderId,
    LPCWSTR szProvider)
    {
    TraceDescriptor D(nameof(SCardSetCardTypeProviderNameW),"SCardSetCardTypeProviderNameW(SCARDCONTEXT,LPCWSTR,DWORD,LPCWSTR):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext,szCardName,dwProviderId,szProvider);
        r = O(SCardSetCardTypeProviderNameW)(hContext,szCardName,dwProviderId,szProvider);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardStatusA)(
    SCARDHANDLE hCard,
    LPSTR mszReaderNames,
    LPDWORD pcchReaderLen,
    LPDWORD pdwState,
    LPDWORD pdwProtocol,
    LPBYTE pbAtr,
    LPDWORD pcbAtrLen)
    {
    TraceDescriptor D(nameof(SCardStatusA),"SCardStatusA(SCARDHANDLE,LPSTR,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hCard,mszReaderNames);
        r = O(SCardStatusA)(hCard,mszReaderNames,pcchReaderLen,pdwState,pdwProtocol,pbAtr,pcbAtrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hCard)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardStatusW)(
    SCARDHANDLE hCard,
    LPWSTR mszReaderNames,
    LPDWORD pcchReaderLen,
    LPDWORD pdwState,
    LPDWORD pdwProtocol,
    LPBYTE pbAtr,
    LPDWORD pcbAtrLen)
    {
    TraceDescriptor D(nameof(SCardStatusW),"SCardStatusW(SCARDHANDLE,LPWSTR,LPDWORD,LPDWORD,LPDWORD,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hCard,mszReaderNames);
        r = O(SCardStatusW)(hCard,mszReaderNames,pcchReaderLen,pdwState,pdwProtocol,pbAtr,pcbAtrLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hCard)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardWriteCacheA)(SCARDCONTEXT hContext,UUID *CardIdentifier,DWORD FreshnessCounter,LPSTR LookupName,PBYTE Data,DWORD DataLen)
    {
    TraceDescriptor D(nameof(SCardWriteCacheA),"SCardWriteCacheA(SCARDCONTEXT,UUID*,DWORD,LPSTR,PBYTE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardWriteCacheA)(hContext,CardIdentifier,FreshnessCounter,LookupName,Data,DataLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

LONG WINAPI H(SCardWriteCacheW)(SCARDCONTEXT hContext,UUID *CardIdentifier,DWORD FreshnessCounter,LPWSTR LookupName,PBYTE Data,DWORD DataLen)
    {
    TraceDescriptor D(nameof(SCardWriteCacheW),"SCardWriteCacheW(SCARDCONTEXT,UUID*,DWORD,LPWSTR,PBYTE,DWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,hContext);
        r = O(SCardWriteCacheW)(hContext,CardIdentifier,FreshnessCounter,LookupName,Data,DataLen);
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, hContext)
            : D.Leave(scope, r,    r);
    END
    return r;
    }

#endif
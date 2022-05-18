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

ModuleSafeHandle WinSCard(LoadLibraryA("winscard.dll"));

VOID DetDetach(PVOID*,PVOID,const char*);
VOID DetAttach(PVOID*,PVOID,const char*);

#define H(E) H##E
#define O(E) O##E
#define T(E) T##E
#define TRY       __try     {
#define FINALLY } __finally {
#define END     }
#define ATTACH(E)           DetAttach(&(PVOID&)O##E,H##E,#E)
#define DETACH(E)           DetDetach(&(PVOID&)O##E,H##E,#E)
#define GetProcAddressT(Module,E)  (T(E))GetProcAddress(Module,#E)
#define ToString(E)         E##ToString

typedef LONG(WINAPI* T(SCardConnectW))(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
typedef LONG(WINAPI* T(SCardConnectA))(SCARDCONTEXT,LPCSTR ,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
typedef LONG(WINAPI* T(SCardTransmit))(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);
typedef LONG(WINAPI* T(SCardBeginTransaction))(SCARDHANDLE);
typedef LONG(WINAPI* T(SCardCancel))(SCARDHANDLE);
typedef LONG(WINAPI* T(SCardControl))(SCARDHANDLE,DWORD,LPCVOID,DWORD,LPVOID,DWORD,LPDWORD);
typedef LONG(WINAPI* T(SCardDisconnect))(SCARDHANDLE,DWORD);
typedef LONG(WINAPI* T(SCardEndTransaction))(SCARDHANDLE,DWORD);

T(SCardConnectA) O(SCardConnectA) = nullptr;
T(SCardConnectW) O(SCardConnectW) = nullptr;
T(SCardTransmit) O(SCardTransmit) = nullptr;
T(SCardBeginTransaction) O(SCardBeginTransaction) = nullptr;
T(SCardCancel)  O(SCardCancel)  = nullptr;
T(SCardControl) O(SCardControl) = nullptr;
T(SCardDisconnect) O(SCardDisconnect) = nullptr;

LONG WINAPI H(SCardConnectW)(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardConnectA)(SCARDCONTEXT,LPCSTR ,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardTransmit)(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);
LONG WINAPI H(SCardBeginTransaction)(SCARDHANDLE);
LONG WINAPI H(SCardCancel)(SCARDHANDLE);
LONG WINAPI H(SCardControl)(SCARDHANDLE,DWORD,LPCVOID,DWORD,LPVOID,DWORD,LPDWORD);
LONG WINAPI H(SCardDisconnect)(SCARDHANDLE,DWORD);

#endif

HRESULT Module::AttachDetours()
    {
    #ifdef USE_MSDETOURS
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourSetIgnoreTooSmall(TRUE);

    O(SCardConnectA) = GetProcAddressT(WinSCard,SCardConnectA);
    O(SCardConnectW) = GetProcAddressT(WinSCard,SCardConnectW);
    O(SCardTransmit) = GetProcAddressT(WinSCard,SCardTransmit);
    O(SCardBeginTransaction) = GetProcAddressT(WinSCard,SCardBeginTransaction);
    O(SCardCancel)  = GetProcAddressT(WinSCard,SCardCancel);
    O(SCardControl) = GetProcAddressT(WinSCard,SCardControl);
    O(SCardDisconnect) = GetProcAddressT(WinSCard,SCardDisconnect);

    ATTACH(SCardConnectW);
    ATTACH(SCardConnectA);
    ATTACH(SCardTransmit);
    ATTACH(SCardCancel);
    ATTACH(SCardControl);
    ATTACH(SCardDisconnect);

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

#endif
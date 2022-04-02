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
typedef LONG(WINAPI* T(SCardTransmit))(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);

T(SCardConnectW) O(SCardConnectW) = nullptr;
T(SCardTransmit) O(SCardTransmit) = nullptr;

LONG WINAPI H(SCardConnectW)(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
LONG WINAPI H(SCardTransmit)(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD);
#endif

HRESULT Module::AttachDetours()
    {
    #ifdef USE_MSDETOURS
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourSetIgnoreTooSmall(TRUE);

    O(SCardConnectW) = GetProcAddressT(WinSCard,SCardConnectW);
    O(SCardTransmit) = GetProcAddressT(WinSCard,SCardTransmit);

    ATTACH(SCardConnectW);
    ATTACH(SCardTransmit);

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
    DETACH(SCardTransmit);

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

LONG WINAPI H(SCardConnectW)(SCARDCONTEXT Context,LPCWSTR Reader,DWORD ShareMode,DWORD PreferredProtocols,LPSCARDHANDLE Card,LPDWORD ActiveProtocol) {
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

LONG WINAPI H(SCardTransmit)(SCARDHANDLE Card,LPCSCARD_IO_REQUEST SendPci,LPCBYTE SendBuffer,DWORD SendLength,
                             LPSCARD_IO_REQUEST RecvPci,LPBYTE RecvBuffer,LPDWORD RecvLength)
    {
    TraceDescriptor D(nameof(SCardTransmit),"SCardTransmit(SCARDHANDLE,LPCSCARD_IO_REQUEST,LPCBYTE,DWORD,LPSCARD_IO_REQUEST,LPBYTE,LPDWORD):LONG");
    LONG scope = 0;
    LONG r = 0;
    TRY
        D.Enter(scope,Card,SendPci,vector<uint8_t>(SendBuffer,SendBuffer + SendLength));
        r = O(SCardTransmit)(Card,SendPci,SendBuffer,SendLength,RecvPci,RecvBuffer,RecvLength);
        //if (r == 0) {
        //    #ifdef _WIN64
        //    LoggingSource::Log(LoggingSeverity::Debug, {
        //        {"Response", nameof(SCardTransmit)},
        //        {"Parameters", {
        //            {nameof(Card),Any::FormatMessage("%016I64x", Card)},
        //            {nameof(RecvBuffer),{RecvBuffer,RecvLength}},
        //            {"RecvBuffer{Decoded}",ScardDecoder::DecodeTransmitResponse(SendPci->dwProtocol,{SendBuffer,SendLength},{RecvBuffer,*RecvLength})},
        //            {nameof(RecvPci),RecvPci},
        //            {"{RetVal}", (scode(r)).str()}
        //            }
        //        }});
        //    #else
        //    LoggingSource::Log(LoggingSeverity::Debug, {
        //        {"Response", nameof(SCardTransmit)},
        //        {"Parameters", {
        //            {nameof(Card),Any::FormatMessage("%08I32x", Card)},
        //            {nameof(RecvBuffer),{RecvBuffer,RecvLength}},
        //            {"RecvBuffer{Decoded}",ScardDecoder::DecodeTransmitResponse(SendPci->dwProtocol,{SendBuffer,SendLength},{RecvBuffer,*RecvLength})},
        //            {nameof(RecvPci),RecvPci},
        //            {"{RetVal}", (scode(r)).str()}
        //            }
        //        }});
        //    #endif
        //    }
    FINALLY
        r = (r == SCARD_S_SUCCESS)
            ? D.Leave(scope, S_OK, r, Card,RecvPci,vector<uint8_t>(RecvBuffer,RecvBuffer + *RecvLength))
            : D.Leave(scope, r,    r);
    END
    return r;
    }
#endif
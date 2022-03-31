#include "hdrstop.h"
#include "module.h"
#include "json.h"
#include "logging.h"
#include "feature.h"
#include "scard.h"
#include "scode.h"

#undef FormatMessage

#undef  THIS_FILE
#define THIS_FILE "detours.cc"
#undef  nameof
#define nameof(E) string(#E)

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
    LONG r = 0;
    TRY
        #ifdef _WIN64
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Request", nameof(SCardConnectW)},
            {"Parameters", {
                {nameof(Context),Any::FormatMessage("%016I64x", Context)},
                {nameof(Reader), Reader},
                {nameof(ShareMode), ScardShareFlagsToString(ShareMode)},
                {nameof(PreferredProtocols), ObjectValue::ScardProtocolToString(PreferredProtocols)}
                }
            }});
        #else
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Request", nameof(SCardConnectW)},
            {"Parameters", {
                {nameof(Context),Any::FormatMessage("%016I32x", Context)},
                {nameof(Reader), Reader},
                {nameof(ShareMode), ScardShareFlagsToString(ShareMode)},
                {nameof(PreferredProtocols), ObjectValue::ScardProtocolToString(PreferredProtocols)}
                }
            }});
        #endif
        r = O(SCardConnectW)(Context,Reader,ShareMode,PreferredProtocols,Card,ActiveProtocol);
        #ifdef _WIN64
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Response", nameof(SCardConnectW)},
            {"Parameters", {
                {nameof(Context),Any::FormatMessage("%016I64x", Context)},
                {nameof(Card), (Card != nullptr) ? Any::FormatMessage("{%016I64x}:%016I64x", Card, *Card) : "{null}"},
                {nameof(ActiveProtocol), (ActiveProtocol != nullptr) ? Any::FormatMessage("{%016I64x}:%s", ActiveProtocol, ObjectValue::ScardProtocolToString(*ActiveProtocol).c_str()) : "{null}"},
                {"{RetVal}", (scode(r)).str()}
                }
            }});
        #else
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Response", nameof(SCardConnectW)},
            {"Parameters", {
                {nameof(Context),Any::FormatMessage("%08I32x", Context)},
                {nameof(Card), (Card != nullptr) ? Any::FormatMessage("{%08I32x}:%08I32x", Card, *Card) : "{null}"},
                {nameof(ActiveProtocol), (ActiveProtocol != nullptr) ? Any::FormatMessage("{%08I32x}:%s", ActiveProtocol, ObjectValue::ScardProtocolToString(*ActiveProtocol).c_str()) : "{null}"},
                {"{RetVal}", (scode(r)).str()}
                }
            }});
        #endif
    FINALLY
    END
    return r;
    }

LONG WINAPI H(SCardTransmit)(SCARDHANDLE Card,LPCSCARD_IO_REQUEST SendPci,LPCBYTE SendBuffer,DWORD SendLength,
                             LPSCARD_IO_REQUEST RecvPci,LPBYTE RecvBuffer,LPDWORD RecvLength)
    {
    LONG r = 0;
    TRY
        #ifdef _WIN64
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Request", nameof(SCardTransmit)},
            {"Parameters", {
                {nameof(Card),Any::FormatMessage("%016I64x", Card)},
                {nameof(SendBuffer),Any(SendBuffer,SendLength)},
                {"SendBuffer{Decoded}",ScardDecoder::DecodeTransmitRequest(SendPci->dwProtocol,{SendBuffer,SendLength})},
                {nameof(SendPci),SendPci},
                {nameof(RecvPci),RecvPci}
                }
            }});
        #else
        LoggingSource::Log(LoggingSeverity::Debug, {
            {"Request", nameof(SCardTransmit)},
            {"Parameters", {
                {nameof(Card),Any::FormatMessage("%08I32x", Card)},
                {nameof(SendBuffer),Any(SendBuffer,SendLength)},
                {"SendBuffer{Decoded}",ScardDecoder::DecodeTransmitRequest(SendPci->dwProtocol,{SendBuffer,SendLength})},
                {nameof(SendPci),SendPci},
                {nameof(RecvPci),RecvPci}
                }
            }});
        #endif
        r = O(SCardTransmit)(Card,SendPci,SendBuffer,SendLength,RecvPci,RecvBuffer,RecvLength);
        if (r == 0) {
            #ifdef _WIN64
            LoggingSource::Log(LoggingSeverity::Debug, {
                {"Response", nameof(SCardTransmit)},
                {"Parameters", {
                    {nameof(Card),Any::FormatMessage("%016I64x", Card)},
                    {nameof(RecvBuffer),{RecvBuffer,RecvLength}},
                    {"RecvBuffer{Decoded}",ScardDecoder::DecodeTransmitResponse(SendPci->dwProtocol,{SendBuffer,SendLength},{RecvBuffer,*RecvLength})},
                    {nameof(RecvPci),RecvPci},
                    {"{RetVal}", (scode(r)).str()}
                    }
                }});
            #else
            LoggingSource::Log(LoggingSeverity::Debug, {
                {"Response", nameof(SCardTransmit)},
                {"Parameters", {
                    {nameof(Card),Any::FormatMessage("%08I32x", Card)},
                    {nameof(RecvBuffer),{RecvBuffer,RecvLength}},
                    {"RecvBuffer{Decoded}",ScardDecoder::DecodeTransmitResponse(SendPci->dwProtocol,{SendBuffer,SendLength},{RecvBuffer,*RecvLength})},
                    {nameof(RecvPci),RecvPci},
                    {"{RetVal}", (scode(r)).str()}
                    }
                }});
            #endif
            }
    FINALLY
    END
    return r;
    }
#endif
#include "hdrstop.h"
#include "module.h"
#include "json.h"
#include "logging.h"

#undef  THIS_FILE
#define THIS_FILE "detours.cc"

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

typedef LONG(WINAPI* T(SCardConnectW))(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);

T(SCardConnectW) O(SCardConnectW) = nullptr;

LONG WINAPI H(SCardConnectW)(SCARDCONTEXT,LPCWSTR,DWORD,DWORD,LPSCARDHANDLE,LPDWORD);
#endif

HRESULT Module::AttachDetours()
    {
    #ifdef USE_MSDETOURS
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourSetIgnoreTooSmall(TRUE);

    O(SCardConnectW) = GetProcAddressT(WinSCard,SCardConnectW);

    ATTACH(SCardConnectW);

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

    if (DetourTransactionCommit()) {
        PVOID *ppbFailedPointer = nullptr;
        return DetourTransactionCommitEx(&ppbFailedPointer);
        }
    #endif
    return S_OK;
    }

#ifdef USE_MSDETOURS

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
        {
        stringstream o;
        JsonWriter<char> writer(__EFILESRC__,o);
        writer.WriteStartObject();
        writer.WritePropertyName("Request").WriteValue(nameof(SCardConnectW));
        writer.WritePropertyName("Parameters");
          writer.WriteStartObject();
            writer.WritePropertyName(nameof(Context)).WriteValue(Context);
            writer.WritePropertyName(nameof(Reader)).WriteValue(Reader);
            writer.WritePropertyName(nameof(ShareMode)).WriteValue(ShareMode);
            writer.WritePropertyName(nameof(PreferredProtocols)).WriteValue(PreferredProtocols);
          writer.WriteEndObject();
        writer.WriteEndObject();
        LoggingSource::Log(LoggingSeverity::Debug, o.str());
        }
        r = O(SCardConnectW)(Context,Reader,ShareMode,PreferredProtocols,Card,ActiveProtocol);
        {
        stringstream o;
        JsonWriter<char> writer(__EFILESRC__,o);
        writer.WriteStartObject();
        writer.WritePropertyName("Response").WriteValue(nameof(SCardConnectW));
        writer.WritePropertyName("Parameters");
          writer.WriteStartObject();
            writer.WritePropertyName(nameof(Context)).WriteValue(Context);
            if (Card) {
                writer.WritePropertyName(nameof(Card)).WriteValue(*Card);
                }
            if (ActiveProtocol) {
                writer.WritePropertyName(nameof(ActiveProtocol)).WriteValue(*ActiveProtocol);
                }
          writer.WriteEndObject();
        writer.WriteEndObject();
        LoggingSource::Log(LoggingSeverity::Debug, o.str());
        }
    FINALLY
    END
    return r;
    }
#endif
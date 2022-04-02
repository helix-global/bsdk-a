#include "hdrstop.h"
#include "module.h"
#include "logging.h"

#undef FormatMessage
#undef QueryFullProcessImageName
#undef GetModuleFileName

LONG Module::s_nTlsIndent = -1;
LONG Module::s_nTlsThread = -1;
LONG Module::s_nThreadCnt =  0;
HMODULE Module::Instance = nullptr;
string Module::ProcessName = "{none}";
string Module::ModuleName  = "{none}";
string Module::FullProcessName  = "{none}";

BOOL Module::ProcessAttach(const HMODULE Module)
    {
    HRESULT hr;
    Instance = Module;
    s_nTlsIndent = TlsAlloc();
    s_nTlsThread = TlsAlloc();
    ModuleName      = Path::GetFileName(GetModuleFileName<char>(Module));
    FullProcessName = GetModuleFileName<char>(nullptr);
    ProcessName     = Path::GetFileName(FullProcessName);
    LoggingSource::Log(LoggingSeverity::Info, "Module::ProcessAttach{%p:%s}", Module,ProcessName.c_str());
    ThreadAttach(Module);
    if (FAILED(hr = AttachDetours())) {
        LoggingSource::Log(LoggingSeverity::Error,
            "Error attaching detours. %s",
            FormatMessage<char>(hr).c_str());
        }
    return TRUE;
    }

BOOL Module::ProcessDetach(const HMODULE Module)
    {
    HRESULT hr;
    LoggingSource::Log(LoggingSeverity::Info, "Module::ProcessDetach{%p}", Module);
    ThreadDetach(Module);
    if (FAILED(hr = DetachDetours())) {
        LoggingSource::Log(LoggingSeverity::Error,
            "Error detaching detours. %s",
            FormatMessage<char>(hr).c_str());
        }
    if (s_nTlsIndent >= 0) { TlsFree(s_nTlsIndent); }
    if (s_nTlsThread >= 0) { TlsFree(s_nTlsThread); }
    return TRUE;
    }

BOOL Module::ThreadAttach(const HMODULE Module)
    {
    LoggingSource::Log(LoggingSeverity::Info, "Module::ThreadAttach{%p}", Module);
    if (s_nTlsIndent >= 0) { TlsSetValue(s_nTlsIndent, reinterpret_cast<LPVOID>(0)); }
    if (s_nTlsThread >= 0) {
        const auto nThread = InterlockedIncrement(&s_nThreadCnt);
        TlsSetValue(s_nTlsThread, (PVOID)(LONG_PTR)nThread);
        }
    return TRUE;
    }

BOOL Module::ThreadDetach(const HMODULE Module)
    {
    LoggingSource::Log(LoggingSeverity::Info, "Module::ThreadDetach{%p}", Module);
    if (s_nTlsIndent >= 0) { TlsSetValue(s_nTlsIndent, reinterpret_cast<LPVOID>(0)); }
    if (s_nTlsThread >= 0) { TlsSetValue(s_nTlsThread, reinterpret_cast<LPVOID>(0)); }
    return TRUE;
    }

#define ModuleSpecialization(SUFFIX,E) \
template<> HRESULT Module::GetModuleFileName<E>(const HMODULE Module,E* Target,const DWORD Size) { \
    GetModuleFileName##SUFFIX(Module,Target,Size); \
    return GetLastError(); \
    } \
template<> HRESULT Module::QueryFullProcessImageName<E>(const HMODULE Module,const DWORD Flags,E* Target,DWORD* Size) { \
    return QueryFullProcessImageName##SUFFIX(Module,Flags,Target,Size) \
        ? S_OK \
        : GetLastError(); \
    }

ModuleSpecialization(A, CHAR)
ModuleSpecialization(W,WCHAR)

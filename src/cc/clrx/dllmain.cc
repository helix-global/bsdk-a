#include "hdrstop.h"
#include <yvals.h>
#include "exception.h"

#undef  THIS_FILE
#define THIS_FILE "dllmain.cc"

BOOL APIENTRY DllMain(HMODULE hModule,DWORD ul_reason_for_call,LPVOID lpReserved)
    {
    switch (ul_reason_for_call)
        {
        case DLL_PROCESS_ATTACH:
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
        }
    return TRUE;
    }

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Outptr_ LPVOID* r)
    {
    #pragma push_macro("__EFUNCSIG__")
    #undef  __EFUNCSIG__
    #define __EFUNCSIG__ "HRESULT DllGetClassObject(REFCLSID,REFIID,LPVOID*)"
    if (r == nullptr) { return E_INVALIDARG; }
    _TRY_BEGIN
    _CATCH_ALL
        const auto scode = (HRESULT)GetLastError();
        SetErrorInfo(0, new HResultException(THIS_FILE,__LINE__,__EFUNCSIG__,scode));
        return scode;
    _CATCH_END
    return E_NOINTERFACE;
    #pragma pop_macro("__EFUNCSIG__")
    }

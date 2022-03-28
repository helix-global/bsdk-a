#pragma once
#include "object.h"

#pragma push_macro("QueryFullProcessImageName")
#pragma push_macro("GetModuleFileName")
#pragma push_macro("FormatMessage")
#undef QueryFullProcessImageName
#undef GetModuleFileName
#undef FormatMessage

class Module : public Object<IUnknown>
    {
public:
    static BOOL ProcessAttach(HMODULE Module);
    static BOOL ProcessDetach(HMODULE Module);
    static BOOL ThreadAttach(HMODULE Module);
    static BOOL ThreadDetach(HMODULE Module);
private:
    static HRESULT AttachDetours();
    static HRESULT DetachDetours();
private:
    static LONG s_nTlsIndent;
    static LONG s_nTlsThread;
    static LONG s_nThreadCnt;
    static HMODULE Instance;
public:
    static string ProcessName;
    static string ModuleName;
private:
    template<class E> static HRESULT QueryFullProcessImageName(HMODULE Module,DWORD Flags,E* Target,DWORD* Size);
    template<class E> static HRESULT GetModuleFileName(HMODULE Module,E* Target,DWORD Size);
    template<class E> static basic_string<E> GetModuleFileName(HMODULE Module) {
        vector<E> Target;
        for (DWORD Size = 0x800;;Size <<= 1) {
            HRESULT hr;
            Target.resize(Size);
            if ((hr = GetModuleFileName<E>(Module,&Target[0],Size)) == S_OK) {
                return &Target[0];
                }
            if (hr != ERROR_INSUFFICIENT_BUFFER) {
                ATLTRACE("Module.GetModuleFileName:{%s}\n", FormatMessage<char>(hr).c_str());
                return basic_string<E>();
                }
            }
        }
    };

#pragma pop_macro("FormatMessage")
#pragma pop_macro("GetModuleFileName")
#pragma pop_macro("QueryFullProcessImageName")

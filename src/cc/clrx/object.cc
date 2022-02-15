#include "hdrstop.h"
#include "object.h"

Object<IUnknown>::Object()
    :m_ref(1)
    {
    }

Object<IUnknown>::~Object()
    {
    }

#pragma region M:QueryInterface(REFIID,void**):HRESULT
HRESULT STDMETHODCALLTYPE Object<IUnknown>::QueryInterface(REFIID riid, _COM_Outptr_ void __RPC_FAR *__RPC_FAR *r) {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = nullptr;
    if (riid == IID_IUnknown) {
        const auto i = static_cast<IUnknown*>(this);
        *r = i;
        AddRef();
        return S_OK;
        };
    #ifdef FEATURE_FREE_THREADED_MARSHALER
    if (riid == IID_IMarshal) {
        if (EnsureMarshaler() == S_OK) {
            marshaler->QueryInterface(riid, r);
            return S_OK;
            }
        };
    #endif
    return E_NOINTERFACE;
    }
#pragma endregion
#pragma region M:AddRef:ULONG
ULONG STDMETHODCALLTYPE Object<IUnknown>::AddRef()
    {
    return InterlockedIncrement(&m_ref);
    }
#pragma endregion
#pragma region M:Release:ULONG
ULONG STDMETHODCALLTYPE Object<IUnknown>::Release() {
    #ifdef FEATURE_FREE_THREADED_MARSHALER
    if (destroying) { return 0; }
    #endif
    InterlockedDecrement(&m_ref);
    if (m_ref == 0) {
        #ifdef FEATURE_FREE_THREADED_MARSHALER
        destroying = true;
        #endif
        try
            {
            #ifdef FEATURE_FREE_THREADED_MARSHALER
            marshaler = nullptr;
            #endif
            delete this;
            }
        catch (...)
            {
            }
        return 0;
        }
    return m_ref;
    }
#pragma endregion

HRESULT STDMETHODCALLTYPE Object<IDispatch>::QueryInterface(REFIID riid,void** r) {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = nullptr;
    if (Object<IUnknown>::QueryInterface(riid, r) == S_OK) { return S_OK; }
    if (riid == IID_IDispatch) {
        const auto i = static_cast<IDispatch*>(this);
        *r = i;
        AddRef();
        return S_OK;
        };
    return E_NOINTERFACE;
    }

ULONG STDMETHODCALLTYPE Object<IDispatch>::AddRef()
    {
    return Object<IUnknown>::AddRef();
    }

ULONG STDMETHODCALLTYPE Object<IDispatch>::Release()
    {
    return Object<IUnknown>::Release();
    }

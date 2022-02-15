#pragma once

#include <unknwn.h>
#include <string>

using namespace std;

template<class... T> class Object;
template<> class Object<IUnknown> :
    public virtual IUnknown
    {
protected:
    Object();
public:
    virtual ~Object();
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, _COM_Outptr_ void __RPC_FAR *__RPC_FAR *r) override;
    ULONG STDMETHODCALLTYPE AddRef()  override;
    ULONG STDMETHODCALLTYPE Release() override;
protected:
    Object(const Object&) = default;
    Object(Object&&)      = default;
    Object& operator=(const Object&) = default;
    Object& operator=(Object&&)      = default;
protected:
    ULONG m_ref;
    };

template<class... T> class Object<IUnknown,T...>:
    public virtual Object<IUnknown>,
    public virtual T...
    {
protected:
    Object()
        {
        }
public:
    virtual ~Object() {
        }
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, _COM_Outptr_ void __RPC_FAR *__RPC_FAR *r) override;
    ULONG STDMETHODCALLTYPE AddRef()  override { return Object<IUnknown>::AddRef();  }
    ULONG STDMETHODCALLTYPE Release() override { return Object<IUnknown>::AddRef();  }
protected:
    Object(const Object&) = default;
    Object(Object&&)      = default;
    Object& operator=(const Object&) = default;
    Object& operator=(Object&&)      = default;
    };

template<> class Object<IDispatch> :
    public virtual Object<IUnknown>,
    public virtual IDispatch
    {
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,void** r) override;
    ULONG STDMETHODCALLTYPE AddRef()  override;
    ULONG STDMETHODCALLTYPE Release() override;
public:
    HRESULT STDMETHODCALLTYPE GetTypeInfoCount(UINT*) override { return E_NOTIMPL; }
    HRESULT STDMETHODCALLTYPE GetTypeInfo(UINT,LCID,ITypeInfo**) override { return E_NOTIMPL; }
    HRESULT STDMETHODCALLTYPE GetIDsOfNames(REFIID,LPOLESTR*,UINT,LCID,DISPID*) override { return E_NOTIMPL; }
    HRESULT STDMETHODCALLTYPE Invoke(DISPID,REFIID,LCID,WORD,DISPPARAMS*,VARIANT*,EXCEPINFO*,UINT*) override { return E_NOTIMPL; }
    };

template<class... T> class Object<IDispatch,T...>:
    public virtual Object<IDispatch>,
    public virtual T...
    {
protected:
    Object()
        {
        }
private:
    template<class I, class... T> struct QueryInterfaceT;
    template<class I> struct QueryInterfaceT<I>
        {
        template<class S>
        HRESULT operator()(S self, REFIID riid, void** r){
            if (riid == __uuidof(I)) {
                auto i = static_cast<I*>(self);
                *r = i;
                i->AddRef();
                return S_OK;
                }
            return E_NOINTERFACE;
            }
        };
    template<class I, class... T> struct QueryInterfaceT
        {
        template<class S>
        HRESULT operator()(S self, REFIID riid, void** r){
            return !SUCCEEDED((QueryInterfaceT<I>()).operator()(self, riid,r))
                ? (QueryInterfaceT<T...>()).operator()(self, riid,r)
                : S_OK;
            }
        };
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,void** r) override {
        if (r == nullptr) { return E_INVALIDARG; }
        *r = nullptr;
        return !SUCCEEDED(Object<IDispatch>::QueryInterface(riid, r))
            ? (QueryInterfaceT<T...>()).operator()(this, riid,r)
            : S_OK;
        }
    ULONG STDMETHODCALLTYPE AddRef()  override { return Object<IDispatch>::AddRef();  }
    ULONG STDMETHODCALLTYPE Release() override { return Object<IDispatch>::Release(); }
public:
    HRESULT STDMETHODCALLTYPE GetTypeInfoCount(UINT* pctinfo) override { return Object<IDispatch>::GetTypeInfoCount(pctinfo); }
    HRESULT STDMETHODCALLTYPE GetTypeInfo(UINT iTInfo,LCID lcid,ITypeInfo** ppTInfo) override { return Object<IDispatch>::GetTypeInfo(iTInfo,lcid,ppTInfo); }
    HRESULT STDMETHODCALLTYPE GetIDsOfNames(REFIID riid,LPOLESTR* rgszNames,UINT cNames,LCID lcid,DISPID* rgDispId) override { return Object<IDispatch>::GetIDsOfNames(riid,rgszNames,cNames,lcid,rgDispId); }
    HRESULT STDMETHODCALLTYPE Invoke(
        DISPID dispIdMember,
        REFIID riid,
        LCID lcid,
        WORD wFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        UINT* puArgErr) override
        {
        return Object<IDispatch>::Invoke(dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr);
        }
public:
    virtual ~Object() {
        }
protected:
    Object(const Object&) = default;
    Object(Object&&)      = default;
    Object& operator=(const Object&) = default;
    Object& operator=(Object&&)      = default;
    };

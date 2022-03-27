#pragma once

#include <unknwn.h>
#include <string>
#include <memory>
#include <codecvt>
#include <vector>
#include <unordered_set>

#pragma push_macro("FormatMessage")
#undef FormatMessage

using namespace std;

class ObjectReference
    {
public:
    ULONG Count;
public:
    ObjectReference(ULONG r):
        Count(r)
        {
        }
    };

template<class T>
class ComPtr
    {
public:
    ComPtr():
        r(nullptr)
        {
        }
    ComPtr(const T* o):
        r(const_cast<T*>(o))
        {
        AddRef();
        EnsureReference();
        }
    ComPtr(T* o):
        r(o)
        {
        AddRef();
        EnsureReference();
        }
    ComPtr(const ComPtr<T>& o):
        r(o.r),Reference(o.Reference)
        {
        AddRef();
        }
    ComPtr(ComPtr<T>&& o):
        r(o.r),Reference(o.Reference)
        {
        AddRef();
        }
    ~ComPtr()
        {
        Release();
        }
    ComPtr(nullptr_t):
        r(nullptr)
        {
        }
public:
    bool operator == (T* o) const { return r == o; }
    T** operator &()       { return &r; }
    T&  operator *() const { return *r; }
    T*  operator->() const { return  r; }
    operator T*()    const { return  r; }
public:
    ComPtr<T>& operator=(T* o) {
        Release();
        r = o;
        EnsureReference();
        AddRef();
        return *this;
        }
    ComPtr<T>& operator=(const ComPtr<T>& o) {
        Release();
        r = o.r;
        Reference = o.Reference;
        AddRef();
        return *this;
        }
    ComPtr<T>& operator=(ComPtr<T>&& o) {
        Release();
        r = o.r;
        Reference = o.Reference;
        AddRef();
        return *this;
        }
public:
    T* Detach() {
        const auto o = r;
        r = nullptr;
        Reference = nullptr;
        return o;
        }

    void Attach(T* o)
        {
        EnsureReference();
        Release();
        r = o;
        EnsureReference();
        }

    template <class Q>
    HRESULT QueryInterface(Q** o) const
        {
        return r->QueryInterface(__uuidof(Q), (void**)o);
        }
private:
    void Release()
        {
        if (r) {
            if (Reference != nullptr) {
                if (Reference->Count > 0) {
                    r->Release();
                    }
                }
            else
                {
                r->Release();
                }
            }
        r = nullptr;
        Reference = nullptr;
        }
    ULONG AddRef()
        {
        return (r)
            ? r->AddRef()
            : 0;
        }
public:
    bool IsValidPointer()
        {
        if (!r) { return false; }
        if (Reference != nullptr) {
            return (Reference->Count > 0);
            }
        return true;
        }
private:
    void EnsureReference() const;
public:
    mutable shared_ptr<ObjectReference> Reference;
private:
    T* r;
    };

template<class T, class... P> inline ComPtr<T> ComPtrM(P&&... args) {
    auto o = new T(args...);
    auto r = ComPtr<T>(o);
    r.Reference = o->Reference;
    ((T*)r)->Release();
    return r;
    }

template<class I, class T, class... P> inline ComPtr<I> ComPtrM(P&&... args) {
    auto o = new T(args...);
    auto r = ComPtr<I>(o);
    r.Reference = o->Reference;
    ((I*)r)->Release();
    return r;
    }

MIDL_INTERFACE("10ed0688-d361-4308-8870-9a20b10df6b7")
IObject : virtual IUnknown
    {
    virtual shared_ptr<ObjectReference> GetReference() = 0;
    };

class ObjectSource
    {
public:
    ObjectSource(const string& FileName, int Line);
    ObjectSource(const ObjectSource& Other);
public:
    string FileName;
    int Line;
    };

template<class... T> class Object;
template<> class Object<IUnknown> :
    public virtual IUnknown,
    public virtual IObject
    {
protected:
    Object(const ObjectSource& ObjectSource);
public:
    virtual ~Object();
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,void** r) override;
    ULONG STDMETHODCALLTYPE AddRef()  override;
    ULONG STDMETHODCALLTYPE Release() override;
    shared_ptr<ObjectReference> GetReference() override;
protected:
    Object(const Object&) = default;
    Object(Object&&)      = default;
    Object& operator=(const Object&) = default;
    Object& operator=(Object&&)      = default;
public:
    shared_ptr<ObjectReference> Reference;
    ObjectSource Source;
    bool Disposing,Disposed;
public:
    virtual string TypeName() const;
protected:
    #ifdef FEATURE_FREE_THREADED_MARSHALER
    ComPtr<IUnknown> Marshaler;
    virtual HRESULT EnsureMarshaler() {
        _ASSERT(this != nullptr);
        _ASSERT(!Disposed);
        if (Marshaler == nullptr) {
            HRESULT hr;
            if ((hr = CoCreateFreeThreadedMarshaler(this, &Marshaler)) != S_OK) { return hr; }
            }
        return S_OK;
        }
    #endif
public:
    static HANDLE Mutex;
    static unordered_set<Object<IUnknown>*> Instances;
public:
    template<class E> static basic_string<E> FormatMessage(time_t value,const E* format);
    template<class E> static basic_string<E> FormatMessage(const E* format, va_list args);
    template<class E> static basic_string<E> FormatMessage(const E* format, ...) {
        va_list args;
        va_start(args, format);
        return FormatMessage<E>(format,args);
        va_end(args);
        }
    template<> static basic_string< CHAR> FormatMessage(const  CHAR* format, va_list args);
    template<> static basic_string<WCHAR> FormatMessage(const WCHAR* format, va_list args);
    };

template<class T> void ComPtr<T>::EnsureReference() const {
    if (r) {
        if (Reference == nullptr) {
            IObject* o = nullptr;
            if (SUCCEEDED(QueryInterface(&o))) {
                Reference = o->GetReference();
                o->Release();
                return;
                }
            }
        }
    Reference = nullptr;
    }

template<class... T> class Object<IUnknown,T...>:
    public Object<IUnknown>,
    public virtual T...
    {
protected:
    Object(const ObjectSource& ObjectSource):
        Object<IUnknown>(ObjectSource)
        {
        }
public:
    virtual ~Object() {
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
        _ASSERT(this != nullptr);
        _ASSERT(!Disposed);
        if (r == nullptr) { return E_INVALIDARG; }
        *r = nullptr;
        return !SUCCEEDED(Object<IUnknown>::QueryInterface(riid, r))
            ? (QueryInterfaceT<T...>()).operator()(this, riid,r)
            : S_OK;
        }
    ULONG STDMETHODCALLTYPE AddRef()  override { return __super::AddRef();  }
    ULONG STDMETHODCALLTYPE Release() override { return __super::Release(); }
protected:
    Object(const Object&) = default;
    Object(Object&&)      = default;
    Object& operator=(const Object&) = default;
    Object& operator=(Object&&)      = default;
    };

template<> class Object<IDispatch> :
    public Object<IUnknown>,
    public virtual IDispatch
    {
protected:
    Object(const ObjectSource& ObjectSource);
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,void** r) override;
    ULONG STDMETHODCALLTYPE AddRef()  override;
    ULONG STDMETHODCALLTYPE Release() override;
public:
    STDMETHOD(GetTypeInfoCount)(UINT*) override { return E_NOTIMPL; }
    STDMETHOD(GetTypeInfo)(UINT,LCID,ITypeInfo**) override { return E_NOTIMPL; }
    STDMETHOD(GetIDsOfNames)(REFIID,LPOLESTR*,UINT,LCID,DISPID*) override { return E_NOTIMPL; }
    STDMETHOD(Invoke)(DISPID,REFIID,LCID,WORD,DISPPARAMS*,VARIANT*,EXCEPINFO*,UINT*) override { return E_NOTIMPL; }
public:
    string TypeName() const override;
    };

template<class... T> class Object<IDispatch,T...>:
    public Object<IDispatch>,
    public virtual T...
    {
protected:
    Object(const ObjectSource& ObjectSource):
        Object<IDispatch>(ObjectSource)
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
        _ASSERT(this != nullptr);
        _ASSERT(!Disposed);
        if (r == nullptr) { return E_INVALIDARG; }
        *r = nullptr;
        return !SUCCEEDED(Object<IDispatch>::QueryInterface(riid, r))
            ? (QueryInterfaceT<T...>()).operator()(this, riid,r)
            : S_OK;
        }
    ULONG STDMETHODCALLTYPE AddRef()  override { return __super::AddRef();  }
    ULONG STDMETHODCALLTYPE Release() override { return __super::Release(); }
public:
    STDMETHOD(GetTypeInfoCount)(UINT* pctinfo) override { return __super::GetTypeInfoCount(pctinfo); }
    STDMETHOD(GetTypeInfo)(UINT iTInfo,LCID lcid,ITypeInfo** ppTInfo) override { return __super::GetTypeInfo(iTInfo,lcid,ppTInfo); }
    STDMETHOD(GetIDsOfNames)(REFIID riid,LPOLESTR* rgszNames,UINT cNames,LCID lcid,DISPID* rgDispId) override { return __super::GetIDsOfNames(riid,rgszNames,cNames,lcid,rgDispId); }
    STDMETHOD(Invoke)(
        DISPID dispIdMember,
        REFIID riid,
        LCID lcid,
        WORD wFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        UINT* puArgErr) override
        {
        return __super::Invoke(dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr);
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

#define __EFUNCSRC__ (::ExceptionSource(::ObjectSource({THIS_FILE,__LINE__}),__EFUNCSIG__))
#define __EFILESRC__ (::ObjectSource(THIS_FILE,__LINE__))

class MutexLock
    {
public:
    explicit MutexLock(HANDLE MutexObject):
        MutexObject(MutexObject)
        {
        if (MutexObject != nullptr) {
            WaitForSingleObject(MutexObject,INFINITE);
            }
        }
    ~MutexLock() {
        if (MutexObject != nullptr) {
            ReleaseMutex(MutexObject);
            MutexObject = nullptr;
            }
        }
private:
    MutexLock(const MutexLock&) = delete;
private:
    HANDLE MutexObject;
    };

struct Path
    {
    static wstring GetFileName(const wstring&);
    static  string GetFileName(const  string&);
    };

#pragma pop_macro("FormatMessage")

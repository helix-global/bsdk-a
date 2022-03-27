#include "hdrstop.h"
#include "object.h"

#pragma push_macro("FormatMessage")
#undef FormatMessage

unordered_set<Object<IUnknown>*> Object<IUnknown>::Instances;
HANDLE Object<IUnknown>::Mutex = nullptr;
string  Object<IUnknown>::TypeName() const { return "Object<IUnknown>" ; }
string Object<IDispatch>::TypeName() const { return "Object<IDispatch>"; }

template<> basic_string<CHAR> Object<IUnknown>::FormatMessage<CHAR>(const  CHAR* format, va_list args)
    {
    size_t size;
    vector<CHAR> o((size = _vscprintf(format,args)) + 1);
    vsprintf_s(&o[0],size,format,args);
    return &o[0];
    }

template<> basic_string<CHAR> Object<IUnknown>::FormatMessage(time_t value,const CHAR* format)
    {
    char o[64];
    tm r;
    localtime_s(&r, &value);
    strftime(o, 64, format, &r);
    return o;
    }

Object<IUnknown>::Object(const ObjectSource& Source):
    Reference(make_shared<ObjectReference>(1)),Source(Source),Disposing(false),Disposed(false)
    {
    MutexLock lock(Mutex);
    Instances.insert(this);
    }

Object<IUnknown>::~Object()
    {
    MutexLock lock(Mutex);
    Reference->Count = 0;
    Instances.erase(this);
    Disposed = true;
    Reference = nullptr;
    ATLTRACE(L"Object<IUnknown>::Finalize{%p}\n", this);
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
    _ASSERT(this != nullptr);
    _ASSERT(!Disposed);
    const auto r = InterlockedIncrement(&Reference->Count);
    ATLTRACE(L"%S::AddRef{%p}:%i\n", TypeName().c_str(), this, r);
    return r;
    }
#pragma endregion
#pragma region M:Release:ULONG
ULONG STDMETHODCALLTYPE Object<IUnknown>::Release() {
    _ASSERT(this != nullptr);
    _ASSERT(!Disposed);
    #ifdef FEATURE_FREE_THREADED_MARSHALER
    if (Disposing) { return 0; }
    #endif
    const auto r = InterlockedDecrement(&Reference->Count);
    ATLTRACE(L"%S::Release{%p}:%i\n", TypeName().c_str(), this, r);
    if (r == 0) {
        Disposing = true;
        try
            {
            #ifdef FEATURE_FREE_THREADED_MARSHALER
            Marshaler = nullptr;
            #endif
            delete this;
            }
        catch (...)
            {
            }
        return 0;
        }
    return r;
    }
#pragma endregion

shared_ptr<ObjectReference> Object<IUnknown>::GetReference() {
    return Reference;
    }

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

Object<IDispatch>::Object(const ObjectSource& Source):
    Object<IUnknown>(Source)
    {
    }

ObjectSource::ObjectSource(const ObjectSource& Other):
    FileName(Other.FileName),Line(Other.Line)
    {
    }

ObjectSource::ObjectSource(const string& FileName, int Line):
    FileName(FileName),Line(Line)
    {
    }

#pragma pop_macro("FormatMessage")
#pragma once

#include "object.h"
#import "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\mscorlib.tlb" \
    rename("or", "objref") \
    rename("ReportEvent", "MSCorReportEvent") \
    raw_interfaces_only

#define THIS_FILE    __FILE__
#define __EFUNCSIG__ __FUNCSIG__

typedef mscorlib::_Exception IException;
typedef mscorlib::_SerializationInfo ISerializationInfo;
typedef mscorlib::StreamingContext StreamingContext;
typedef mscorlib::_MethodBase IMethodBase;
typedef mscorlib::_Type IType;

class Exception;
class ExceptionSource
    {
public:
    ExceptionSource(const ObjectSource& ObjectSource, const string& Source);
    ExceptionSource(const string& FileName, int Line, const string& Source);
    ExceptionSource(const ExceptionSource&);
    ExceptionSource(ExceptionSource&&);
public:
    ComPtr<Exception> GetExceptionForHR(HRESULT);
    ComPtr<Exception> GetExceptionForHR(HRESULT,const string&);
    ComPtr<Exception> GetExceptionForHR(HRESULT,const char*,...);
    ComPtr<Exception> GetExceptionForHR(HRESULT,const ComPtr<Exception>&);
    ComPtr<Exception> GetExceptionForHR(HRESULT,const ComPtr<Exception>&,const  string&);
    ComPtr<Exception> GetExceptionForHR(HRESULT,const ComPtr<Exception>&,const wstring&);
    ComPtr<Exception> GetErrorInfo(HRESULT) const;
    ComPtr<Exception> PushStack(const ComPtr<Exception>&) const;
public:
    ObjectSource ObjectSource;
    string Source;
private:
    friend class E;
    };

class Exception : public Object<
        IDispatch,
        IException,
        IErrorInfo>
    {
public:
    typedef Object<IDispatch,IException,IErrorInfo> BaseType;
public:
    Exception(const ExceptionSource& Source);
public:
    HRESULT STDMETHODCALLTYPE get_ToString(BSTR*) override;
    HRESULT STDMETHODCALLTYPE Equals(VARIANT,VARIANT_BOOL*) override;
    HRESULT STDMETHODCALLTYPE GetHashCode(LONG*) override;
    HRESULT STDMETHODCALLTYPE GetType(IType**) override;
    HRESULT STDMETHODCALLTYPE GetBaseException(IException**) override;
    HRESULT STDMETHODCALLTYPE GetObjectData(ISerializationInfo*,StreamingContext) override;
    HRESULT STDMETHODCALLTYPE get_InnerException(IException**) override;
    HRESULT STDMETHODCALLTYPE get_TargetSite(IMethodBase**) override;
private:
    STDMETHOD(get_HelpLink  )(BSTR*) override;
    STDMETHOD(put_HelpLink  )(BSTR ) override;
    STDMETHOD(get_Source    )(BSTR*) override;
    STDMETHOD(put_Source    )(BSTR ) override;
    STDMETHOD(get_Message   )(BSTR*) override;
    STDMETHOD(get_StackTrace)(BSTR*) override;
private:
    HRESULT STDMETHODCALLTYPE GetGUID(GUID*) override;
    HRESULT STDMETHODCALLTYPE GetSource(BSTR*) override;
    HRESULT STDMETHODCALLTYPE GetDescription(BSTR*) override;
    HRESULT STDMETHODCALLTYPE GetHelpFile(BSTR*) override;
    HRESULT STDMETHODCALLTYPE GetHelpContext(DWORD*) override;
private:
    CComBSTR HelpLink,Source,HelpFile,Message,StackTrace;
    DWORD HelpContext;
protected:
    CComPtr<IException> InnerException;
public:
    ComPtr<Exception> PushStack(const string& FileName, const int Line, const string& Source);
    ComPtr<Exception> PushStack(const ObjectSource&, const string& Source);
    ComPtr<Exception> PushStack(const ExceptionSource&);
    };

class HResultException : public Exception
    {
public:
    HResultException(const ExceptionSource& Source, HRESULT SCode);
private:
    HRESULT SCode;
    };

#pragma pack(push)
#pragma pack(1)
template<class T> class __declspec(novtable) NullableReference{
public:
    BOOL HasValue;
    T Value;
public:
    NullableReference():
        HasValue(false),Value(T())
        {
        }
    NullableReference(const T& value):
        HasValue(true),Value(value)
        {
        }
    NullableReference(const NullableReference<T>& value):
        HasValue(value.HasValue),Value(value.Value)
        {
        }
    NullableReference(const nullptr_t):
        HasValue(false),Value(T())
        {
        }
public:
    NullableReference<T>& operator=(const NullableReference<T>& value) {
        HasValue = value.HasValue;
        Value = value.Value;
        return *this;
        }
    NullableReference<T>& operator=(NullableReference<T>&& value) {
        HasValue = value.HasValue;
        Value = value.Value;
        return *this;
        }
    template<class P>
    NullableReference<T>& operator=(const P& value) {
        HasValue = true;
        Value = value;
        return *this;
        }
public:
    operator T() {
        if (!HasValue) {
            throw __EFUNCSRC__.GetExceptionForHR(COR_E_NULLREFERENCE);
            }
        return Value;
        }
    bool operator == (const nullptr_t r) const {
        return (r == nullptr)
            ? !HasValue
            :  HasValue;
        }
    bool operator != (const nullptr_t r) const {
        return (r != nullptr)
            ?  HasValue
            : !HasValue;
        }
public:
    static const NullableReference& Empty;
    };
#pragma pack(pop)

template<class T> const NullableReference<T>& NullableReference<T>::Empty = NullableReference<T>();

template<typename E, typename T> E union_cast(const T& value) {
    union U
        {
        T first;
        E second;
        } u;
    u.first = value;
    return u.second;
    }

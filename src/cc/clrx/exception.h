#pragma once

#include "object.h"
#import "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\mscorlib.tlb" \
    rename("or", "objref") \
    rename("ReportEvent", "MSCorReportEvent") \
    raw_interfaces_only


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

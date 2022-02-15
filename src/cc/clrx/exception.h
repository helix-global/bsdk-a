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

class Exception : public Object<
        IDispatch,
        IException,
        IErrorInfo>
    {
public:
    Exception(const string& FileName, int LineNumber, const string& Source);
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
    };

class HResultException : public Exception
    {
public:
    HResultException(const string& FileName, int LineNumber, const string& Source, HRESULT SCode);
private:
    HRESULT SCode;
    };

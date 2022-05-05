#include "hdrstop.h"
#include "exception.h"
#include <ostream>

ExceptionSource::ExceptionSource(ExceptionSource&& o):
    ObjectSource(o.ObjectSource),Source(o.Source)
    {
    }

ExceptionSource::ExceptionSource(const ExceptionSource& o):
    ObjectSource(o.ObjectSource),Source(o.Source)
    {
    }

ExceptionSource::ExceptionSource(const ::ObjectSource& ObjectSource, const string& Source):
    ObjectSource(ObjectSource),Source(Source)
    {
    }

ExceptionSource::ExceptionSource(const string& FileName, int Line, const string& Source):
    ObjectSource(FileName,Line),Source(Source)
    {
    }

ComPtr<Exception> ExceptionSource::PushStack(const ComPtr<Exception>& Target) const
    {
    return Target->PushStack(*this);
    }

Exception::Exception(const ExceptionSource& Source):
    BaseType(Source.ObjectSource),HelpContext(-1)
    {
    }

HResultException::HResultException(const ExceptionSource& Source, const HRESULT SCode):
    Exception(Source),SCode(SCode)
    {
    }

HRESULT Exception::GetGUID(GUID* r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    RtlZeroMemory(&(*r), sizeof(*r));
    return S_OK;
    }

HRESULT Exception::GetHelpContext(DWORD* r) {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = HelpContext;
    return S_OK;
    }

HRESULT Exception::GetSource(BSTR* r)      { return Source.CopyTo(r);     }
HRESULT Exception::get_Source(BSTR* r)     { return Source.CopyTo(r);     }
HRESULT Exception::GetHelpFile(BSTR* r)    { return HelpFile.CopyTo(r);   }
HRESULT Exception::get_HelpLink(BSTR* r)   { return HelpLink.CopyTo(r);   }
HRESULT Exception::get_Message(BSTR* r)    { return Message.CopyTo(r);    }
HRESULT Exception::get_StackTrace(BSTR* r) { return StackTrace.CopyTo(r); }
HRESULT Exception::get_ToString(BSTR* r)   { return get_Message(r); }
HRESULT Exception::GetDescription(BSTR* r) { return get_Message(r); }

HRESULT Exception::get_InnerException(IException** r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = InnerException;
    return S_OK;
    }

HRESULT Exception::get_TargetSite(IMethodBase** r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = nullptr;
    return S_OK;
    }

HRESULT Exception::put_HelpLink(const BSTR r)
    {
    HelpLink.Empty();
    HelpLink.Attach(r);
    return S_OK;
    }

HRESULT Exception::put_Source(const BSTR r)
    {
    Source.Empty();
    Source.Attach(r);
    return S_OK;
    }

HRESULT Exception::Equals(VARIANT, VARIANT_BOOL* r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = FALSE;
    return S_OK;
    }

HRESULT Exception::GetBaseException(IException** r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = nullptr;
    return S_OK;
    }

HRESULT Exception::GetHashCode(LONG* r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = 0;
    return S_OK;
    }

HRESULT Exception::GetObjectData(ISerializationInfo*, StreamingContext)
    {
    return E_NOTIMPL;
    }

HRESULT Exception::GetType(IType** r)
    {
    if (r == nullptr) { return E_INVALIDARG; }
    *r = nullptr;
    return S_OK;
    }

ComPtr<Exception> ExceptionSource::GetExceptionForHR(HRESULT hr)
    {
    return ComPtrM<Exception,HResultException>(*this, hr);
    }

ComPtr<Exception> Exception::PushStack(const string& FileName, const int Line, const string& Source)
    {
    _ASSERT(this != nullptr);
    _ASSERT(!Disposed);
    USES_CONVERSION;
    wstringstream r;
    r << L"   at ";
    r << (Source.empty()) ? L"[code]" : (A2W(Source.c_str()));
    if (!FileName.empty()) {
        r << L" in " <<
            A2W(Path::GetFileName(FileName).c_str()) <<
            L":line " <<
            Line;
        }
    if (StackTrace != nullptr) {
        r << endl;
        r << StackTrace;
        }
    StackTrace = r.str().c_str();
    return this;
    }

ComPtr<Exception> Exception::PushStack(const ObjectSource& FileName, const string& Source)
    {
    _ASSERT(this != nullptr);
    _ASSERT(!Disposed);
    return PushStack(FileName.FileName,FileName.Line,Source);
    }

ComPtr<Exception> Exception::PushStack(const ExceptionSource& Source)
    {
    _ASSERT(this != nullptr);
    _ASSERT(!Disposed);
    return PushStack(Source.ObjectSource,Source.Source);
    }

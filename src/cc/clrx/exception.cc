#include "hdrstop.h"
#include "exception.h"

Exception::Exception(const string& FileName, int LineNumber, const string& Source)
    :HelpContext(-1)
    {
    }

HResultException::HResultException(const string& FileName, const int LineNumber, const string& Source, const HRESULT SCode):
    Exception(FileName,LineNumber,Source),SCode(SCode)
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

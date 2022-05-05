#include "hdrstop.h"
#include "any.h"

#undef FormatMessage

Any::~Any()
    {
    Clear();
    }

Any::Any()
    {
    VariantInit(&value);
    }

Any::Any(const string& r)
    {
    VariantInit(&value);
    USES_CONVERSION;
    V_VT(&value)   = VT_BSTR;
    V_BSTR(&value) = SysAllocString(A2W_CP(r.c_str(),CP_ACP));
    }

Any::Any(bool r)
    {
    VariantInit(&value);
    V_VT(&value)   = VT_BOOL;
    V_BOOL(&value) = r ? -1 : 0;
    }

#define ANY(VT,T) \
Any::Any(T r) \
    { \
    VariantInit(&value); \
    V_VT(&value)   = VT_##VT; \
    V_##VT(&value) = r; \
    }

ANY(UI1,uint8_t)
ANY(UI2,uint16_t)
ANY(UI4,uint32_t)
ANY(UI8,uint64_t)
ANY(UI4,unsigned long)
ANY(I1,int8_t)
ANY(I2,int16_t)
ANY(I4,int32_t)
ANY(I8,int64_t)
ANY(I4,signed long)

//template<>
//Any::Any(string&& r)
//    {
//    VariantInit(&value);
//    USES_CONVERSION;
//    V_VT(&value)   = VT_BSTR;
//    V_BSTR(&value) = SysAllocString(A2W_CP(r.c_str(),CP_ACP));
//    }

Any::Any(PCCH r)
    {
    VariantInit(&value);
    if (r == nullptr) { V_VT(&value) = VT_NULL; }
    else
        {
        USES_CONVERSION;
        V_VT(&value)   = VT_BSTR;
        V_BSTR(&value) = SysAllocString(A2W_CP(r,CP_ACP));
        }
    }

Any::Any(PCWCH r)
    {
    VariantInit(&value);
    if (r)
        {
        V_VT(&value)   = VT_BSTR;
        V_BSTR(&value) = SysAllocString(r);
        }
    else
        {
        V_VT(&value)   = VT_NULL;
        }
    }

Any::Any(const uint8_t* r, const size_t length)
    {
    VariantInit(&value);
    if (r != nullptr) {
        if (length > 0) {
            CComSafeArray<uint8_t> o((ULONG)length);
            V_VT(&value) = VT_UI1 | VT_ARRAY;
            memcpy(o.m_psa->pvData, r, length);
            V_ARRAY(&value) = o.Detach();
            }
        }
    else
        {
        V_VT(&value) = VT_NULL;
        }
    }

Any::Any(const shared_ptr<vector<uint8_t>>& r)
    {
    VariantInit(&value);
    if (r != nullptr) {
        if (!r->empty()) {
            CComSafeArray<uint8_t> o((ULONG)r->size());
            V_VT(&value) = VT_UI1 | VT_ARRAY;
            memcpy(o.m_psa->pvData, &(*r)[0], r->size());
            V_ARRAY(&value) = o.Detach();
            }
        }
    else
        {
        V_VT(&value) = VT_NULL;
        }
    }

#define AnySpecification(Type,Suffix) \
template<> \
Any::Any(Type && r) \
    { \
    VariantInit(&value); \
    V_VT(&value)  = VT_##Suffix; \
    V_##Suffix(&value) = r; \
    } \
template<> \
Any::Any(const Type& r) \
    { \
    VariantInit(&value); \
    V_VT(&value)  = VT_##Suffix; \
    V_##Suffix(&value) = r; \
    }

//AnySpecification(unsigned long,UI4)
//AnySpecification(signed   long,I4)
//AnySpecification(uint32_t,UI4)
//AnySpecification(uint64_t,UI8)
//AnySpecification(uint16_t,UI2)
//AnySpecification( uint8_t,UI1)
//AnySpecification(int32_t,I4)
//AnySpecification(int64_t,I8)
//AnySpecification(int16_t,I2)
//AnySpecification( int8_t,I1)

Any::Any(const Any& r)
    {
    VariantInit(&value);
    r.CopyTo(*this);
    }

Any::Any(Any&& r)
    {
    VariantInit(&value);
    r.CopyTo(*this);
    }

Any& Any::CopyFrom(const Any& r)
    {
    Clear();
    r.CopyTo(*this);
    return *this;
    }

void Any::CopyTo(Any& r) const
    {
    V_VT(&r.value) = V_VT(&value);
    switch (V_VT(&value)) {
        case VT_BSTR:
            {
            V_BSTR(&r.value) = (V_BSTR(&value))
                ? SysAllocString(V_BSTR(&value))
                : nullptr;
            }
            break;
        case VT_NULL    :
        case VT_EMPTY   :
        case VT_INT     :
        case VT_UINT    :
        case VT_INT_PTR :
        case VT_UINT_PTR:
        case VT_I8      :
        case VT_I4      :
        case VT_I2      :
        case VT_I1      :
        case VT_UI4     :
        case VT_UI2     :
        case VT_UI1     :
        case VT_BOOL    :
        case VT_UI8: V_UI8(&r.value) = V_UI8(&value); break;
        default:
            if (V_ISARRAY(&value)) {
                if (V_ARRAY(&value)) {
                    SafeArrayCopy(V_ARRAY(&value),&V_ARRAY(&r.value));
                    }
                break;
                }
            ATLASSERT(false);
        }
    }

void Any::Clear()
    {
    const auto vt = V_VT(&value);
    if ((vt != VT_NULL) && (vt != VT_EMPTY)) {
        if (vt == VT_BSTR) {
            SysFreeString(V_BSTR(&value));
            V_BSTR(&value) = nullptr;
            V_VT(&value) = VT_NULL;
            return;
            }
        if (V_ISARRAY(&value)) {
            if (V_ARRAY(&value)) {
                SafeArrayDestroy(V_ARRAY(&value));
                V_ARRAY(&value) = nullptr;
                V_VT(&value) = VT_NULL;
                return;
                }
            }
        }
    VariantClear(&value);
    }

template<> basic_string<CHAR> Any::FormatMessage<CHAR>(const CHAR* format, va_list args)
    {
    size_t size;
    vector<CHAR> o((size = _vscprintf(format,args)) + 1);
    vsprintf_s(&o[0],o.size(),format,args);
    return &o[0];
    }

template<> basic_string<WCHAR> Any::FormatMessage<WCHAR>(const WCHAR* format, va_list args)
    {
    size_t size;
    vector<WCHAR> o((size = _vscwprintf(format,args)) + 1);
    vswprintf_s(&o[0],o.size(),format,args);
    return &o[0];
    }

Any::operator shared_ptr<vector<uint8_t>>() const {
    const auto vt = V_VT(&value);
    if (vt == VT_NULL)  { return nullptr; }
    if (vt == VT_EMPTY) { return make_shared<vector<uint8_t>>(); }
    ATLASSERT(V_ISARRAY(&value));
    if (V_ISARRAY(&value)) {
        if (V_ARRAY(&value)) {
            const auto size = (V_ARRAY(&value)->rgsabound[0].cElements)*SafeArrayGetElemsize(V_ARRAY(&value));
            auto r = make_shared<vector<uint8_t>>(size);
            if (size > 0) {
                memcpy(&(*r)[0], V_ARRAY(&value)->pvData, size);
                }
            return r;
            }
        }
    return nullptr;
    }

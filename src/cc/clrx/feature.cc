#include "hdrstop.h"
#include "feature.h"

#undef FormatMessage
#define const_ref(T) add_lvalue_reference_t<add_const<T>::type>

ObjectValue::ObjectValue(const ObjectValue& r):
    IsFeature(r.IsFeature),Value(r.Value),Feature(r.Feature)
    {
    }

ObjectValue::ObjectValue(ObjectValue&& r):
    IsFeature(r.IsFeature),Value(r.Value),Feature(r.Feature)
    {
    }

ObjectValue::ObjectValue():
    IsFeature(false)
    {
    }

ObjectValue::ObjectValue(ObjectFeature&& r):
    IsFeature(true),Feature(r)
    {
    }

ObjectValue::ObjectValue(const ObjectFeature& r):
    IsFeature(true),Feature(r)
    {
    }

ObjectValue::ObjectValue(const map<string, ObjectValue>& r):
    IsFeature(true),Feature(r)
    {
    }

ObjectValue::ObjectValue(const initializer_list<pair<string, ObjectValue>>& r):
    IsFeature(true)
    {
    for (const auto& i: r) {
        Feature.Source.emplace(i);
        }
    }

ObjectFeature::ObjectFeature(const initializer_list<pair<string,ObjectValue>>& source) {
    for (const auto& i: source) {
        Source.emplace(i);
        }
    }

ObjectFeature::ObjectFeature(const map<string,ObjectValue>& source):
    Source(source)
    {
    }

ObjectFeature::ObjectFeature(ObjectFeature&& r):
    Source(r.Source)
    {
    }

ObjectFeature::ObjectFeature(const ObjectFeature& r):
    Source(r.Source)
    {
    }

void ObjectFeature::WriteTo(wstringstream& o) const
    {
    WriteTo(o, 0);
    }

void ObjectFeature::WriteTo(wstringstream& o, int level) const
    {
    USES_CONVERSION;
    o << L"{" << endl;
    auto i = 0;
    ++level;
    for (const auto& value: Source) {
        if (i != 0) { o << L"," << endl; }
        o << wstring(level*2, L' ');
        o << L'"' << wstring(A2W(value.first.c_str())) << L'"' << L": ";
        value.second.WriteTo(o, level);
        ++i;
        }
    if (i > 0) {
        o << endl << wstring(level*2, L' ');
        }
    o << L"}";
    }

template<> ObjectValue::ObjectValue(const uint8_t* r, const DWORD* count):
    IsFeature(false)
    {
    if (r == nullptr) {
        Value = "{null}";
        }
    else
        {
        if (count == nullptr) {
            #ifdef _WIN64
            Value = Any::FormatMessage("{%016I64x}", r);
            #else
            Value = Any::FormatMessage("{%08I32x}", r);
            #endif
            }
        else
            {
            Value = Any(r,*count);
            }
        }
    }

ObjectValue::ObjectValue(const shared_ptr<vector<uint8_t>>& r):
    Value(r)
    {
    }

void ObjectValue::WriteTo(wstringstream& o, int level) const {
    if (IsFeature) { Feature.WriteTo(o, level); }
    else
        {
        switch (V_VT(&Value.value)) {
            case VT_BSTR:
                {
                if (V_BSTR(&Value.value)) {
                    o << L'"' << wstring(V_BSTR(&Value.value)) << L'"';
                    }
                else
                    {
                    o << L"{null}";
                    }
                }
                break;
            case VT_UI1:
            case VT_UI2:
            case VT_UI4:
            case VT_UI8:
                {
                o << V_UI8(&Value.value);
                }
                break;
            case VT_I1:
            case VT_I2:
            case VT_I4:
            case VT_I8:
                {
                o << V_I8(&Value.value);
                }
                break;
            case VT_BOOL:
                {
                o << ((V_BOOL(&Value.value) == -1)
                    ? L"true"
                    : L"false");
                }
                break;
            default:
                if (V_ISARRAY(&Value.value)) {
                    if (V_ARRAY(&Value.value)) {
                        wstringstream r;
                        const auto left  = (BYTE*)(V_ARRAY(&Value.value)->pvData);
                        const auto count = (V_ARRAY(&Value.value)->rgsabound[0].cElements)*SafeArrayGetElemsize(V_ARRAY(&Value.value));
                        r << L"{base32}:";
                        for (auto i = 0; i < count; ++i) {
                            r << Any::FormatMessage(L"%02x", left[i]);
                            }
                        o << L"\"" << r.str() << L"\"";
                        break;
                        }
                    }
                ATLASSERT(false);
            }
        }
    }

#define ObjectValueDirectSpecialization(T) \
template<> \
ObjectValue::ObjectValue(const_ref(T) r): \
    IsFeature(false),Value(r) \
    { \
    }

ObjectValueDirectSpecialization(string)
ObjectValueDirectSpecialization(const wchar_t*)
ObjectValueDirectSpecialization(const char*)
ObjectValueDirectSpecialization(uint32_t)
ObjectValueDirectSpecialization(uint64_t)
ObjectValueDirectSpecialization(uint16_t)
ObjectValueDirectSpecialization(int32_t)
ObjectValueDirectSpecialization(int64_t)
ObjectValueDirectSpecialization(int16_t)
ObjectValueDirectSpecialization(any)
ObjectValueDirectSpecialization(bool)
ObjectValueDirectSpecialization(long)
ObjectValueDirectSpecialization(unsigned long)
ObjectValueDirectSpecialization(unsigned char)

ObjectValue::ObjectValue(const char* r):
    IsFeature(false),Value(r)
    {
    }

template<>
ObjectValue::ObjectValue(const_ref(LPCSCARD_IO_REQUEST) r):
    IsFeature(r != nullptr)
    {
    Value = "{null}";
    if (r)
        {
        Feature = ObjectFeature({
            {"Protocol", ScardProtocolToString(r->dwProtocol)},
            {"Length", r->cbPciLength}
            });
        }
    }

template<>
ObjectValue::ObjectValue(const_ref(LPSCARD_IO_REQUEST) r):
    IsFeature(r != nullptr)
    {
    Value = "{null}";
    if (r)
        {
        Feature = ObjectFeature({
            {"Protocol", ScardProtocolToString(r->dwProtocol)},
            {"Length", r->cbPciLength}
            });
        }
    }

string ObjectValue::ScardProtocolToString(DWORD value)
    {
    vector<string> values;
    if (value == 0) { return "SCARD_PROTOCOL_UNDEFINED"; }
    if (value & SCARD_PROTOCOL_T0)  { values.emplace_back("SCARD_PROTOCOL_T0");  value &= ~SCARD_PROTOCOL_T0;  }
    if (value & SCARD_PROTOCOL_T1)  { values.emplace_back("SCARD_PROTOCOL_T1");  value &= ~SCARD_PROTOCOL_T1;  }
    if (value & SCARD_PROTOCOL_RAW) { values.emplace_back("SCARD_PROTOCOL_RAW"); value &= ~SCARD_PROTOCOL_RAW; }
    if (value) {values.emplace_back(Any::FormatMessage("%016I32x", value)); }
    stringstream r;
    auto j = 0;
    for (const auto& i: values) {
        if (j) { r << ","; }
        r << i;
        j++;
        }
    return r.str();
    }

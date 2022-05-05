#pragma once
#include "feature.h"

#define SELECT_RETURN_FCI_TEMPLATE 0
#define SELECT_RETURN_FCP_TEMPLATE 1
#define SELECT_RETURN_FMD_TEMPLATE 2
#define SELECT_RETURN_PROPRIETARY  3

struct scard_select_P2
    {
    uint8_t
        file_occurrence:2,
        file_control_information:2;
    };

class bertlv_object;
class ScardDecoder
    {
public:
    static ObjectValue DecodeTransmitRequest(DWORD protocol, const any& request);
    static ObjectValue DecodeTransmitResponse(DWORD protocol, const any& request, const any& response);
private:
    static ObjectValue DecodeTransmitRequest(DWORD protocol, const vector<uint8_t>& request);
    static ObjectValue DecodeTransmitResponse(DWORD protocol, const vector<uint8_t>& request, const vector<uint8_t>& response);
    static ObjectValue DecodeInstructionClass(BYTE r);
    static ObjectValue DecodeInstructionRequest(const vector<uint8_t>& request);
    static string SecureMessagingIndicationToString(BYTE value);
public:
    template<class T> static pair<string,ObjectValue> DecodeTLV(const shared_ptr<bertlv_object>& value, T predicate);
    };

#define BERTLV_CLASS_NUI 0
#define BERTLV_CLASS_APP 1
#define BERTLV_CLASS_CON 2
#define BERTLV_CLASS_PRI 3
#define BERTLV_ENC_PRI 0
#define BERTLV_ENC_CON 1
#define BERTLV_STATE_FAILED  1
#define BERTLV_STATE_SUCCESS 0

enum class bertlv_class
    {
    uni = 0,
    app = 1,
    con = 2,
    pri = 3
    };

enum class bertlv_encoding
    {
    pri = 0,
    con = 1,
    };

class bertlv_object
    {
public:
    bertlv_object(const uint8_t* buffer):
        tag(0),encoding(bertlv_encoding::pri),Class(bertlv_class::app),
        size(0),state(BERTLV_STATE_SUCCESS),header(0)
        {
        const auto r = buffer;
        encoding = (bertlv_encoding)((buffer[0] >> 5) & 0x01);
        Class    = (bertlv_class)((buffer[0] >> 6) & 0x03);
        tag      = (buffer[0]     ) & 0x1f;
        header   = *buffer++;
        if (*buffer & 0x80) {
            switch (*buffer & 0x7f) {
                case 1: size = buffer[1]; buffer += 2; break;
                case 2: size = *(uint16_t*)&buffer[1]; buffer += 3; break;
                case 3: size = ((uint32_t)buffer[1] << 16) | ((uint32_t)buffer[2] << 8) | ((uint32_t)buffer[3]); buffer += 4; break;
                case 4: size = *(uint32_t*)&buffer[1]; buffer += 5; break;
                default: state = BERTLV_STATE_FAILED; break;
                }
            }
        else
            {
            size = *buffer++;
            }
        if (state == 0) {
            if (encoding == bertlv_encoding::con) {
                uint32_t i = 0;
                values = make_shared<vector<shared_ptr<bertlv_object>>>();
                for(;;) {
                    auto j = make_shared<bertlv_object>(buffer);
                    i += j->size;
                    if (i <= size) {
                        values->emplace_back(j);
                        buffer += j->size;
                        }
                    else
                        {
                        break;
                        }
                    }
                }
            else
                {
                if (size > 0) {
                    value = make_shared<vector<uint8_t>>(size);
                    memcpy(&(*value)[0],buffer,size);
                    }
                size += (uint32_t)(buffer - r);
                }
            }
        }
public:
    uint8_t header;
    uint8_t tag;
    bertlv_encoding encoding;
    bertlv_class    Class;
    uint32_t size;
    uint8_t  state;
    shared_ptr<vector<uint8_t>> value;
    shared_ptr<vector<shared_ptr<bertlv_object>>> values;
    };

template <class T>
pair<string,ObjectValue> ScardDecoder::DecodeTLV(const shared_ptr<bertlv_object>& value, T predicate) {
    if (value->values != nullptr) {
        typedef map<string,ObjectValue>::value_type value_type;
        map<string,ObjectValue> values;
        for (const auto& i: (*(value->values))) {
            values.emplace(predicate(i));
            }
        const auto r = predicate(value);
        return pair<string,ObjectValue>(r.first, values);
        }
    else
        {
        return pair<string,ObjectValue>(predicate(value));
        }
    }

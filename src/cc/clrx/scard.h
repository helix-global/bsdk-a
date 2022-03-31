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
    };

class TLVBER
    {
public:
    TLVBER(uint8_t tag, int64_t size,const uint8_t* buffer, int64_t length):
        Tag(tag),Size(size)
        {
        ATLASSERT(size <= length);
        while (length >= size) {
            auto i = make_shared<TLVBER>(buffer[0],buffer[1],buffer + 2, length - 2);
            buffer += i->Size;
            length -= i->Size + 2;
            Children.emplace_back(i);
            }
        }
public:
    uint8_t Tag;
    size_t Size;
    vector<shared_ptr<TLVBER>> Children;
    };
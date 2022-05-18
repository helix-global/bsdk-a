#include "hdrstop.h"
#include "scard.h"
#include "trace.h"

#undef FormatMessage
#define F(...) Any::FormatMessage(__VA_ARGS__)

typedef ObjectValue(*CommandRequest)(const vector<uint8_t>&);
typedef ObjectValue(*CommandResponse)(const vector<uint8_t>&,const vector<uint8_t>&);
class CommandDescriptor
    {
public:
    CommandDescriptor(const string& CommandName, CommandRequest CommandRequest,CommandResponse CommandResponse = nullptr):
        CommandName(CommandName),CommandRequest(CommandRequest),CommandResponse(CommandResponse)
        {
        }
public:
    string CommandName;
    CommandRequest CommandRequest;
    CommandResponse CommandResponse;
    };

ObjectValue DecodeSelectRequest(const vector<uint8_t>&);
ObjectValue DecodeSelectResponse(const vector<uint8_t>&,const vector<uint8_t>&);

map<uint8_t,CommandDescriptor> DecodeTable = {
    {0x04, {"DEACTIVATE FILE", nullptr}},
    {0x0c, {"ERASE RECORD", nullptr}},
    {0x0e, {"ERASE BINARY", nullptr}},
    {0x0f, {"ERASE BINARY", nullptr}},
    {0x10, {"PERFORM SCQL OPERATION", nullptr}},
    {0x12, {"PERFORM TRANSACTION OPERATION", nullptr}},
    {0x14, {"PERFORM USER OPERATION", nullptr}},
    {0x20, {"VERIFY", nullptr}},
    {0x21, {"VERIFY", nullptr}},
    {0x22, {"MANAGE SECURITY ENVIRONMENT", nullptr}},
    {0x24, {"CHANGE REFERENCE DATA", nullptr}},
    {0x26, {"DISABLE VERIFICATION REQUIREMENT", nullptr}},
    {0x28, {"ENABLE VERIFICATION REQUIREMENT", nullptr}},
    {0x2a, {"PERFORM SECURITY OPERATION", nullptr}},
    {0x2c, {"RESET RETRY COUNTER", nullptr}},
    {0x44, {"ACTIVATE FILE", nullptr}},
    {0x46, {"GENERATE ASYMMETRIC KEY PAIR", nullptr}},
    {0x70, {"MANAGE CHANNEL", nullptr}},
    {0x82, {"EXTERNAL(MUTUAL) AUTHENTICATE", nullptr}},
    {0x84, {"GET CHALLENGE", nullptr}},
    {0x86, {"GENERAL AUTHENTICATE", nullptr}},
    {0x87, {"GENERAL AUTHENTICATE", nullptr}},
    {0x88, {"INTERNAL AUTHENTICATE", nullptr}},
    {0xa0, {"SEARCH BINARY", nullptr}},
    {0xa1, {"SEARCH BINARY", nullptr}},
    {0xa2, {"SEARCH RECORD", nullptr}},
    {0xa4, {"SELECT", DecodeSelectRequest, DecodeSelectResponse}},
    {0xb0, {"READ BINARY", nullptr}},
    {0xb1, {"READ BINARY", nullptr}},
    {0xb2, {"READ RECORD", nullptr}},
    {0xb3, {"READ RECORD", nullptr}},
    {0xc0, {"GET RESPONSE", nullptr}},
    {0xc2, {"ENVELOPE", nullptr}},
    {0xc3, {"ENVELOPE", nullptr}},
    {0xca, {"GET DATA", nullptr}},
    {0xcb, {"GET DATA", nullptr}},
    {0xd0, {"WRITE BINARY", nullptr}},
    {0xd1, {"WRITE BINARY", nullptr}},
    {0xd2, {"WRITE RECORD", nullptr}},
    {0xd6, {"UPDATE BINARY", nullptr}},
    {0xd7, {"UPDATE BINARY", nullptr}},
    {0xda, {"PUT DATA", nullptr}},
    {0xdb, {"PUT DATA", nullptr}},
    {0xdc, {"UPDATE RECORD", nullptr}},
    {0xdd, {"UPDATE RECORD", nullptr}},
    {0xe0, {"CREATE FILE", nullptr}},
    {0xe2, {"APPEND RECORD", nullptr}},
    {0xe4, {"DELETE FILE", nullptr}},
    {0xe6, {"TERMINATE DF", nullptr}},
    {0xe8, {"TERMINATE EF", nullptr}},
    {0xfe, {"TERMINATE CARD USAGE", nullptr}},
    };

ObjectValue ScardDecoder::DecodeTransmitResponse(const DWORD protocol, const any& request, const any& response)
    {
    return DecodeTransmitResponse(protocol,
        *(shared_ptr<vector<uint8_t>>)request,
        *(shared_ptr<vector<uint8_t>>)response
        );
    }

ObjectValue ScardDecoder::DecodeTransmitResponse(const DWORD protocol, const vector<uint8_t>& request, const vector<uint8_t>& response) {
    typedef map<string,ObjectValue>::value_type value_type;
    map<string,ObjectValue> values;
    auto I = response.size() - 2;
    auto status = F("{%02x%02x}", response[I], response[I+1]);
    switch (response[I]) {
        case 0x90:
            status += ":{Normal processing}";
            switch (response[I+1])
                {
                case 0x00: status += ":{No further qualification}"; break;
                }
            break;
        case 0x61:
            status += F(":{Normal processing}:{%i data bytes still available}",response[I+1]);
            break;
        case 0x62:
            status += ":{Warning processing}:{State of non-volatile memory is unchanged}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x81: status += ":{Part of returned data may be corrupted}"; break;
                case 0x82: status += ":{End of file or record reached before reading N{e} bytes}"; break;
                case 0x83: status += ":{Selected file deactivated}"; break;
                case 0x84: status += ":{File control information not formatted according specification}"; break;
                case 0x85: status += ":{Selected file in termination state}"; break;
                case 0x86: status += ":{No input data available from a sensor on the card}"; break;
                default:   status += ":{Triggering by the card}"; break;
                }
            break;
        case 0x63:
            status += ":{Warning processing}:{State of non-volatile memory has changed}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x81: status += ":{File filled up by the last write}"; break;
                default: status += ((response[I+1] >= 0xc0) && (response[I+1] <= 0xcf))
                    ? F(":{Counter:{%i}}", response[I+1] - 0xc0)
                    : "";
                }
            break;
        case 0x64:
            status += ":{Execution error}:{State of non-volatile memory is unchanged}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x01: status += ":{Immediate response required by the card}"; break;
                default:   status += ":{Triggering by the card}"; break;
                }
            break;
        case 0x65:
            status += ":{Execution error}:{State of non-volatile memory has changed}";
            switch (response[I+1])
                {
                case 0x81: status += ":{Memory failure}"; break;
                }
            break;
        case 0x66:
            status += ":{Execution error}:{Security-related issues}";
            switch (response[I+1])
                {
                case 0x00: break;
                }
            break;
        case 0x67:
            status += ":{Checking error}";
            switch (response[I+1])
                {
                case 0x00: status += ":{Wrong length}"; break;
                }
            break;
        case 0x68:
            status += ":{Checking error}:{Functions in CLA not supported}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x81: status += ":{Logical channel not supported}"; break;
                case 0x82: status += ":{Secure messaging not supported}"; break;
                case 0x83: status += ":{Last command of the chain expected}"; break;
                case 0x84: status += ":{Command chaining not supported}"; break;
                }
            break;
        case 0x69:
            status += ":{Checking error}:{Command not allowed}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x81: status += ":{Command incompatible with file structure}"; break;
                case 0x82: status += ":{Security status not satisfied}"; break;
                case 0x83: status += ":{Authentication method blocked}"; break;
                case 0x84: status += ":{Reference data not usable}"; break;
                case 0x85: status += ":{Conditions of use not satisfied}"; break;
                case 0x86: status += ":{Command not allowed (no current EF)}"; break;
                case 0x87: status += ":{Expected secure messaging data objects missing}"; break;
                case 0x88: status += ":{Incorrect secure messaging data objects}"; break;
                }
            break;
        case 0x6a:
            status += ":{Checking error}:{Wrong parameters}";
            switch (response[I+1])
                {
                case 0x00: break;
                case 0x80: status += ":{Incorrect parameters in the command data field}"; break;
                case 0x81: status += ":{Function not supported}"; break;
                case 0x82: status += ":{File or application not found}"; break;
                case 0x83: status += ":{Record not found}"; break;
                case 0x84: status += ":{Not enough memory space in the file}"; break;
                case 0x85: status += ":{N{c} inconsistent with {TLV} structure}"; break;
                case 0x86: status += ":{Incorrect parameters P{1}-P{2}}"; break;
                case 0x87: status += ":{N{c} inconsistent with parameters P{1}-P{2}}"; break;
                case 0x88: status += ":{Referenced data or reference data not found}"; break;
                case 0x89: status += ":{File already exists}"; break;
                case 0x8a: status += ":{DF name already exists}"; break;
                }
            break;
        case 0x6b:
            status += ":{Checking error}:{Wrong parameters}";
            switch (response[I+1])
                {
                case 0x00: status += ":{Wrong parameters P{1}-P{2}}"; break;
                }
            break;
        case 0x6c:
            status += F(":{Checking error}:{Wrong L{e} field}:{%i data bytes still available}",response[I+1]);
            break;
        case 0x6d:
            status += ":{Checking error}";
            switch (response[I+1])
                {
                case 0x00: status += ":{Instruction code not supported or invalid}"; break;
                }
            break;
        case 0x6e:
            status += ":{Checking error}";
            switch (response[I+1])
                {
                case 0x00: status += ":{Class not supported}"; break;
                }
            break;
        case 0x6f:
            status += ":{Checking error}";
            switch (response[I+1])
                {
                case 0x00: status += ":{No precise diagnosis}"; break;
                }
            break;
        }
    values["Status"] = status;
    values["Length"] = response.size();
    if (response.size() > 2) {
        const auto i = DecodeTable.find(request[1]);
        if (i != DecodeTable.end()) {
            if (i->second.CommandResponse != nullptr) {
                values["Details"] = i->second.CommandResponse(request,response);
                }
            }
        }
    return values;
    }

ObjectValue ScardDecoder::DecodeTransmitRequest(const DWORD protocol, const any& request) {
    return DecodeTransmitRequest(protocol, *(shared_ptr<vector<uint8_t>>)request);
    }

ObjectValue ScardDecoder::DecodeTransmitRequest(const DWORD protocol, const vector<uint8_t>& request)
    {
    typedef map<string,ObjectValue>::value_type value_type;
    map<string,ObjectValue> values;
    switch (protocol) {
        case SCARD_PROTOCOL_T0:
            {
            const auto o = (LPSCARD_T0_COMMAND)&request[0];
            if (request.size() >= 2) {
                values.emplace(value_type("Class",       DecodeInstructionClass(o->bCla)));
                values.emplace(value_type("Instruction", DecodeInstructionRequest(request)));
                }
            }
            break;
        }
    return values;
    }

ObjectValue ScardDecoder::DecodeInstructionRequest(const vector<uint8_t>& request)
    {
    ATLASSERT(request.size() > 1);
    typedef map<string,ObjectValue>::value_type value_type;
    map<string,ObjectValue> values;
    const auto i = DecodeTable.find(request[1]);
    if (i != DecodeTable.end()) {
        values.emplace(value_type("Code", Any::FormatMessage("{%02x}:{%s}", request[1], i->second.CommandName.c_str())));
        if (i->second.CommandRequest != nullptr) {
            values.emplace(value_type("Parameters", i->second.CommandRequest(request)));
            }
        return ObjectFeature(values);
        }
    return Any::FormatMessage("{%02x}", request[1]);
    }

ObjectValue ScardDecoder::DecodeInstructionClass(const BYTE r) {
    typedef map<string,ObjectValue>::value_type value_type;
    map<string,ObjectValue> values;
    if ((r & 0xe0) == 0) {
        values["CommandChainingControl"] = (r & 0x10)
            ? "The command is not the last command of a chain"
            : "The command is the last or only command of a chain";
        values["SecureMessagingIndication"] = SecureMessagingIndicationToString((r >> 2) & 0x03);
        values["LogicalChannelNumber"] = (r & 0x03);
        }
    return (values.empty())
        ? ObjectValue(r)
        : ObjectValue(values);
    }

string ScardDecoder::SecureMessagingIndicationToString(BYTE value) {
    switch (value) {
        case 0: return "No SM or no indication";
        case 1: return "Proprietary SM format";
        case 2: return "2";
        case 3: return "3";
        default: return Any::FormatMessage("%i",value);
        }
    }

ObjectValue DecodeSelectRequest(const vector<uint8_t>& request)
    {
    typedef map<string,ObjectValue>::value_type value_type;
    map<string,ObjectValue> values;
    auto r = Any::FormatMessage("{%02x}", request[2]);
    switch (request[2])
        {
        case 0x00: r = "{00}:{Select MF, DF or EF}"; break;
        case 0x01: r = "{01}:{Select child DF}"; break;
        case 0x02: r = "{02}:{Select EF under the current DF}"; break;
        case 0x03: r = "{03}:{Select parent DF of the current DF}"; break;
        case 0x04: r = "{04}:{Select by DF name}"; break;
        case 0x08: r = "{08}:{Select from the MF}"; break;
        case 0x09: r = "{09}:{Select from the current DF}"; break;
        }
    values["P1"] = r;
    r = Any::FormatMessage("{%02x}", request[3]);
    if ((request[3] & 0xf0) == 0x00) {
        switch(((scard_select_P2*)&request[3])->file_occurrence) {
            case 0x00: r += ":{First or only occurrence}"; break;
            case 0x01: r += ":{Last occurrence}"; break;
            case 0x02: r += ":{Next occurrence}"; break;
            case 0x03: r += ":{Previous occurrence}"; break;
            }
        switch(((scard_select_P2*)&request[3])->file_control_information) {
            case SELECT_RETURN_FCI_TEMPLATE: r += ":{Return FCI template}"; break;
            case SELECT_RETURN_FCP_TEMPLATE: r += ":{Return FCP template}"; break;
            case SELECT_RETURN_FMD_TEMPLATE: r += ":{Return FMD template}"; break;
            case SELECT_RETURN_PROPRIETARY : r += ":{Depends on Le}"; break;
            }
        }
    values["P2"] = r;
    values["Lc"] = request[4];
    if (request[4] > 0) {
        values["Data"] = Any(&request[0] + 5, request[4]);
        }
    return ObjectValue(values);
    }

ObjectValue DecodeSelectResponse(const vector<uint8_t>& request,const vector<uint8_t>& response)
    {
    map<string,ObjectValue> values;
    #pragma region BER-TLV
    if ((response[0] >= 0) && (response[1] <= 0xbf)) {
        const auto P2 = (scard_select_P2*)&request[3];
        if (response[0] == 0x62) {
            ATLASSERT(P2->file_control_information == SELECT_RETURN_FCP_TEMPLATE);
            values.emplace(ScardDecoder::DecodeTLV(make_shared<bertlv_object>(&response[0]),
                [](const shared_ptr<bertlv_object>& i) {
                    switch (i->header) {
                        case 0x62: return pair<string,ObjectValue>("FCP template", i->value);
                        case 0x80: return pair<string,ObjectValue>("Number of data bytes in the file, excluding structural information", i->value);
                        case 0x81: return pair<string,ObjectValue>("Number of data bytes in the file, including structural information", i->value);
                        case 0x82: return pair<string,ObjectValue>("File descriptor", i->value);
                        case 0x83: return pair<string,ObjectValue>("File identifier", i->value);
                        case 0x84: return pair<string,ObjectValue>("DF name", i->value);
                        case 0x85: return pair<string,ObjectValue>("Proprietary information not encoded in BER-TLV", i->value);
                        case 0x86: return pair<string,ObjectValue>("Security attribute in proprietary format", i->value);
                        case 0x87: return pair<string,ObjectValue>("Identifier of an EF containing an extension of the file control information", i->value);
                        case 0x88: return pair<string,ObjectValue>("Short EF identifier", i->value);
                        case 0x8a: return pair<string,ObjectValue>("Life cycle status byte", i->value);
                        case 0x8b: return pair<string,ObjectValue>("Security attribute referencing the expanded format", i->value);
                        case 0x8c: return pair<string,ObjectValue>("Security attribute in compact format", i->value);
                        case 0x8d: return pair<string,ObjectValue>("Identifier of an EF containing security environment templates", i->value);
                        case 0x8e: return pair<string,ObjectValue>("Channel security attribute", i->value);
                        case 0xa0: return pair<string,ObjectValue>("Security attribute template for data objects", i->value);
                        case 0xa1: return pair<string,ObjectValue>("Security attribute template in proprietary format", i->value);
                        case 0xa2: return pair<string,ObjectValue>("Template consisting of one or more pairs of data objects", i->value);
                        case 0xa5: return pair<string,ObjectValue>("Proprietary information encoded in BER-TLV", i->value);
                        case 0xab: return pair<string,ObjectValue>("Security attribute template in expanded format", i->value);
                        case 0xac: return pair<string,ObjectValue>("Cryptographic mechanism identifier template", i->value);
                        }
                    return pair<string,ObjectValue>(F("{%02x}", i->header),0);
                    }));
            return values;
            }
        }
    #pragma endregion
    return "{none}";
    }

template <>
void TraceDescriptor::Pack(vector<uint8_t>& target, const_ref(LPCSCARD_IO_REQUEST) value) {
    (value != nullptr)
        ? PackSequence(target,value->dwProtocol,value->cbPciLength)
        : Pack(target,nullptr);
    }

template <>
void TraceDescriptor::Pack(vector<uint8_t>& target, const_ref(LPSCARD_IO_REQUEST) value) {
    Pack(target,const_cast<LPCSCARD_IO_REQUEST>(value));
    }

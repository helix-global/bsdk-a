#include "hdrstop.h"
#include "scard.h"
#include "trace.h"

#undef FormatMessage
#define F(...) Any::FormatMessage(__VA_ARGS__)

#undef  THIS_FILE
#define THIS_FILE "detours.cc"
#undef  nameof
#define nameof(E) #E

#ifdef USE_MSDETOURS

#define H(E) H##E
#define O(E) O##E
#define T(E) T##E
#define TRY       __try     {
#define FINALLY } __finally {
#define END     }
#define GetProcAddressT(Module,E)  (T(E))GetProcAddress(Module,#E)

template <>
const TraceDescriptor& TraceDescriptor::packD(vector<uint8_t>& target, const_ref(LPCSCARD_IO_REQUEST) value) const {
    return (value != nullptr)
        ? packG(target,Asn1PrivateStruct,
            value->dwProtocol,
            value->cbPciLength)
        : packD(target,nullptr);
    }

template <>
const TraceDescriptor& TraceDescriptor::packD(vector<uint8_t>& target, const_ref(LPSCARD_READERSTATEA) value) const {
    return (value != nullptr)
        ? packG(target,Asn1PrivateStruct,
            value->szReader,
            DWORD_PTR(value->pvUserData),
            value->dwCurrentState,
            value->dwEventState,
            vector<BYTE>(value->rgbAtr,value->rgbAtr+value->cbAtr))
        : packD(target,nullptr);
    }

template <>
const TraceDescriptor& TraceDescriptor::packD(vector<uint8_t>& target, const_ref(LPSCARD_IO_REQUEST) value) const {
    return packD(target,const_cast<LPCSCARD_IO_REQUEST>(value));
    }

#endif
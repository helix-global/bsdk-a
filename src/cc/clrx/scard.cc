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
TraceDescriptor& TraceDescriptor::packD(vector<uint8_t>& target, const_ref(LPCSCARD_IO_REQUEST) value) {
    return (value != nullptr)
        ? packGI(target,
            make_tuple(1,value->dwProtocol),
            make_tuple(2,value->cbPciLength))
        : packD(target,nullptr);
    }

template <>
TraceDescriptor& TraceDescriptor::packD(vector<uint8_t>& target, const_ref(LPSCARD_IO_REQUEST) value) {
    return packD(target,const_cast<LPCSCARD_IO_REQUEST>(value));
    }

#endif
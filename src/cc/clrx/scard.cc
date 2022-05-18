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
void TraceDescriptor::Pack(vector<uint8_t>& target, const_ref(LPCSCARD_IO_REQUEST) value) {
    (value != nullptr)
        ? PackSequence(target,value->dwProtocol,value->cbPciLength)
        : Pack(target,nullptr);
    }

template <>
void TraceDescriptor::Pack(vector<uint8_t>& target, const_ref(LPSCARD_IO_REQUEST) value) {
    Pack(target,const_cast<LPCSCARD_IO_REQUEST>(value));
    }

#endif
#pragma once

#include "object.h"

#pragma push_macro("FormatMessage")
#undef FormatMessage

typedef class Any
    {
public:
    VARIANT value;
public:
    Any();
    Any(const Any& r);
    Any(Any&& r);
    Any(const string& r);
    Any(PCWCH);
    Any(PCCH);
    Any(const uint8_t* r, size_t length);
    Any(bool);
    Any(uint16_t);
    Any(uint32_t);
    Any(uint64_t);
    Any(int16_t);
    Any(int32_t);
    Any(int64_t);
    Any(int8_t);
    Any(uint8_t);
    Any(signed   long);
    Any(unsigned long);
    //template<class T> Any(const T& r);
    //template<class T> Any(T&& r);
public:
    Any& operator =(const Any& r) { return CopyFrom(r); };
    Any& operator =(Any&& r)      { return CopyFrom(r); }
private:
    void Clear();
    void CopyTo(Any& r) const;
    Any& CopyFrom(const Any& r);
public:
    template<class E> static basic_string<E> FormatMessage(const E* format, va_list args);
    template<class E> static basic_string<E> FormatMessage(const E* format, ...) {
        va_list args;
        va_start(args, format);
        return FormatMessage<E>(format,args);
        va_end(args);
        }
public:
    operator shared_ptr<vector<uint8_t>>() const;
public:
    ~Any();
    } any;

#pragma pop_macro("FormatMessage")

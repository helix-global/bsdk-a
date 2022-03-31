#pragma once
#include <utility>
#include "object.h"

#define JSON_STATE_PROPERTY_NAME  2
#define JSON_STATE_PROPERTY_VALUE 3

template<class E>
class JsonWriter : Object<IUnknown>
    {
public:
    explicit JsonWriter(const ObjectSource& ObjectSource,basic_iostream<E>& Target):
        Object<IUnknown>(ObjectSource),target(Target),state(0),indention(0)
        {
        }
public:
    JsonWriter& WriteStartObject() {
        target << (E)'{' << endl;
        indention++;
        state = 1;
        return *this;
        }
    JsonWriter& WriteEndObject()
        {
        target << endl;
        target << string(indention*2, ' ') << (E)'}';
        indention--;
        return *this;
        }
    JsonWriter& WritePropertyName(const string&);
    JsonWriter& WriteStartArray()
        {
        return *this;
        }
    JsonWriter& WriteEndArray()
        {
        return *this;
        }
    template<class T> JsonWriter& WriteValue(const T& value);
    template<int   N> JsonWriter& WriteValue(const char (& value)[N]);
private:
    int state;
    int indention;
    basic_iostream<E>& target;
    };

template<> inline JsonWriter<char>& JsonWriter<char>::WritePropertyName(const string& PropertyName) {
    if (state == JSON_STATE_PROPERTY_VALUE) {
        target << "," << endl;
        }
    target << string(indention*2, ' ');
    target << '"' << PropertyName << '"' << ": ";
    state = JSON_STATE_PROPERTY_NAME;
    return *this;
    }

template <>
template <int N>
inline JsonWriter<char>& JsonWriter<char>::WriteValue(const char (& value)[N])
    {
    target << '"' << value << '"';
    state = JSON_STATE_PROPERTY_VALUE;
    return *this;
    }

template <>template <>
inline JsonWriter<char>& JsonWriter<char>::WriteValue<unsigned long long>(const unsigned long long& value)
    {
    target << value;
    state = JSON_STATE_PROPERTY_VALUE;
    return *this;
    }

template <>template <>
inline JsonWriter<char>& JsonWriter<char>::WriteValue<unsigned long>(const unsigned long& value)
    {
    target << value;
    state = JSON_STATE_PROPERTY_VALUE;
    return *this;
    }

template <>template <>
inline JsonWriter<char>& JsonWriter<char>::WriteValue<const wchar_t*>(wchar_t const * const &value)
    {
    USES_CONVERSION;
    target << '"' << string(W2A(value)) << '"';
    state = JSON_STATE_PROPERTY_VALUE;
    return *this;
    }



#pragma once
#include "object.h"
#include "any.h"

class ObjectValue;
class ObjectFeature
    {
    friend class ObjectValue;
public:
    ObjectFeature() = default;
    ObjectFeature(ObjectFeature&&);
    ObjectFeature(const ObjectFeature&);
    ObjectFeature(const initializer_list<pair<string,ObjectValue>>& source);
    ObjectFeature(const map<string,ObjectValue>& source);
public:
    ObjectFeature& operator=(const ObjectFeature&) = default;
    ObjectFeature& operator=(ObjectFeature&&)      = default;
private:
    map<string,ObjectValue> Source;
public:
    void WriteTo(wstringstream& o) const;
private:
    void WriteTo(wstringstream& o, int level) const;
    };

class ObjectValue
    {
    friend class ObjectFeature;
public:
    ObjectValue();
    ObjectValue(const ObjectValue& );
    ObjectValue(      ObjectValue&&);
    ObjectValue(const initializer_list<pair<string,ObjectValue>>& r);
    ObjectValue(const map<string,ObjectValue>& r);
    ObjectValue(const ObjectFeature& r);
    ObjectValue(ObjectFeature&& r);
    ObjectValue(const char* r);
    ObjectValue(const shared_ptr<vector<uint8_t>>& r);
    template<class T> ObjectValue(const uint8_t* r,const  T* count);
    template<class T> ObjectValue(const T& r);
public:
    ObjectValue& operator=(const ObjectValue&) = default;
    ObjectValue& operator=(ObjectValue&&)      = default;
private:
    bool IsFeature;
    any  Value;
    ObjectFeature Feature;
public:
    static string ScardProtocolToString(DWORD value);
private:
    void WriteTo(wstringstream& o, int level) const;
    };



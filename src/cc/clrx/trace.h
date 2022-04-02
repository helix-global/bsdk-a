#pragma once
#include "exception.h"

#define ASN1_EOC  0x00
#define ASN1_NULL 0x05
#define ASN1_NULL 0x05
#define ASN1_SEQ  0x10

#define BER_ENC_PRI 0
#define BER_ENC_CON 1

template<class... T> class TracePacker;

class TraceDescriptor
    {
public:
    TraceDescriptor(const string& shortname, const string& longname):
        ShortName(shortname),LongName(longname)
        {
        }
public:
    const string& ShortName;
    const string& LongName;
private:
    template<class T> static void Pack(vector<uint8_t>& o, const T& value);
    template<class T, class... P> static void Pack(vector<uint8_t>& o, const T& frst, const P&... args) {
        Pack(o, frst);
        Pack(o, args...);
        }
private:
    static void PackValue(vector<uint8_t>& o, const uint8_t* buffer, size_t count);
    static void PackBody(vector<uint8_t>& target, const uint8_t* source, size_t count);
    static void PackBody(vector<uint8_t>& target, const vector<uint8_t>& source);
    static void PackSize(vector<uint8_t>& target, size_t value);
    static void PackType(vector<uint8_t>& target, uint8_t type);
public:
    template<class... P> void Enter(LONG& scope, P... args) {
        vector<uint8_t> o;
        Pack(o, args...);
        vector<uint8_t> target;
        PackType(target, 0xe0);
        PackSize(target, o.size());
        PackBody(target, o);
        EnterI(scope,target,nullptr);
        }
    template<class T, class... P> T Leave(LONG scope,const HRESULT status,const T& r, const P&... args) {
        vector<uint8_t> o;
        Pack(o, status);
        Pack(o, r, args...);
        vector<uint8_t> target;
        PackType(target, 0xe0);
        PackSize(target, o.size());
        PackBody(target, o);
        LeaveI(scope,target,nullptr);
        return r;
        }
private:
    HRESULT EnterI(LONG&,const vector<uint8_t>& target, const NullableReference<LONG64>&);
    HRESULT LeaveI(LONG, const vector<uint8_t>& target, const NullableReference<LONG64>&);
    };

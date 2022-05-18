#pragma once
#include "exception.h"

#define Asn1OctetString   0x04
#define Asn1Null          0x05
#define Asn1Utf8String    0x0c
#define Asn1GeneralString 0x1b
#define Asn1UnicodeString 0x1e
#define Asn1Sequence      0x10
#define ASN1_EOC   0x00

#define Universal       (0 << 6)
#define Application     (1 << 6)
#define ContextSpecific (2 << 6)
#define Private         (3 << 6)
#define Asn1ExplicitConstructed 0x20

#define nameof(expr) #expr
#define packof(expr) make_tuple(string(u8 ## #expr),expr)

template<class... T> class TracePacker;

class TraceDescriptor
    {
public:
    TraceDescriptor(const string& shortname, const string& longname):
        ShortName(shortname),LongName(longname)
        {
        }
public:
    string ShortName;
    string LongName;
private:
    template<class T> static void Pack(vector<uint8_t>& target, const tuple<string,T>& value)
        {
        PackSequence(target,
            get<0>(value),
            get<1>(value));
        }
    template<class T> static void Pack(vector<uint8_t>& target, const pair<int,T>& value) {
        vector<uint8_t> o;
        Pack(o, value.second);
        packT(o, Application | value.first);
        PackSequence(target,
            get<0>(value),
            get<1>(value));
        }
    template<class T> static void Pack(vector<uint8_t>& o, const T& value);
    template<class T, class... P> static void Pack(vector<uint8_t>& o, const T& frst, const P&... args) {
        Pack(o, frst);
        Pack(o, args...);
        }
    template<class... P> static void PackSequence(vector<uint8_t>& o, const P&... args) {
        vector<uint8_t> target;
        Pack(target, args...);
        packT(o, Asn1Sequence | Asn1ExplicitConstructed);
        packS(o, target.size());
        packB(o, target);
        }
private:
    static void PackValue(vector<uint8_t>& o, const uint8_t* buffer, size_t count);
    static void packB(vector<uint8_t>& target, const uint8_t* source, size_t count);
    static void packB(vector<uint8_t>& target, const vector<uint8_t>& source);
    static void packS(vector<uint8_t>& target, size_t value);
    static void packT(vector<uint8_t>& target, uint8_t type);
public:
    template<class... P> void Enter(LONG& scope, P... args) {
        vector<uint8_t> target;
        PackSequence(target,args...);
        EnterI(scope,target,nullptr);
        }
    template<class T, class... P> T Leave(const LONG scope,const HRESULT scode,const T& r, const P&... args) {
        vector<uint8_t> target;
        PackSequence(target,r,args...);
        LeaveI(scope,scode,target,nullptr);
        return r;
        }
private:
           HRESULT EnterI(LONG&,const vector<uint8_t>& target, const NullableReference<LONG64>&) const;
    static HRESULT LeaveI(LONG, HRESULT, const vector<uint8_t>& target, const NullableReference<LONG64>&);
    };

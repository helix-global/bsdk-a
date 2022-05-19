#pragma once
#include "exception.h"

#define Asn1Universal               0x00
#define Asn1Application             0x40
#define Asn1ContextSpecific         0x80
#define Asn1Private                 0xc0
#define Asn1ExplicitConstructed     0x20

#define Asn1OctetString    (Asn1Universal | 0x04)
#define Asn1Null           (Asn1Universal | 0x05)
#define Asn1Utf8String     (Asn1Universal | 0x0c)
#define Asn1GeneralString  (Asn1Universal | 0x1b)
#define Asn1UnicodeString  (Asn1Universal | 0x1e)
#define Asn1Sequence       (Asn1Universal | 0x10)
#define Asn1EndOfContent   (Asn1Universal | 0x00)
#define Asn1PrivateDirect  (Asn1Private   | 0x00)
#define Asn1PrivatePointer (Asn1Private   | 0x01)
#define Asn1PrivateStruct  (Asn1Private   | 0x02)
#define Asn1PrivateNull    (Asn1Private   | 0x03)

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
    template<class T> static void packD(vector<uint8_t>& target, const tuple<string,T>& value)
        {
        packG(target,
            get<0>(value),
            get<1>(value));
        }
    template<class T> TraceDescriptor& packD(vector<uint8_t>& target, const tuple<int,T>& value) {
        vector<uint8_t> o;
        packB(o, (const uint8_t*)&get<1>(value), sizeof(T));
        packT(target, Asn1ContextSpecific | get<0>(value));
        packS(target, o.size());
        packB(target, o);
        return *this;
        }

    template<class T> TraceDescriptor& packD(vector<uint8_t>& target, const tuple<int,T*>& value) {
        vector<uint8_t> o;
        packD(o, get<1>(value));
        packT(target, Asn1ContextSpecific | get<0>(value));
        packS(target, o.size());
        packB(target, o);
        return *this;
        }

    template<class T> TraceDescriptor& packD(vector<uint8_t>& o, const T& value);
    template<class T, class... P> TraceDescriptor& packD(vector<uint8_t>& o, const T& frst, const P&... args) {
        packD(o, frst);
        packD(o, args...);
        return *this;
        }
    template<class... P> TraceDescriptor& packGI(vector<uint8_t>& o, const P&... args) {
        packGE(o, Asn1PrivateStruct, args...);
        return *this;
        }
    template<class... P> TraceDescriptor& packGE(vector<uint8_t>& o, uint8_t type, const P&... args) {
        vector<uint8_t> target;
        packD(target, args...);
        packT(o, type|Asn1ExplicitConstructed);
        packS(o, target.size());
        packB(o, target);
        return *this;
        }
private:
    TraceDescriptor& PackValue(vector<uint8_t>& o, const uint8_t* buffer, size_t count);
    static void packB(vector<uint8_t>& target, const uint8_t* source, size_t count);
    static void packB(vector<uint8_t>& target, const vector<uint8_t>& source);
    static void packS(vector<uint8_t>& target, size_t value);
    static void packT(vector<uint8_t>& target, uint8_t type);
public:
    template<class... P> void Enter(LONG& scope, P... args) {
        vector<uint8_t> target;
        packGE(target,Asn1ContextSpecific|1,args...);
        EnterI(scope,target,nullptr);
        }
    template<class T, class... P> T Leave(const LONG scope,const HRESULT scode,const T& r, const P&... args) {
        vector<uint8_t> target;
        packGE(target,Asn1ContextSpecific|3,make_tuple(0,r),args...);
        LeaveI(scope,scode,target,nullptr);
        return r;
        }
private:
           HRESULT EnterI(LONG&,const vector<uint8_t>& target, const NullableReference<LONG64>&) const;
    static HRESULT LeaveI(LONG, HRESULT, const vector<uint8_t>& target, const NullableReference<LONG64>&);
    };

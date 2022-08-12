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
#define Asn1PrivateArray   (Asn1Private   | 0x03)

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
    TraceDescriptor(const TraceDescriptor&) = default;
    TraceDescriptor(TraceDescriptor&&) = default;
public:
    string ShortName;
    string LongName;
private:
    template<class T> static void packD(vector<uint8_t>& target, const tuple<string,T>& value)
        {
        packG(target,Asn1PrivateStruct,
            get<0>(value),
            get<1>(value));
        }
    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const tuple<int,T>& value) const
        {
        vector<uint8_t> o;
        packB(o, (const uint8_t*)&get<1>(value), sizeof(T));
        packT(target, Asn1ContextSpecific | get<0>(value));
        packS(target, o.size());
        packB(target, o);
        return *this;
        }

    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const tuple<int,T*>& value) const
        {
        vector<uint8_t> o;
        packD(o, get<1>(value));
        packT(target, Asn1ContextSpecific | get<0>(value));
        packS(target, o.size());
        packB(target, o);
        return *this;
        }

    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const vector<T>& value) const {
        if (value.empty()) { return packD(target, nullptr); }
        vector<uint8_t> o;
        for (const auto& i:value) {
            packB(o, (const uint8_t*)&i, sizeof(T));
            }
        packT(target, Asn1PrivateDirect);
        packS(target, o.size());
        packB(target, o);
        return *this;
        }
    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const list<T>& value) const {
        if (value.empty()) { return packD(target, nullptr); }
        vector<uint8_t> o;
        for (const auto& i:value) {
            packD(o, i);
            }
        packT(target, Asn1PrivateArray|Asn1ExplicitConstructed);
        packS(target, o.size());
        packB(target, o);
        return *this;
        }
    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const basic_string<T>& value) const {
        if (value.empty()) { return packD(target, nullptr); }
        vector<uint8_t> o;
        for (const auto& i:value) {
            packB(o, (const uint8_t*)&i, sizeof(T));
            }
        packT(target, Asn1PrivateDirect);
        packS(target, o.size());
        packB(target, o);
        return *this;
        }
    template<class T> const TraceDescriptor& packD(vector<uint8_t>& target, const T& value) const;
    template<class T, class... P> const TraceDescriptor& packD(vector<uint8_t>& o, const T& frst, const P&... args) const
        {
        packD(o, frst);
        packD(o, args...);
        return *this;
        }
    template<class... P> const TraceDescriptor& packG(vector<uint8_t>& o, uint8_t type, const P&... args) const
        {
        vector<uint8_t> target;
        packD(target, args...);
        packT(o, type|Asn1ExplicitConstructed);
        packS(o, target.size());
        packB(o, target);
        return *this;
        }
private:
    const TraceDescriptor& PackValue(vector<uint8_t>& o, const uint8_t* buffer, size_t count) const;
    static void packB(vector<uint8_t>& target, const uint8_t* source, size_t count);
    static void packB(vector<uint8_t>& target, const vector<uint8_t>& source);
    static void packS(vector<uint8_t>& target, size_t value);
    static void packT(vector<uint8_t>& target, uint8_t type);
public:
    template<class... P> void Enter(LONG& scope, P... args) const {
        vector<uint8_t> target;
        packG(target,Asn1ContextSpecific|1,args...);
        EnterI(scope,target,nullptr);
        }
    template<class T, class... P> T Leave(const LONG scope,const HRESULT scode,const T& r) const {
        vector<uint8_t> target;
        vector<uint8_t> o;
        packD(o, make_tuple(0,r));
        packT(target, Asn1ContextSpecific|3|Asn1ExplicitConstructed);
        packS(target, o.size());
        packB(target, o);
        LeaveI(scope,scode,target,nullptr);
        return r;
        }
    template<class T, class... P> T Leave(const LONG scope,const HRESULT scode,const T& r, const P&... args) const {
        vector<uint8_t> target;
        vector<uint8_t> o;
        packD(o, make_tuple(0,r));
        packG(o, uint8_t(Asn1ContextSpecific|1),args...);
        packT(target, Asn1ContextSpecific|3|Asn1ExplicitConstructed);
        packS(target, o.size());
        packB(target, o);
        LeaveI(scope,scode,target,nullptr);
        return r;
        }
private:
    HRESULT EnterI(LONG&,const vector<uint8_t>& target, const NullableReference<LONG64>&) const;
    HRESULT LeaveI(LONG, HRESULT, const vector<uint8_t>& target, const NullableReference<LONG64>&) const;
    };

template<class T>
inline vector<T> make_vector(const T* buffer, size_t count)
    {
    return vector<T>(buffer,count);
    }

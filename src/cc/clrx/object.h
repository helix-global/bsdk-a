#pragma once

#include <unknwn.h>

template<class... T> class Object;
template<> class Object<IUnknown> : public virtual IUnknown
    {
protected:
    Object();
public:
    virtual ~Object();
protected:
    ULONG m_ref;
    };

template<class... T> class Object:
    virtual public Object<IUnknown>,
    virtual public T...
    {
protected:
    Object();
public:
    virtual ~Object();
    };

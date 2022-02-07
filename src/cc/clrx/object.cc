#include "hdrstop.h"
#include "object.h"

Object<IUnknown>::Object()
    :m_ref(1)
    {
    }

Object<IUnknown>::~Object()
    {
    }

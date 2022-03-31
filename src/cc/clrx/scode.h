#pragma once
#include <string>

using namespace std;

class scode
    {
public:
    scode(const HRESULT value):
        value(value)
        {
        }
public:
    string str() const;
private:
    HRESULT value;
    };

// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#pragma warning(push)
#pragma warning(disable: 4668 4820 4365 4625 4917 4987 4626 4986 5026 5027)
#pragma warning(disable: 4530)
#include "framework.h"
#include <windows.h>
#include <wincrypt.h>
#include <propvarutil.h>
#include <oaidl.h>
#include <propidl.h>
#include <atlbase.h>
#include <atlsafe.h>
#include <winscard.h>
#include <string>
#include <iostream>
#include <sstream>
#include <map>
#include <vector>
#pragma warning(pop)

#ifdef USE_MSDETOURS
#include <detours.h>
#endif

#endif //PCH_H

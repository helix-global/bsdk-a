#pragma once
#include "object.h"

#pragma push_macro("FormatMessage")
#undef FormatMessage

enum class LoggingSeverity
    {
    Info,
    Warning,
    Error,
    Debug
    };

MIDL_INTERFACE("87e670f7-b2b2-4979-a9c4-1df99df6b23b")
ILoggingSource : IUnknown
    {
    STDMETHOD(Log)(LoggingSeverity severity, BSTR message) = 0;
    };

class LoggingSource : public Object<IUnknown,ILoggingSource>
    {
public:
    template<class E> static void Log(LoggingSeverity severity, const E* format, ...)
        {
        va_list args;
        va_start(args, format);
        LogCore(severity, format, args);
        va_end(args);
        }
private:
    template<class E> static void LogCore(LoggingSeverity severity, const E* format, va_list args) {
        LogCore(severity, FormatMessage(format, args));
        }
    static void LogCore(LoggingSeverity severity, const wstring& message);
    static void LogCore(LoggingSeverity severity, const  string& message);
    };

#pragma pop_macro("FormatMessage")

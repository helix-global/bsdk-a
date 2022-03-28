#include "hdrstop.h"
#include "logging.h"
#include "module.h"

#ifdef USE_LOG4CPP
#include <log4cpp/Category.hh>
#include <log4cpp/Appender.hh>
#include <log4cpp/FileAppender.hh>
#include <log4cpp/OstreamAppender.hh>
#include <log4cpp/Layout.hh>
#include <log4cpp/BasicLayout.hh>
#include <log4cpp/Priority.hh>
#ifdef _DEBUG
#ifdef _MT
#pragma comment(lib,"log4cppMDd.lib")
#else
#pragma comment(lib,"log4cppMTd.lib")
#endif
#else
#ifdef _MT
#pragma comment(lib,"log4cppMD.lib")
#else
#pragma comment(lib,"log4cppMT.lib")
#endif
#endif

namespace log4cpp
    {
    class DefaultLayout final : public Layout {
        public:
            DefaultLayout()          = default;
            virtual ~DefaultLayout() = default;
            virtual std::string format(const LoggingEvent& event) {
                std::ostringstream message;
                const auto& priorityName = Priority::getPriorityName(event.priority);
                const auto c = time(nullptr);
                message << priorityName << " "
                    << event.categoryName << " " << event.ndc << ": "
                    << event.message << std::endl;
                return message.str();
            }
        };
    }
log4cpp::Appender* DefaultAppender = nullptr;
#endif

#undef FormatMessage

void LoggingSource::LogCore(const LoggingSeverity severity, const wstring& message)
    {
    USES_CONVERSION;
    LogCore(severity, W2A(message.c_str()));
    }

void LoggingSource::LogCore(const LoggingSeverity severity, const string& message)
    {
    #ifdef USE_LOG4CPP
    if (DefaultAppender == nullptr) {
        DefaultAppender = new log4cpp::FileAppender("default", FormatMessage("%s-%s-%s.log",
            Path::GetFileNameWithoutExtension(Module::ModuleName).c_str(),
            Module::ProcessName.c_str(),
            FormatMessage(time(nullptr),"%Y-%m-%dT%H-%M-%S").c_str()));
        DefaultAppender->setLayout(new log4cpp::DefaultLayout());
        auto& root = log4cpp::Category::getRoot();
        root.setPriority(log4cpp::Priority::DEBUG);
        root.addAppender(DefaultAppender);
        }
    switch (severity)
        {
        case LoggingSeverity::Info:     { log4cpp::Category::getRoot().info(message);  } break;
        case LoggingSeverity::Warning:  { log4cpp::Category::getRoot().warn(message);  } break;
        case LoggingSeverity::Error:    { log4cpp::Category::getRoot().error(message); } break;
        case LoggingSeverity::Debug:    { log4cpp::Category::getRoot().debug(message); } break;
        case LoggingSeverity::Trace:    { log4cpp::Category::getRoot().debug(message); } break;
        }
    #endif
    }

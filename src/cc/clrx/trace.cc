#include "hdrstop.h"
#include "trace.h"
#include <sqlite3.h>
#include <stack>
#include "scode.h"
#include "module.h"

#undef FormatMessage

static sqlite3* DataSource = nullptr;
static string DataSourceFileName;
static sqlite3_stmt* InsertStatement = nullptr;
static sqlite3_stmt* LeaveStatement  = nullptr;
typedef stack<LONG> stack_type;
typedef pair<DWORD, shared_ptr<stack_type>> pair_type;
static map<DWORD, shared_ptr<stack_type>> ParentId;

bool FolderExists(const string& folder)
    {
    const auto r = GetFileAttributesA(folder.c_str());
    return (r != INVALID_FILE_ATTRIBUTES) && (r & FILE_ATTRIBUTE_DIRECTORY);
    }

LONG GetParentId() {
    const auto ThreadId = GetCurrentThreadId();
    auto i = ParentId.find(ThreadId);
    if (i != ParentId.end()) {
        if (i->second == nullptr) { return 0; }
        if (i->second->empty()) { return 0; }
        return i->second->top();
        }
    return 0;
    }

void PushParentId(LONG value) {
    const auto ThreadId = GetCurrentThreadId();
    auto i = ParentId.find(ThreadId);
    if (i != ParentId.end()) {
        i->second->push(value);
        }
    else
        {
        shared_ptr<stack_type> j;
        ParentId[ThreadId] = (j = make_shared<stack_type>());
        j->push(value);
        }
    }

void PopParentId()
    {
    const auto ThreadId = GetCurrentThreadId();
    auto i = ParentId.find(ThreadId);
    if (i != ParentId.end()) {
        if (!i->second->empty()) { i->second->pop(); }
        if (i->second->empty()) {
            ParentId.erase(i);
            }
        }
    }

template <>
void TraceDescriptor::Pack(vector<uint8_t>& o, wchar_t const* const& value) {
    PackValue(o,(uint8_t*)value, (wcslen(value) + 1)*2);
    }

template <>
void TraceDescriptor::Pack(vector<uint8_t>& target, const vector<uint8_t>& value) {
    if (value.empty())
        {
        PackType(target,ASN1_NULL);
        PackSize(target,0x00);
        }
    else
        {
        PackValue(target, &value[0],value.size());
        }
    }

#define PACK(T) \
template <> \
void TraceDescriptor::Pack(vector<uint8_t>& o, const T& value) { \
    PackValue(o,(uint8_t*)&value, sizeof(T)); \
    } \
template <> \
void TraceDescriptor::Pack(vector<uint8_t>& target, T* const & value) { \
    if (value != nullptr) \
        { \
        Pack(target,*value); \
        } \
    else \
        { \
        PackType(target,ASN1_NULL); \
        PackSize(target,0x00); \
        } \
    }

PACK(LONG)
PACK(ULONG)
PACK(ULONGLONG)

void TraceDescriptor::PackSize(vector<uint8_t>& target, const size_t value) {
    if ((value >= 0) && (value < 0x80)) {
        target.emplace_back(uint8_t(value & 0xff));
        return;
        }
    if (value == (size_t)(-1)) {
        target.emplace_back(0x80);
        return;
        }
    vector<uint8_t> values;
    auto i = value;
    while (i > 0) {
        values.emplace_back(uint8_t(i & 0xff));
        i >>= 8;
        }
    const auto c = values.size();
    target.emplace_back(uint8_t(c | 0x80));
    for (i = c; i > 0; i--){
        target.emplace_back(values[i - 1]);
        }
    }

void TraceDescriptor::PackType(vector<uint8_t>& target, const uint8_t type) {
    target.emplace_back(type);
    }

void TraceDescriptor::PackBody(vector<uint8_t>& target, const vector<uint8_t>& source) {
    for (const auto& i: source) {
        target.emplace_back(i);
        }
    }

void TraceDescriptor::PackBody(vector<uint8_t>& target, const uint8_t* source, const size_t count) {
    for (size_t i = 0; i < count; i++) {
        target.emplace_back(
            source[i]);
        }
    }

void TraceDescriptor::PackValue(
    vector<uint8_t>& target, const uint8_t* source,
    const size_t count)
    {
    if (source != nullptr) {
        PackType(target, 0xc0);
        PackSize(target, count);
        PackBody(target, source, count);
        }
    else
        {
        PackType(target, 0x05);
        PackSize(target, 0x00);
        }
    }

HRESULT TraceDescriptor::EnterI(LONG& scope, const vector<uint8_t>& target, const NullableReference<LONG64>& size) {
    int status;
    scope = 0;
    if (DataSource == nullptr) {
        #define OrderThreadId   1
        #define OrderLongName   2
        #define OrderShortName  3
        #define OrderEntryTime  4
        #define OrderSize       5
        #define OrderParentId   6
        #define OrderParameters 7
        const auto module = Module::FullProcessName;
        const auto process = Path::GetFileNameWithoutExtension(module);
        const auto folder = Path::GetDirectoryName(module) + ".sqlite3";
        if (!FolderExists(folder))
            {
            CreateDirectoryA(folder.c_str(), nullptr);
            SetFileAttributesA(folder.c_str(), FILE_ATTRIBUTE_DIRECTORY | FILE_ATTRIBUTE_HIDDEN);
            }
        DataSourceFileName = folder + "\\trace-" + process + "-" + Object<IUnknown>::FormatMessage(time(nullptr), "%Y-%m-%d-%H-%M-%S") + ".db";
        char* message = nullptr;
        if ((status = sqlite3_open(DataSourceFileName.c_str(), &DataSource)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_exec(DataSource,
            "CREATE TABLE[TraceInfo]                             \
                (                                                \
                [Id]        [INTEGER] PRIMARY KEY AUTOINCREMENT, \
                [ThreadId]  [INTEGER] NULL,                      \
                [LongName]  [TEXT]    NULL,                      \
                [ShortName] [TEXT]    NULL,                      \
                [EntryTime] [BIGINT]  NOT NULL,                  \
                [Size]      [BIGINT]  NULL,                      \
                [LeavePoint][INTEGER] NULL,                      \
                [ParentId]  [INTEGER] NULL,                      \
                [Parameters][BLOB]    NULL                       \
                )", nullptr,nullptr,&message)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_prepare_v2(DataSource, "\
            INSERT INTO TraceInfo([ThreadId],[LongName],[ShortName],[EntryTime],[Size],[ParentId],[Parameters])\
                   VALUES         (@ThreadId,@LongName, @ShortName, @EntryTime, @Size, @ParentId, @Parameters)",
            -1, &InsertStatement, nullptr)) != SQLITE_OK) {
            return COR_E_INVALIDOPERATION;
            }
        }
    if (InsertStatement != nullptr) {
        if ((status = sqlite3_bind_int(InsertStatement, OrderThreadId, union_cast<int>(GetCurrentThreadId()))) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_bind_text(InsertStatement, OrderLongName, LongName.c_str(), -1, SQLITE_TRANSIENT)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = !ShortName.empty()
            ? sqlite3_bind_text(InsertStatement, OrderShortName, ShortName.c_str(), -1, SQLITE_TRANSIENT)
            : sqlite3_bind_null(InsertStatement, OrderShortName)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_bind_int64(InsertStatement, OrderEntryTime, GetTickCount64())) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = (size != nullptr)
            ? sqlite3_bind_int64(InsertStatement, OrderSize, size.Value)
            : sqlite3_bind_null (InsertStatement, OrderSize)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_bind_int64(InsertStatement, OrderParentId, GetParentId())) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = (!target.empty())
            ? sqlite3_bind_blob64(InsertStatement, OrderParameters,&target[0],target.size(),nullptr)
            : sqlite3_bind_null  (InsertStatement, OrderParameters)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
        if ((status = sqlite3_step(InsertStatement)) != SQLITE_DONE) { return COR_E_INVALIDOPERATION; }
        PushParentId(scope = LONG(sqlite3_last_insert_rowid(DataSource)));
        sqlite3_reset(InsertStatement);
        }
    return S_OK;
    }

HRESULT TraceDescriptor::LeaveI(LONG scope,const vector<uint8_t>& target, const NullableReference<LONG64>&)
    {
    int status;
    if (scope == 0)            { return E_INVALIDARG;           }
    if (DataSource == nullptr) { return COR_E_INVALIDOPERATION; }
    PopParentId();
    if (LeaveStatement == nullptr) {
        if ((status = sqlite3_prepare_v2(DataSource,
            "INSERT INTO TraceInfo([EntryTime],[LeavePoint],[Parameters]) VALUES (@EntryTime,@LeavePoint,@Parameters)",
            -1, &LeaveStatement, nullptr)) != SQLITE_OK) {
            return COR_E_INVALIDOPERATION;
            }
        }
    if ((status = sqlite3_bind_int64(LeaveStatement, 1, GetTickCount64())) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
    if ((status = sqlite3_bind_int(LeaveStatement, 2, scope)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
    if ((status = (!target.empty())
        ? sqlite3_bind_blob64(InsertStatement, 3,&target[0],target.size(),nullptr)
        : sqlite3_bind_null  (InsertStatement, 3)) != SQLITE_OK) { return COR_E_INVALIDOPERATION; }
    if ((status = sqlite3_step(LeaveStatement)) != SQLITE_DONE) { return COR_E_INVALIDOPERATION; }
    sqlite3_reset(LeaveStatement);
    return S_OK;
    }

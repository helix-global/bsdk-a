#include "hdrstop.h"
#include "object.h"

wstring Path::GetFileName(const wstring& path)
    {
    wchar_t drive[_MAX_DRIVE];
    wchar_t dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _wsplitpath_s(path.c_str(), drive, dir, filename, ext);
    return wstring(filename) + ext;
    }

string Path::GetFileName(const string& path)
    {
    char drive[_MAX_DRIVE];
    char dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _splitpath_s(path.c_str(), drive, dir, filename, ext);
    return std::string(filename) + ext;
    }

#include "hdrstop.h"
#include "object.h"

template<> basic_string<WCHAR> Path::GetFileName(const basic_string<WCHAR>& path)
    {
    wchar_t drive[_MAX_DRIVE];
    wchar_t dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _wsplitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<WCHAR>(filename) + ext;
    }

template<> basic_string<CHAR> Path::GetFileName(const basic_string<CHAR>& path)
    {
    char drive[_MAX_DRIVE];
    char dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _splitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<CHAR>(filename) + ext;
    }

template<> basic_string<WCHAR> Path::GetFileNameWithoutExtension(const basic_string<WCHAR>& path)
    {
    wchar_t drive[_MAX_DRIVE];
    wchar_t dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _wsplitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<WCHAR>(filename);
    }

template<> basic_string<CHAR> Path::GetFileNameWithoutExtension(const basic_string<CHAR>& path)
    {
    char drive[_MAX_DRIVE];
    char dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _splitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<CHAR>(filename);
    }

template<> basic_string<WCHAR> Path::GetDirectoryName(const basic_string<WCHAR>& path)
    {
    wchar_t drive[_MAX_DRIVE];
    wchar_t dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _wsplitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<WCHAR>(drive) + dir;
    }

template<> basic_string<CHAR> Path::GetDirectoryName(const basic_string<CHAR>& path)
    {
    char drive[_MAX_DRIVE];
    char dir[_MAX_DIR], filename[_MAX_FNAME], ext[_MAX_EXT];
    _splitpath_s(path.c_str(), drive, dir, filename, ext);
    return basic_string<CHAR>(drive) + dir;
    }

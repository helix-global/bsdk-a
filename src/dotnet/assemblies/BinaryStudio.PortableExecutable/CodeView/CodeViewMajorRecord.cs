using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewMajorRecord
        {
        public CV8_MAJOR_RECORD_TYPE Type { get; }
        public Int32 Length { get; }
        public Byte[] Content { get; }

        internal unsafe CodeViewMajorRecord(CV8_MAJOR_RECORD_TYPE type, Int32 length, Byte* content)
            {
            Type = type;
            Length = length;
            var r = new Byte[length];
            for (var i = 0; i < length; i++) {
                r[i] = content[i];
                }
            Content = r;
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return Type.ToString();
            }

        internal static unsafe CodeViewMajorRecord From(CV8_MAJOR_RECORD_TYPE type, Int32 length, Byte* content) {
            switch (type) {
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_SOURCE_FILENAME_STRING_TABLE: { return new CodeViewSourceFileNameStringTableRecord(type, length, content); }
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_SOURCE_FILE_INFO:             { return new CodeViewSourceFileInfoRecord(type, length, content);            }
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_LINE_NUMBERS_FOR_SECTION:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_SYMBOL_INFORMATION:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_NAME_OF_OBJECT_FILE:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_CREATOR_SIGNATURE:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_CODE_LABEL:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_LOCAL_DATA:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_GLOBAL_DATA:
                case CV8_MAJOR_RECORD_TYPE.CV8_MAJOR_RECORD_TYPE_PROCEDURE_START:
                default:
                    {
                    return new CodeViewMajorRecord(type, length, content);
                    }
                }
            }
        }
    }
using System;

namespace BinaryStudio.PlatformUI
    {
    internal struct CodeMarkerExStartEnd : IDisposable
        {
        private Int32 _end;
        private Byte[] _aBuff;

        internal CodeMarkerExStartEnd(Int32 begin, Int32 end, Byte[] aBuff, Boolean correlated = false)
            {
            _aBuff = correlated ? CodeMarkers.AttachCorrelationId(aBuff, Guid.NewGuid()) : aBuff;
            _end = end;
            CodeMarkers.Instance.CodeMarkerEx(begin, _aBuff);
            }

        internal CodeMarkerExStartEnd(Int32 begin, Int32 end, Guid guidData, Boolean correlated = false)
            {
            this = new CodeMarkerExStartEnd(begin, end, guidData.ToByteArray(), correlated);
            }

        internal CodeMarkerExStartEnd(Int32 begin, Int32 end, String stringData, Boolean correlated = false)
            {
            this = new CodeMarkerExStartEnd(begin, end, CodeMarkers.StringToBytesZeroTerminated(stringData), correlated);
            }

        internal CodeMarkerExStartEnd(Int32 begin, Int32 end, UInt32 uintData, Boolean correlated = false)
            {
            this = new CodeMarkerExStartEnd(begin, end, BitConverter.GetBytes(uintData), correlated);
            }

        internal CodeMarkerExStartEnd(Int32 begin, Int32 end, UInt64 ulongData, Boolean correlated = false)
            {
            this = new CodeMarkerExStartEnd(begin, end, BitConverter.GetBytes(ulongData), correlated);
            }

        public void Dispose()
            {
            if (_end == 0)
                return;
            CodeMarkers.Instance.CodeMarkerEx(_end, _aBuff);
            _end = 0;
            _aBuff = null;
            }
        }
    }
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace shell
    {
    internal class ETraceInfoEntry
        {
        public IList<ETraceInfoEntry> Entries { get; }
        public Int64 Id { get;set; }
        public Int64 ParentId { get;set; }
        public String ShortName { get;set; }
        public TimeSpan Duration { get;set; }
        public Byte[] InputParameters { get;set; }
        public Byte[] OutputParameters { get;set; }
        public Asn1Object DecodedInputParameters  { get;set; }
        public Asn1Object DecodedOutputParameters { get;set; }

        public ETraceInfoEntry()
            {
            Entries = new ObservableCollection<ETraceInfoEntry>();
            }
        }
    }
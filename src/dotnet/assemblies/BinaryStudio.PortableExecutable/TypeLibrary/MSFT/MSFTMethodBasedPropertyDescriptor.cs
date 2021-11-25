using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class MSFTMethodBasedPropertyDescriptor : TypeLibraryPropertyDescriptor
        {
        public MSFTMethodBasedPropertyDescriptor(ITypeLibraryTypeDescriptor type, ITypeLibraryMethodDescriptor gmi, ITypeLibraryMethodDescriptor smi)
            : base(type)
            {
            GetMethod = gmi;
            SetMethod = smi;
            var parameters = new List<ITypeLibraryParameterDescriptor>();
            if ((gmi != null) && (smi != null)) {
                if (gmi.Id != smi.Id) { throw new ArgumentOutOfRangeException(nameof(smi)); }
                Id = gmi.Id;
                if (Equals(gmi.ReturnType, smi.ReturnType)) {
                    if (gmi.ReturnType.Name != "HRESULT") { throw new ArgumentOutOfRangeException(nameof(smi)); }
                    if (gmi.Parameters.Count != smi.Parameters.Count) { throw new ArgumentOutOfRangeException(nameof(smi)); }
                    var c = gmi.Parameters.Count - 1;
                    if (!gmi.Parameters[c].IsRetval) { throw new ArgumentOutOfRangeException(nameof(gmi)); }
                    if (!gmi.Parameters[c].IsOut)    { throw new ArgumentOutOfRangeException(nameof(gmi)); }
                    for (var i = 0; i < c; ++i) {
                        if (!Equals(gmi.Parameters[i].ParameterType,smi.Parameters[i].ParameterType)) { throw new ArgumentOutOfRangeException(nameof(smi)); }
                        parameters.Add(gmi.Parameters[i]);
                        }
                    PropertyType = smi.Parameters[c].ParameterType;
                    if (String.Equals(gmi.Name, smi.Name)) { Name = smi.Name; }
                    else
                        {
                        throw new NotSupportedException();
                        }
                    if (String.Equals(gmi.HelpString, smi.HelpString)) { HelpString = gmi.HelpString; }
                    else
                        {
                        HelpString = gmi.HelpString ?? smi.HelpString;
                        }
                    }
                else
                    {
                    throw new NotSupportedException();
                    }
                }
            else if (gmi != null) {
                var c = gmi.Parameters.Count - 1;
                if (!gmi.Parameters[c].IsRetval) { throw new ArgumentOutOfRangeException(nameof(gmi)); }
                if (!gmi.Parameters[c].IsOut)    { throw new ArgumentOutOfRangeException(nameof(gmi)); }
                for (var i = 0; i < c; ++i) { parameters.Add(gmi.Parameters[i]); }
                Name = gmi.Name;
                HelpString = gmi.HelpString;
                Id = gmi.Id;
                PropertyType = Dereference(gmi.Parameters[gmi.Parameters.Count - 1].ParameterType);
                }
            else
                {
                var c = smi.Parameters.Count - 1;
                for (var i = 0; i < c; ++i) { parameters.Add(smi.Parameters[i]); }
                Name = smi.Name;
                HelpString = smi.HelpString;
                Id = smi.Id;
                PropertyType = smi.Parameters[c].ParameterType;
                }
            Parameters = new ReadOnlyCollection<ITypeLibraryParameterDescriptor>(parameters);
            }

        private static ITypeLibraryTypeDescriptor Dereference(ITypeLibraryTypeDescriptor source)
            {
            if (!source.IsPointer) { throw new ArgumentOutOfRangeException(nameof(source)); }
            return source.UnderlyingType;
            }

        public override String Name { get; }
        public override String HelpString { get; }
        public override Int32 Id { get; }
        public override ITypeLibraryTypeDescriptor PropertyType { get; }
        public override ITypeLibraryMethodDescriptor GetMethod { get; }
        public override ITypeLibraryMethodDescriptor SetMethod { get; }
        public override IList<ITypeLibraryParameterDescriptor> Parameters { get; }
        }
    }
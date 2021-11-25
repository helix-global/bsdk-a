using System.ComponentModel;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [TypeConverter(typeof(X509SubjectTypeConverter))]
    public enum X509SubjectType
        {
        CA,
        EndEntity
        }
    }
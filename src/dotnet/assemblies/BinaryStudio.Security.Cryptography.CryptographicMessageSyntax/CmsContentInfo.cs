using System;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /// <summary>
    /// The <see cref="CmsContentInfo" /> class represents the CMS ContentInfo
    /// data structure as defined in the CMS standards document.<br/>This data
    /// structure is the basis for all CMS messages.
    /// </summary>
    public abstract class CmsContentInfo : CmsObject
        {
        /// <summary>
        /// Creates an instance of the <see cref="CmsContentInfo"/> class by using an ASN1 object as the data.
        /// </summary>
        /// <param name="source">An ASN1 object that represents the data from which to create the <see cref="CmsContentInfo"/> object.</param>
        protected CmsContentInfo(Asn1Object source)
            : base(source)
            {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if (!(source is Asn1ContextSpecificObject))  { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (((Asn1ContextSpecificObject)source).Type != 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
            }
        }
    }
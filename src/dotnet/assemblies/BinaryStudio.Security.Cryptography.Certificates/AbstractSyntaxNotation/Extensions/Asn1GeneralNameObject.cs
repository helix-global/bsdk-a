using System;
using System.Xml;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions
    {
    internal abstract class Asn1GeneralNameObject: Asn1LinkObject<Asn1ContextSpecificObject>,IX509GeneralName
        {
        protected internal Asn1GeneralNameObject(Asn1ContextSpecificObject source)
            : base(source)
            {
            }

        public override String ToString()
            {
            return GetType().Name;
            }

        public virtual Boolean IsEmpty
            {
            get { return false; }
            }

        X509GeneralNameType IX509GeneralName.Type { get { return InternalType; }}
        protected abstract X509GeneralNameType InternalType { get; }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("GeneralName");
            writer.WriteAttributeString("Type", InternalType.ToString());
            writer.WriteAttributeString("Type.Numeric", ((Int32)InternalType).ToString());
            writer.WriteStartElement("GeneralName.Value");
            writer.WriteString(ToString());
            writer.WriteEndElement();
            writer.WriteEndElement();
            }
        }
    }
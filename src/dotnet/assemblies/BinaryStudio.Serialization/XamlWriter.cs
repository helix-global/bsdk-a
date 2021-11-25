using System;
using System.Xml;

namespace BinaryStudio.Serialization
    {
    public class XamlWriter : XmlWriter, IXamlWriter
        {
        private XmlWriter writer;
        public XamlWriter(XmlWriter writer)
            {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            this.writer = writer;
            }

        /// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0".</summary>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument()
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0" and the standalone attribute.</summary>
        /// <param name="standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no". </param>
        /// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument(Boolean standalone)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, closes any open elements or attributes and puts the writer back in the Start state.</summary>
        /// <exception cref="T:System.ArgumentException">The XML document is invalid. </exception>
        public override void WriteEndDocument()
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes the DOCTYPE declaration with the specified name and optional attributes.</summary>
        /// <param name="name">The name of the DOCTYPE. This must be non-empty. </param>
        /// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="pubid" /> and <paramref name="sysid" /> are replaced with the value of the given arguments. </param>
        /// <param name="sysid">If <paramref name="pubid" /> is null and <paramref name="sysid" /> is non-null it writes SYSTEM "sysid" where <paramref name="sysid" /> is replaced with the value of this argument. </param>
        /// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument. </param>
        /// <exception cref="T:System.InvalidOperationException">This method was called outside the prolog (after the root element). </exception>
        /// <exception cref="T:System.ArgumentException">The value for <paramref name="name" /> would result in invalid XML. </exception>
        public override void WriteDocType(String name, String pubid, String sysid, String subset)
            {
            throw new NotImplementedException();
            }

        #region M:WriteStartElement(String,String,String)
        /**
         * <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace and prefix.</summary>
         * <param name="prefix">The namespace prefix of the element.</param>
         * <param name="localname">The local name of the element.</param>
         * <param name="ns">The namespace URI to associate with the element.</param>
         * <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
         * <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
         */
        public override void WriteStartElement(String prefix, String localname, String ns)
            {
            writer.WriteStartElement(prefix, localname, ns);
            }
        #endregion
        #region M:WriteEndElement
        /**
         * <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
         * <exception cref="T:System.InvalidOperationException">This results in an invalid XML document.</exception>
         */
        public override void WriteEndElement()
            {
            writer.WriteEndElement();
            }
        #endregion

        /// <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
        public override void WriteFullEndElement()
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
        /// <param name="prefix">The namespace prefix of the attribute. </param>
        /// <param name="localName">The local name of the attribute. </param>
        /// <param name="ns">The namespace URI for the attribute. </param>
        /// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
        public override void WriteStartAttribute(String prefix, String localName, String ns)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)" /> call.</summary>
        public override void WriteEndAttribute()
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing the specified text.</summary>
        /// <param name="text">The text to place inside the CDATA block. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document. </exception>
        public override void WriteCData(String text)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the specified text.</summary>
        /// <param name="text">Text to place inside the comment. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document. </exception>
        public override void WriteComment(String text)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.</summary>
        /// <param name="name">The name of the processing instruction. </param>
        /// <param name="text">The text to include in the processing instruction. </param>
        /// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.<paramref name="name" /> is either null or String.Empty.This method is being used to create an XML declaration after <see cref="M:System.Xml.XmlWriter.WriteStartDocument" /> has already been called. </exception>
        public override void WriteProcessingInstruction(String name, String text)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes out an entity reference as &amp;name;.</summary>
        /// <param name="name">The name of the entity reference. </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="name" /> is either null or String.Empty. </exception>
        public override void WriteEntityRef(String name)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, forces the generation of a character entity for the specified Unicode character value.</summary>
        /// <param name="ch">The Unicode character for which to generate a character entity. </param>
        /// <exception cref="T:System.ArgumentException">The character is in the surrogate pair character range, 0xd800 - 0xdfff. </exception>
        public override void WriteCharEntity(Char ch)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes out the given white space.</summary>
        /// <param name="ws">The string of white space characters. </param>
        /// <exception cref="T:System.ArgumentException">The string contains non-white space characters. </exception>
        public override void WriteWhitespace(String ws)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes the given text content.</summary>
        /// <param name="text">The text to write. </param>
        /// <exception cref="T:System.ArgumentException">The text string contains an invalid surrogate pair. </exception>
        public override void WriteString(String text)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, generates and writes the surrogate character entity for the surrogate character pair.</summary>
        /// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF. </param>
        /// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF. </param>
        /// <exception cref="T:System.ArgumentException">An invalid surrogate character pair was passed. </exception>
        public override void WriteSurrogateCharEntity(Char lowChar, Char highChar)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes text one buffer at a time.</summary>
        /// <param name="buffer">Character array containing the text to write. </param>
        /// <param name="index">The position in the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />; the call results in surrogate pair characters being split or an invalid surrogate pair being written.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="buffer" /> parameter value is not valid.</exception>
        public override void WriteChars(Char[] buffer, Int32 index, Int32 count)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes raw markup manually from a character buffer.</summary>
        /// <param name="buffer">Character array containing the text to write. </param>
        /// <param name="index">The position within the buffer indicating the start of the text to write. </param>
        /// <param name="count">The number of characters to write. </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="buffer" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
        public override void WriteRaw(Char[] buffer, Int32 index, Int32 count)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, writes raw markup manually from a string.</summary>
        /// <param name="data">String containing the text to write. </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="data" /> is either null or String.Empty. </exception>
        public override void WriteRaw(String data)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
        /// <param name="buffer">Byte array to encode.</param>
        /// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is less than zero. -or-The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public override void WriteBase64(Byte[] buffer, Int32 index, Int32 count)
            {
            throw new NotImplementedException();
            }

        #region M:Close
        /**
         * <summary>When overridden in a derived class, closes this stream and the underlying stream.</summary>
         * <exception cref="T:System.InvalidOperationException">A call is made to write more output after Close has been called or the result of this call is an invalid XML document.</exception>
         */
        public override void Close()
            {
            writer = null;
            }
        #endregion
        #region M:Flush
        /**
         * <summary>When overridden in a derived class, flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.</summary>
         */
        public override void Flush()
            {
            writer.Flush();
            }
        #endregion

        /// <summary>When overridden in a derived class, returns the closest prefix defined in the current namespace scope for the namespace URI.</summary>
        /// <returns>The matching prefix or null if no matching namespace URI is found in the current scope.</returns>
        /// <param name="ns">The namespace URI whose prefix you want to find. </param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="ns" /> is either null or String.Empty. </exception>
        public override String LookupPrefix(String ns)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, gets the state of the writer.</summary>
        /// <returns>One of the <see cref="T:System.Xml.WriteState" /> values.</returns>
        public override WriteState WriteState { get; }

        #region M:IXamlWriter.WriteElementScope(String,String,String):IDisposable
        IDisposable IXamlWriter.WriteElementScope(String prefix, String localname, String ns) {
            WriteStartElement(prefix, localname, ns);
            return new Scope(WriteEndElement);
            }
        #endregion
        #region M:IXamlWriter.WriteElementScope(String):IDisposable
        IDisposable IXamlWriter.WriteElementScope(String localname) {
            WriteStartElement(localname);
            return new Scope(WriteEndElement);
            }
        #endregion

        private class Scope : IDisposable {
            private readonly Action mi;
            public Scope(Action mi) {
                this.mi = mi;
                }

            public void Dispose()
                {
                mi();
                }
            }
        }
    }
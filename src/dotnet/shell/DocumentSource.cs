﻿using System;
using System.IO;
using System.Xml;

namespace shell
    {
    internal abstract class DocumentSource
        {
        public Object Source { get; }
        public DesiredDocumentType Type { get; }
        protected DocumentSource(Object source, DesiredDocumentType type)
            {
            Source = source;
            Type = type;
            }

        public static DocumentSource Load(Object source) {
            if (source is String S) { return Load(S); }
            throw new NotSupportedException();
            }

        public static DocumentSource Load(String source) {
            if (String.IsNullOrWhiteSpace(source)) { throw new NotSupportedException(); }
            if (File.Exists(source)) {
                #region DesiredDocumentType.XmlFile
                    {
                    var Target = new XmlDocument();
                    try
                        {
                        Target.Load(source);
                        return new XmlTextDocumentSource(new Uri($"file://{source}"));
                        }
                    catch
                        {
                        }
                    }
                #endregion
                }
            return null;
            }
        }
    }
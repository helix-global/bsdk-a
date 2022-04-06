using System;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace shell
    {
    internal class DocumentViewFactory
        {
        public static View CreateView(Object o) {
            if (o is Asn1Certificate crt)
                {

                }
            return new View<Object>(
                new ContentControl
                    {
                    Content = o
                    });
            }
        }
    }
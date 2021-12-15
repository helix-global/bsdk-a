﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography;
using Options;
using Options.Descriptors;

namespace Operations
    {
    internal abstract class Operation
        {
        public static ILogger Logger { get;set; }
        public TextWriter Out { get; }
        public TextWriter Error { get; }
        protected Operation(TextWriter output, TextWriter error, IList<OperationOption> args)
            {
            Out = output;
            Error = error ?? output;
            }

        public static IList<OperationOption> Parse(String[] args) {
            var r = new List<OperationOption>();
            for (var i = 0; i < args.Length; ++i) {
                foreach (var descriptor in descriptors) {
                    if (descriptor.TryParse(args[i], out var option)) {
                        r.Add(option);
                        }
                    }
                }
            return r;
            }

        protected static readonly IList<OptionDescriptor> descriptors = new List<OptionDescriptor>();
        static Operation()
            {
            foreach (var type in typeof(OptionDescriptor).Assembly.GetTypes()) {
                if ((typeof(OptionDescriptor).IsAssignableFrom(type)) && (!type.IsAbstract)) {
                    descriptors.Add((OptionDescriptor)Activator.CreateInstance(type));
                    }
                }
            }

        public abstract void Execute(TextWriter output);
        protected static Boolean IsNullOrEmpty<T>(ICollection<T> value) {
            return (value == null) || (value.Count == 0);
            }

        protected static unsafe void RequestConsoleSecureStringEventHandler(Object sender, RequestSecureStringEventArgs e)
            {
            Console.WriteLine($@"Type pin-code for container ""{e.Container}""");
            Console.Write("Pin-code:");
            var o = Console.ReadLine();
            fixed (Char* c = o)
                {
                e.SecureString = new SecureString(c, o.Length);
                }
            }

        protected static unsafe void RequestWindowSecureStringEventHandler(Object sender, RequestSecureStringEventArgs e)
            {
            e.SecureString = new SecureString();
            e.SecureString.AppendChar('1');
            e.SecureString.AppendChar('2');
            e.SecureString.AppendChar('3');
            e.SecureString.AppendChar('4');
            e.SecureString.AppendChar('5');
            e.SecureString.AppendChar('6');
            e.SecureString.AppendChar('7');
            e.SecureString.AppendChar('8');
            e.StoreSecureString = true;
            }
        }
    }
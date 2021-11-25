using System;
using System.Windows;
using System.Windows.Input;

namespace shell
    {
    internal class DocumentGroupCommandSource : ICommandSource
        {
        public ICommand Command { get; }
        public Object CommandParameter { get; }
        public IInputElement CommandTarget { get; }
        public DocumentGroupCommandSource(IInputElement target, ICommand command) {
            CommandTarget = target;
            Command = command;
            }
        }
    }
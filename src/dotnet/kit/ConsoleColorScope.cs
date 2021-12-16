using System;

namespace Kit
    {
    public class ConsoleColorScope : IDisposable
        {
        private readonly ConsoleColor color;
        public ConsoleColorScope()
            {
            color = Console.ForegroundColor;
            }

        public ConsoleColorScope(ConsoleColor color)
            {
            this.color = Console.ForegroundColor;
            Console.ForegroundColor = color;
            }

        public void Dispose()
            {
            Console.ForegroundColor = color;
            }
        }
    }
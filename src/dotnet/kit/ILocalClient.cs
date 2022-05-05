
using System;

public interface ILocalClient : IDisposable
    {
    Int32 Main(String[] args);
    void OnCancelKeyPress(Object sender, ConsoleCancelEventArgs e);
    }

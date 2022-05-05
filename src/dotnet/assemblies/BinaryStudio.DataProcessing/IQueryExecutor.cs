using System;

namespace BinaryStudio.DataProcessing
    {
    public interface IQueryExecutor
        {
        Object Execute();
        }

    public interface IQueryExecutor<T>: IQueryExecutor
        {
        T Execute();
        }
    }
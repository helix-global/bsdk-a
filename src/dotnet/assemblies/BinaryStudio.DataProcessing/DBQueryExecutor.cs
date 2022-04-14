using System;

namespace BinaryStudio.DataProcessing
    {
    public abstract class DBQueryExecutor<T> :
        IQueryExecutor,
        IQueryExecutor<T>
        {
        public abstract T Execute();

        Object IQueryExecutor.Execute()
            {
            return Execute();
            }
        }
    }
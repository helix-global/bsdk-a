using System;

namespace BinaryStudio.DataProcessing
    {
    public abstract class DBQueryFactory
        {
        public abstract Type GetQueryType();
        public abstract Type GetExecutorType();
        }
    }
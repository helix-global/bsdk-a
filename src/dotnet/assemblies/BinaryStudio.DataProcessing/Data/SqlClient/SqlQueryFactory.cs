using System;

namespace BinaryStudio.DataProcessing
    {
    public class SqlQueryFactory : DBQueryFactory
        {
        public override Type GetQueryType()
            {
            throw new NotImplementedException();
            }

        public override Type GetExecutorType()
            {
            throw new NotImplementedException();
            }
        }
    }
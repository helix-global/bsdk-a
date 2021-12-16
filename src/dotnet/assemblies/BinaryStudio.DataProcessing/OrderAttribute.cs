using System;

namespace BinaryStudio.DataProcessing
    {
    public class OrderAttribute : Attribute
        {
        public Int32 Order { get; }
        public OrderAttribute(Int32 order)
            {
            Order = order;
            }
        }
    }